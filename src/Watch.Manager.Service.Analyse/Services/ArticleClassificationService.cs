namespace Watch.Manager.Service.Analyse.Services;

using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;

using Watch.Manager.Service.Analyse.Models;
using Watch.Manager.Service.Database.Context;
using Watch.Manager.Service.Database.Entities;

/// <summary>
///     Service de classification automatique des articles par IA utilisant les embeddings existants.
/// </summary>
internal sealed class ArticleClassificationService : IArticleClassificationService
{
    private readonly ArticlesContext context;
    private readonly IExtractEmbeddingAI embeddingService;
    private readonly ILogger<ArticleClassificationService> logger;

    /// <summary>
    ///     Initializes a new instance of the <see cref="ArticleClassificationService" /> class.
    /// </summary>
    /// <param name="context">The database context used to access and manage articles.</param>
    /// <param name="embeddingService">The service used to extract embeddings for article classification.</param>
    /// <param name="logger">The logger instance used for logging diagnostic and operational messages.</param>
    public ArticleClassificationService(ArticlesContext context, IExtractEmbeddingAI embeddingService, ILogger<ArticleClassificationService> logger)
    {
        this.context = context;
        this.embeddingService = embeddingService;
        this.logger = logger;
    }

    /// <inheritdoc />
    public async Task<IReadOnlyList<CategoryClassificationResult>> ClassifyArticleAsync(int articleId,
                                                                                        ClassificationOptions? options = null,
                                                                                        CancellationToken cancellationToken = default)
    {
        var article = await this.context.Articles
                                .FirstOrDefaultAsync(a => a.Id == articleId, cancellationToken).ConfigureAwait(false);

        if (article == null)
        {
            this.logger.LogWarning("Article with ID {ArticleId} not found", articleId);
            return [];
        }

        return await this.ClassifyArticleAsync(article, options, cancellationToken).ConfigureAwait(false);
    }

    /// <inheritdoc />
    public async Task<IReadOnlyList<CategoryClassificationResult>> ClassifyArticleAsync(Article article, ClassificationOptions? options = null, CancellationToken cancellationToken = default)
    {
        options ??= new();
        var results = new List<CategoryClassificationResult>();

        // Récupérer toutes les catégories actives
        var categories = await this.context.Categories
                                   .Where(c => c.IsActive)
                                   .ToListAsync(cancellationToken).ConfigureAwait(false);

        if (categories.Count == 0)
        {
            this.logger.LogInformation("No active categories found for classification");
            return results;
        }

        // Pour chaque catégorie, calculer un score de classification
        foreach (var category in categories)
        {
            var (score, reason) = await this.CalculateClassificationScore(article, category, options, cancellationToken).ConfigureAwait(false);

            if (score >= options.MinSuggestionScore)
            {
                results.Add(new()
                {
                    CategoryId = category.Id,
                    CategoryName = category.Name,
                    ConfidenceScore = score,
                    Reason = reason,
                    IsRecommendedForAutoAssignment = score >= options.MinAutoClassificationScore,
                });
            }
        }

        // Trier par score décroissant et limiter le nombre de suggestions
        return
        [
            .. results
              .OrderByDescending(r => r.ConfidenceScore)
              .Take(options.MaxSuggestionsPerArticle),
        ];
    }

    /// <inheritdoc />
    public async Task<IReadOnlyDictionary<int, IReadOnlyList<CategoryClassificationResult>>> ClassifyArticlesBatchAsync(IEnumerable<int> articleIds,
                                                                                                                        ClassificationOptions? options = null,
                                                                                                                        CancellationToken cancellationToken = default)
    {
        var results = new Dictionary<int, IReadOnlyList<CategoryClassificationResult>>();

        var articles = await this.context.Articles
                                 .Where(a => articleIds.Contains(a.Id))
                                 .ToListAsync(cancellationToken).ConfigureAwait(false);

        foreach (var article in articles)
        {
            var classifications = await this.ClassifyArticleAsync(article, options, cancellationToken).ConfigureAwait(false);
            results[article.Id] = classifications;
        }

        return results;
    }

    /// <inheritdoc />
    public async Task<int> AutoAssignCategoriesAsync(int articleId,
                                                     ClassificationOptions? options = null,
                                                     CancellationToken cancellationToken = default)
    {
        var classifications = await this.ClassifyArticleAsync(articleId, options, cancellationToken).ConfigureAwait(false);
        var autoAssignments = classifications.Where(c => c.IsRecommendedForAutoAssignment).ToList();

        if (autoAssignments.Count == 0)
            return 0;

        var assignedCount = 0;

        foreach (var classification in autoAssignments)
        {
            // Vérifier si l'article n'est pas déjà assigné à cette catégorie
            var existingAssignment = await this.context.ArticleCategories
                                               .AnyAsync(ac => ac.ArticleId == articleId && ac.CategoryId == classification.CategoryId, cancellationToken).ConfigureAwait(false);

            if (!existingAssignment)
            {
                var articleCategory = new ArticleCategory
                {
                    ArticleId = articleId,
                    CategoryId = classification.CategoryId,
                    ConfidenceScore = classification.ConfidenceScore,
                    IsManual = false,
                    AssignedAt = DateTime.UtcNow,
                };

                _ = this.context.ArticleCategories.Add(articleCategory);
                assignedCount++;

                this.logger.LogInformation(
                    "Auto-assigned article {ArticleId} to category {CategoryName} with confidence {Confidence:F2}",
                    articleId,
                    classification.CategoryName,
                    classification.ConfidenceScore);
            }
        }

        if (assignedCount > 0)
            _ = await this.context.SaveChangesAsync(cancellationToken).ConfigureAwait(false);

        return assignedCount;
    }

    /// <inheritdoc />
    public async Task<IReadOnlyList<CategorySuggestion>> SuggestNewCategoriesAsync(int maxSuggestions = 10,
                                                                                   CancellationToken cancellationToken = default)
    {
        var suggestions = new List<CategorySuggestion>();

        // Récupérer les articles non classifiés ou avec faible confiance
        var unclassifiedArticles = await this.context.Articles
                                             .Where(a => !a.ArticleCategories.Any() ||
                                                         a.ArticleCategories.All(ac => ac.ConfidenceScore < 0.6))
                                             .Take(100) // Limiter pour les performances
                                             .ToListAsync(cancellationToken).ConfigureAwait(false);

        if (unclassifiedArticles.Count == 0)
            return suggestions;

        // Analyser les tags et mots-clés communs
        var tagFrequency = new Dictionary<string, int>();
        var keywordFrequency = new Dictionary<string, int>();

        foreach (var article in unclassifiedArticles)
        {
            // Analyser les tags
            foreach (var tag in article.Tags)
                tagFrequency[tag] = tagFrequency.GetValueOrDefault(tag, 0) + 1;

            // Analyser les mots-clés du titre et du résumé
            var keywords = ExtractKeywords(article.Title + " " + article.Summary);
            foreach (var keyword in keywords)
                keywordFrequency[keyword] = keywordFrequency.GetValueOrDefault(keyword, 0) + 1;
        }

        // Créer des suggestions basées sur les tags fréquents
        var frequentTags = tagFrequency
                          .Where(kv => kv.Value >= 3) // Au moins 3 articles
                          .OrderByDescending(kv => kv.Value)
                          .Take(maxSuggestions);

        foreach (var tagEntry in frequentTags)
        {
            var relatedArticles = unclassifiedArticles
                                 .Where(a => a.Tags.Contains(tagEntry.Key))
                                 .Take(5)
                                 .ToArray();

            suggestions.Add(new()
            {
                SuggestedName = CapitalizeTag(tagEntry.Key),
                SuggestedDescription = $"Catégorie pour les articles liés à {tagEntry.Key}",
                SuggestedKeywords = [tagEntry.Key],
                RelevanceScore = Math.Min(1.0, tagEntry.Value / 10.0),
                PotentialArticleCount = tagEntry.Value,
                ExampleArticleIds = [.. relatedArticles.Select(a => a.Id)],
                Reason = $"Tag '{tagEntry.Key}' trouvé dans {tagEntry.Value} articles non classifiés",
            });
        }

        return [.. suggestions.OrderByDescending(s => s.RelevanceScore)];
    }

    /// <inheritdoc />
    public async Task<int> UpdateCategoryEmbeddingsAsync(CancellationToken cancellationToken = default)
    {
        if (!this.embeddingService.IsEnabled)
        {
            this.logger.LogWarning("Embedding service is not enabled, cannot update category embeddings");
            return 0;
        }

        var categories = await this.context.Categories
                                   .Where(c => c.IsActive)
                                   .ToListAsync(cancellationToken).ConfigureAwait(false);

        var updatedCount = 0;

        foreach (var category in categories)
        {
            // Créer le texte représentatif de la catégorie
            var categoryText = BuildCategoryText(category);

            // Générer l'embedding
            var embedding = await this.embeddingService.GetEmbeddingAsync(categoryText, cancellationToken).ConfigureAwait(false);

            if (embedding != null)
            {
                category.Embedding = embedding;
                category.UpdatedAt = DateTime.UtcNow;
                updatedCount++;

                this.logger.LogInformation("Updated embedding for category {CategoryName}", category.Name);
            }
        }

        if (updatedCount > 0)
            _ = await this.context.SaveChangesAsync(cancellationToken).ConfigureAwait(false);

        return updatedCount;
    }

    /// <inheritdoc />
    public async Task<ClassificationQualityReport> EvaluateClassificationQualityAsync(CancellationToken cancellationToken = default)
    {
        var report = new ClassificationQualityReport
        {
            // Statistiques générales
            TotalArticles = await this.context.Articles.CountAsync(cancellationToken).ConfigureAwait(false),
            AutoClassifiedArticles = await this.context.ArticleCategories.CountAsync(ac => !ac.IsManual, cancellationToken).ConfigureAwait(false),
            ManuallyClassifiedArticles = await this.context.ArticleCategories.CountAsync(ac => ac.IsManual, cancellationToken).ConfigureAwait(false),
        };

        // Score moyen de confiance
        if (report.AutoClassifiedArticles > 0)
        {
            report.AverageConfidenceScore = await this.context.ArticleCategories
                                                      .Where(ac => !ac.IsManual)
                                                      .AverageAsync(ac => ac.ConfidenceScore, cancellationToken).ConfigureAwait(false) ?? 0;
        }

        // Top catégories
        report.TopCategories = await this.context.ArticleCategories
                                         .Include(ac => ac.Category)
                                         .GroupBy(ac => ac.Category!.Name)
                                         .ToDictionaryAsync(g => g.Key, g => g.Count(), cancellationToken).ConfigureAwait(false);

        // Articles suggérés pour révision (faible confiance)
        report.ArticlesSuggestedForReview = await this.context.ArticleCategories
                                                      .Where(ac => !ac.IsManual && ac.ConfidenceScore < 0.7)
                                                      .Select(ac => ac.ArticleId)
                                                      .Distinct()
                                                      .ToArrayAsync(cancellationToken).ConfigureAwait(false);

        // Suggestions d'amélioration
        var improvementSuggestions = new List<string>();

        if (report.AverageConfidenceScore < 0.8)
            improvementSuggestions.Add("Considérer l'ajustement des seuils de confiance pour améliorer la précision");

        if (report.ArticlesSuggestedForReview.Length > report.AutoClassifiedArticles * 0.2)
            improvementSuggestions.Add("Trop d'articles avec une confiance faible - révision des critères de classification recommandée");

        var unclassifiedCount = report.TotalArticles -
                                await this.context.Articles.CountAsync(a => a.ArticleCategories.Any(), cancellationToken).ConfigureAwait(false);

        if (unclassifiedCount > report.TotalArticles * 0.3)
            improvementSuggestions.Add("Nombreux articles non classifiés - création de nouvelles catégories recommandée");

        report.ImprovementSuggestions = [.. improvementSuggestions];

        return report;
    }

    /// <summary>
    ///     Calcule le score de correspondance des mots-clés.
    /// </summary>
    private static double CalculateKeywordMatchScore(Article article, Category category)
    {
        if (category.Keywords.Length == 0)
            return 0;

        var articleText = $"{article.Title} {article.Summary} {string.Join(" ", article.Tags)}".ToLowerInvariant();
        var matchedKeywords = category.Keywords.Count(keyword => articleText.Contains(keyword, StringComparison.InvariantCultureIgnoreCase));

        return (double)matchedKeywords / category.Keywords.Length;
    }

    /// <summary>
    ///     Combine deux embeddings en calculant leur moyenne pondérée.
    /// </summary>
    private static float[] CombineEmbeddings(float[] embeddingHead, float[] embeddingBody)
    {
        var combined = new float[embeddingHead.Length];
        for (var i = 0; i < embeddingHead.Length; i++)
            combined[i] = embeddingHead[i] * 0.3f + embeddingBody[i] * 0.7f; // Plus de poids au body

        return combined;
    }

    /// <summary>
    ///     Calcule la similarité cosinus entre deux vecteurs.
    /// </summary>
    private static double CalculateCosineSimilarity(float[] vectorA, float[] vectorB)
    {
        if (vectorA.Length != vectorB.Length)
            return 0;

        double dotProduct = 0;
        double normA = 0;
        double normB = 0;

        for (var i = 0; i < vectorA.Length; i++)
        {
            dotProduct += vectorA[i] * vectorB[i];
            normA += vectorA[i] * vectorA[i];
            normB += vectorB[i] * vectorB[i];
        }

        if (normA == 0 || normB == 0)
            return 0;

        return dotProduct / (Math.Sqrt(normA) * Math.Sqrt(normB));
    }

    /// <summary>
    ///     Construit le texte représentatif d'une catégorie pour générer son embedding.
    /// </summary>
    private static string BuildCategoryText(Category category)
    {
        var parts = new List<string> { category.Name };

        if (!string.IsNullOrEmpty(category.Description))
            parts.Add(category.Description);

        if (category.Keywords.Length != 0)
            parts.Add(string.Join(" ", category.Keywords));

        return string.Join(" ", parts);
    }

    /// <summary>
    ///     Extrait les mots-clés d'un texte.
    /// </summary>
    private static string[] ExtractKeywords(string text)
    {
        if (string.IsNullOrWhiteSpace(text))
            return [];

        var stopWords = new HashSet<string> { "le", "la", "les", "un", "une", "des", "de", "du", "et", "ou", "pour", "avec", "dans", "sur", "par", "à", "au", "aux" };

        return
        [
            .. text
              .ToLowerInvariant()
              .Split([' ', '.', ',', ';', ':', '!', '?', '\n', '\r', '\t'], StringSplitOptions.RemoveEmptyEntries)
              .Where(word => word.Length > 3 && !stopWords.Contains(word))
              .Distinct(),
        ];
    }

    /// <summary>
    ///     Met en forme un tag pour qu'il ressemble à un nom de catégorie.
    /// </summary>
    private static string CapitalizeTag(string tag)
    {
        if (string.IsNullOrEmpty(tag))
            return tag;

        return char.ToUpperInvariant(tag[0]) + tag[1..].ToLowerInvariant();
    }

    /// <summary>
    ///     Calcule le score de classification pour un article et une catégorie.
    /// </summary>
    private async Task<(double Score, string Reason)> CalculateClassificationScore(Article article,
                                                                                   Category category,
                                                                                   ClassificationOptions options,
                                                                                   CancellationToken cancellationToken)
    {
        var reasons = new List<string>();
        double totalScore = 0;

        // 1. Score basé sur les mots-clés
        var keywordScore = CalculateKeywordMatchScore(article, category);

        if (keywordScore > 0)
        {
            totalScore += keywordScore * options.KeywordMatchWeight;
            reasons.Add($"Correspondance mots-clés: {keywordScore:F2}");
        }

        // 2. Score basé sur la similarité sémantique des embeddings
        var semanticScore = this.CalculateSemanticSimilarityScore(article, category);

        if (semanticScore > 0)
        {
            totalScore += semanticScore * options.SemanticSimilarityWeight;
            reasons.Add($"Similarité sémantique: {semanticScore:F2}");
        }

        // 3. Bonus pour les catégories avec historique de classification manuelle similaire
        var historicalScore = await this.CalculateHistoricalScore(article, category, cancellationToken).ConfigureAwait(false);

        if (historicalScore > 0)
        {
            totalScore += historicalScore * 0.1; // Faible poids
            reasons.Add($"Historique similaire: {historicalScore:F2}");
        }

        var reason = reasons.Count != 0 ? string.Join(", ", reasons) : "Aucune correspondance trouvée";
        return (Math.Min(1.0, totalScore), reason);
    }

    /// <summary>
    ///     Calcule la similarité sémantique entre l'article et la catégorie.
    /// </summary>
    private double CalculateSemanticSimilarityScore(Article article, Category category)
    {
        if (category.Embedding == null || !this.embeddingService.IsEnabled)
            return 0;

        // Utiliser l'embedding existant de l'article (combinaison head + body)
        var articleEmbedding = CombineEmbeddings(article.EmbeddingHead, article.EmbeddingBody);

        // Calculer la similarité cosinus
        return CalculateCosineSimilarity(articleEmbedding, category.Embedding);
    }

    /// <summary>
    ///     Calcule un score basé sur l'historique de classification.
    /// </summary>
    private async Task<double> CalculateHistoricalScore(Article article,
                                                        Category category,
                                                        CancellationToken cancellationToken)
    {
        // Rechercher des articles similaires déjà classifiés dans cette catégorie
        var similarClassifications = await this.context.ArticleCategories
                                               .Include(ac => ac.Article)
                                               .Where(ac => ac.CategoryId == category.Id && ac.IsManual)
                                               .Where(ac => ac.Article.Tags.Any(tag => article.Tags.Contains(tag)))
                                               .CountAsync(cancellationToken).ConfigureAwait(false);

        if (similarClassifications == 0)
            return 0;

        // Score basé sur le nombre d'articles similaires déjà classifiés
        return Math.Min(1.0, similarClassifications / 10.0);
    }
}
