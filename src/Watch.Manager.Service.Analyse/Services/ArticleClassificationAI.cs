namespace Watch.Manager.Service.Analyse.Services;

using System.Diagnostics;
using System.Text;
using System.Text.Json;

using Microsoft.Extensions.AI;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;

using Watch.Manager.Service.Analyse.Models;

/// <summary>
///     Service de classification automatique d'articles par IA.
/// </summary>
internal sealed class ArticleClassificationAI : IArticleClassificationAI
{
    private const int EmbeddingDimensions = 1536;
    private const double DefaultSimilarityThreshold = 0.7;

    private readonly IChatClient? chatClient;
    private readonly IEmbeddingGenerator<string, Embedding<float>>? embeddingGenerator;
    private readonly ILogger<ArticleClassificationAI> logger;
    private readonly ChatOptions chatOptions;

    public ArticleClassificationAI(ILogger<ArticleClassificationAI> logger, IServiceProvider serviceProvider)
    {
        this.logger = logger;
        this.chatClient = serviceProvider.GetService<IChatClient>();
        this.embeddingGenerator = serviceProvider.GetService<IEmbeddingGenerator<string, Embedding<float>>>();

        this.chatOptions = new()
        {
            ResponseFormat = ChatResponseFormat.Json,
            Temperature = 0.3f, // Plus déterministe pour la classification
        };
    }

    /// <inheritdoc />
    public bool IsEnabled => this.chatClient is not null && this.embeddingGenerator is not null;

    /// <inheritdoc />
    public async Task<IEnumerable<CategorySuggestionResult>> ClassifyArticleAsync(string articleContent,
                                                                                  IEnumerable<CategoryForClassification> availableCategories,
                                                                                  CancellationToken cancellationToken = default)
    {
        if (!this.IsEnabled)
        {
            this.logger.LogWarning("Classification IA non disponible - services IA non configurés");
            return [];
        }

        var categoriesList = availableCategories.ToArray();

        if (categoriesList.Length == 0)
        {
            this.logger.LogInformation("Aucune catégorie disponible pour la classification");
            return [];
        }

        var stopwatch = Stopwatch.StartNew();
        var suggestions = new List<CategorySuggestionResult>();

        try
        {
            // 1. Classification par similarité sémantique (embeddings)
            var semanticSuggestions = await this.ClassifyBySemanticSimilarityAsync(articleContent, categoriesList, cancellationToken).ConfigureAwait(false);
            suggestions.AddRange(semanticSuggestions);

            // 2. Classification par mots-clés
            var keywordSuggestions = ClassifyByKeywords(articleContent, categoriesList);
            MergeSuggestions(suggestions, keywordSuggestions);

            // 3. Classification par IA contextuelle
            var aiSuggestions = await this.ClassifyByAIAsync(articleContent, categoriesList, cancellationToken).ConfigureAwait(false);
            MergeSuggestions(suggestions, aiSuggestions);

            // 4. Normaliser et trier les résultats
            CategorySuggestionResult[] finalSuggestions = [.. NormalizeAndRankSuggestions(suggestions, categoriesList)];

            this.logger.LogInformation("Classification terminée en {ElapsedMs}ms - {SuggestionsCount} suggestions générées",
                stopwatch.ElapsedMilliseconds,
                finalSuggestions.Length);

            return finalSuggestions;
        }
        catch (Exception ex)
        {
            this.logger.LogError(ex, "Erreur lors de la classification de l'article");
            return [];
        }
    }

    /// <inheritdoc />
    public async Task<IEnumerable<NewCategorySuggestion>> SuggestNewCategoriesAsync(string articleContent,
                                                                                    IEnumerable<string> existingCategories,
                                                                                    CancellationToken cancellationToken = default)
    {
        if (!this.IsEnabled)
            return [];

        try
        {
            var existingCategoriesList = existingCategories.ToList();
            var systemPrompt = BuildNewCategorySystemPrompt(existingCategoriesList);

            var messages = new List<ChatMessage>
            {
                new(ChatRole.System, systemPrompt),
                new(ChatRole.User, $"Analyse ce contenu d'article et suggère de nouvelles catégories si nécessaire :\n\n{articleContent}"),
            };

            var response = await this.chatClient!.GetResponseAsync(messages, this.chatOptions, cancellationToken).ConfigureAwait(false);
            var responseText = response.Messages.FirstOrDefault()?.Text;

            if (string.IsNullOrEmpty(responseText))
                return [];

            List<NewCategorySuggestion> suggestions;

            try
            {
                // Essayer de désérialiser comme un tableau
                suggestions = JsonSerializer.Deserialize<List<NewCategorySuggestion>>(responseText) ?? [];
            }
            catch (JsonException)
            {
                try
                {
                    // Si cela échoue, essayer de désérialiser comme un objet unique
                    var singleSuggestion = JsonSerializer.Deserialize<NewCategorySuggestion>(responseText);
                    suggestions = singleSuggestion != null ? [singleSuggestion] : [];
                }
                catch (JsonException ex)
                {
                    this.logger.LogWarning(ex, "Impossible de désérialiser la réponse IA pour les nouvelles catégories: {Response}", responseText);
                    return [];
                }
            }

            this.logger.LogInformation("Génération de {Count} nouvelles suggestions de catégories", suggestions.Count);

            return suggestions;
        }
        catch (Exception ex)
        {
            this.logger.LogError(ex, "Erreur lors de la suggestion de nouvelles catégories");
            return [];
        }
    }

    /// <inheritdoc />
    public double CalculateSemanticSimilarity(float[] articleEmbedding, float[] categoryEmbedding)
    {
        if (articleEmbedding.Length != categoryEmbedding.Length)
            throw new ArgumentException("Les embeddings doivent avoir la même dimension");

        // Calcul de la similarité cosinus
        var dotProduct = articleEmbedding.Zip(categoryEmbedding, (a, b) => a * b).Sum();
        var magnitudeA = Math.Sqrt(articleEmbedding.Select(x => x * x).Sum());
        var magnitudeB = Math.Sqrt(categoryEmbedding.Select(x => x * x).Sum());

        if (magnitudeA == 0 || magnitudeB == 0)
            return 0;

        var similarity = dotProduct / (magnitudeA * magnitudeB);

        // Normaliser entre 0 et 1
        return Math.Max(0, Math.Min(1, (similarity + 1) / 2));
    }

    /// <inheritdoc />
    public async Task LearnFromFeedbackAsync(string articleContent,
                                             IEnumerable<int> correctCategories,
                                             IEnumerable<int> incorrectCategories,
                                             CancellationToken cancellationToken = default)
    {
        // Implémentation future pour l'apprentissage supervisé
        // Pour l'instant, on log les informations pour analysis future

        var correctList = correctCategories.ToList();
        var incorrectList = incorrectCategories.ToList();

        this.logger.LogInformation("Feedback d'apprentissage reçu - Correctes: {CorrectCount}, Incorrectes: {IncorrectCount}",
            correctList.Count,
            incorrectList.Count);

        // TODO: Implémenter la logique d'apprentissage supervisé
        // - Stocker les feedbacks dans une base de données
        // - Ajuster les poids des modèles
        // - Réentraîner périodiquement

        await Task.CompletedTask.ConfigureAwait(false);
    }

    private static List<CategorySuggestionResult> ClassifyByKeywords(string articleContent, IEnumerable<CategoryForClassification> categories)
    {
        var suggestions = new List<CategorySuggestionResult>();
        var contentLower = articleContent.ToLowerInvariant();

        foreach (var category in categories.Where(c => c.Keywords.Length > 0))
        {
            var matchingKeywords = category.Keywords
                                           .Where(keyword => contentLower.Contains(keyword, StringComparison.InvariantCultureIgnoreCase))
                                           .ToList();

            if (matchingKeywords.Count == 0)
                continue;

            var keywordScore = (double)matchingKeywords.Count / category.Keywords.Length;
            var confidence = Math.Min(0.9, keywordScore); // Limiter à 90% pour les mots-clés

            suggestions.Add(new()
            {
                CategoryId = category.Id,
                CategoryName = category.Name,
                ConfidenceScore = confidence,
                Reason = $"Mots-clés correspondants: {string.Join(", ", matchingKeywords)}",
                ExceedsAutoThreshold = confidence >= category.AutoThreshold,
                ExceedsManualThreshold = confidence >= category.ManualThreshold,
            });
        }

        return suggestions;
    }

    private static void MergeSuggestions(List<CategorySuggestionResult> existingSuggestions, IEnumerable<CategorySuggestionResult> newSuggestions)
    {
        foreach (var newSuggestion in newSuggestions)
        {
            var existingSuggestion = existingSuggestions.FirstOrDefault(s => s.CategoryId == newSuggestion.CategoryId);

            if (existingSuggestion == null)
            {
                existingSuggestions.Add(newSuggestion);
                continue;
            }

            // Combiner les scores (moyenne pondérée)
            existingSuggestion.ConfidenceScore = (existingSuggestion.ConfidenceScore + newSuggestion.ConfidenceScore) / 2;
            existingSuggestion.Reason = $"{existingSuggestion.Reason}; {newSuggestion.Reason}";
        }
    }

    private async Task<IEnumerable<CategorySuggestionResult>> ClassifyBySemanticSimilarityAsync(string articleContent, CategoryForClassification[] categories, CancellationToken cancellationToken)
    {
        var suggestions = new List<CategorySuggestionResult>();

        if (this.embeddingGenerator == null)
            return suggestions;

        try
        {
            // Générer l'embedding de l'article
            var articleEmbedding = await this.embeddingGenerator.GenerateVectorAsync(articleContent, cancellationToken: cancellationToken).ConfigureAwait(false);
            var articleVector = articleEmbedding[..EmbeddingDimensions].ToArray();

            foreach (var category in categories.Where(c => c.Embedding != null))
            {
                var similarity = this.CalculateSemanticSimilarity(articleVector, category.Embedding!);

                if (similarity >= DefaultSimilarityThreshold)
                {
                    suggestions.Add(new()
                    {
                        CategoryId = category.Id,
                        CategoryName = category.Name,
                        ConfidenceScore = similarity,
                        Reason = $"Similarité sémantique: {similarity:P1}",
                        ExceedsAutoThreshold = similarity >= category.AutoThreshold,
                        ExceedsManualThreshold = similarity >= category.ManualThreshold,
                    });
                }
            }
        }
        catch (Exception ex)
        {
            this.logger.LogError(ex, "Erreur lors de la classification sémantique");
        }

        return suggestions;
    }

    private async Task<IEnumerable<CategorySuggestionResult>> ClassifyByAIAsync(string articleContent, CategoryForClassification[] categories, CancellationToken cancellationToken)
    {
        if (this.chatClient == null)
            return [];

        try
        {
            var systemPrompt = BuildClassificationSystemPrompt(categories);

            var messages = new List<ChatMessage>
            {
                new(ChatRole.System, systemPrompt),
                new(ChatRole.User, $"Classifie ce contenu d'article :\n\n{articleContent}"),
            };

            var response = await this.chatClient.GetResponseAsync(messages, this.chatOptions, cancellationToken).ConfigureAwait(false);
            var responseText = response.Messages.FirstOrDefault()?.Text;

            if (string.IsNullOrEmpty(responseText))
                return [];

            List<CategorySuggestionResult> aiSuggestions;

            try
            {
                // Essayer de désérialiser comme un tableau
                aiSuggestions = JsonSerializer.Deserialize<List<CategorySuggestionResult>>(responseText) ?? [];
            }
            catch (JsonException)
            {
                try
                {
                    // Si cela échoue, essayer de désérialiser comme un objet unique
                    var singleSuggestion = JsonSerializer.Deserialize<CategorySuggestionResult>(responseText);
                    aiSuggestions = singleSuggestion != null ? [singleSuggestion] : [];
                }
                catch (JsonException ex)
                {
                    this.logger.LogWarning(ex, "Impossible de désérialiser la réponse IA: {Response}", responseText);
                    return [];
                }
            }

            // Valider et ajuster les suggestions IA
            return aiSuggestions.Where(s => categories.Any(c => c.Id == s.CategoryId));
        }
        catch (Exception ex)
        {
            this.logger.LogError(ex, "Erreur lors de la classification par IA");
            return [];
        }
    }

    private static IEnumerable<CategorySuggestionResult> NormalizeAndRankSuggestions(List<CategorySuggestionResult> suggestions,
                                                                                     IEnumerable<CategoryForClassification> categories)
    {
        var categoriesDict = categories.ToDictionary(c => c.Id, c => c);

        return
        [
            .. suggestions
              .Where(s => categoriesDict.ContainsKey(s.CategoryId))
              .Select(s =>
               {
                   var category = categoriesDict[s.CategoryId];
                   s.ExceedsAutoThreshold = s.ConfidenceScore >= category.AutoThreshold;
                   s.ExceedsManualThreshold = s.ConfidenceScore >= category.ManualThreshold;
                   return s;
               })
              .OrderByDescending(s => s.ConfidenceScore)
              .Take(10),
        ];
    }

    private static string BuildClassificationSystemPrompt(IEnumerable<CategoryForClassification> categories)
    {
        var sb = new StringBuilder();
        _ = sb.AppendLine("Tu es un expert en classification d'articles techniques.");
        _ = sb.AppendLine("Ton rôle est de classifier des articles en sélectionnant les catégories les plus pertinentes.");
        _ = sb.AppendLine();
        _ = sb.AppendLine("Catégories disponibles:");

        foreach (var category in categories)
        {
            _ = sb.AppendLine($"- ID: {category.Id}, Nom: {category.Name}");
            if (!string.IsNullOrEmpty(category.Description))
                _ = sb.AppendLine($"  Description: {category.Description}");

            if (category.Keywords.Length > 0)
                _ = sb.AppendLine($"  Mots-clés: {string.Join(", ", category.Keywords)}");
        }

        _ = sb.AppendLine();
        _ = sb.AppendLine("Réponds uniquement en JSON avec ce format:");
        _ = sb.AppendLine("[");
        _ = sb.AppendLine("  {");
        _ = sb.AppendLine("    \"CategoryId\": 1,");
        _ = sb.AppendLine("    \"CategoryName\": \"Nom de la catégorie\",");
        _ = sb.AppendLine("    \"ConfidenceScore\": 0.85,");
        _ = sb.AppendLine("    \"Reason\": \"Raison de la classification\",");
        _ = sb.AppendLine("    \"ExceedsAutoThreshold\": true,");
        _ = sb.AppendLine("    \"ExceedsManualThreshold\": false");
        _ = sb.AppendLine("  }");
        _ = sb.AppendLine("]");

        return sb.ToString();
    }

    private static string BuildNewCategorySystemPrompt(IEnumerable<string> existingCategories)
    {
        var sb = new StringBuilder();
        _ = sb.AppendLine("Tu es un expert en taxonomie et classification d'articles techniques.");
        _ = sb.AppendLine("Ton rôle est d'analyser du contenu et de suggérer de nouvelles catégories si le contenu ne correspond à aucune catégorie existante.");
        _ = sb.AppendLine();
        _ = sb.AppendLine("Catégories existantes à éviter:");
        _ = sb.AppendLine(string.Join(", ", existingCategories));
        _ = sb.AppendLine();
        _ = sb.AppendLine("Suggère uniquement des nouvelles catégories si le contenu contient des concepts non couverts par les catégories existantes.");
        _ = sb.AppendLine("Limite-toi à 3 suggestions maximum et assure-toi qu'elles sont pertinentes et spécifiques.");
        _ = sb.AppendLine();
        _ = sb.AppendLine("Réponds uniquement en JSON avec ce format:");
        _ = sb.AppendLine("[");
        _ = sb.AppendLine("  {");
        _ = sb.AppendLine("    \"SuggestedName\": \"Nom de la nouvelle catégorie\",");
        _ = sb.AppendLine("    \"SuggestedDescription\": \"Description de la catégorie\",");
        _ = sb.AppendLine("    \"SuggestedKeywords\": [\"mot-clé1\", \"mot-clé2\"],");
        _ = sb.AppendLine("    \"RelevanceScore\": 0.75,");
        _ = sb.AppendLine("    \"Justification\": \"Pourquoi cette catégorie est nécessaire\"");
        _ = sb.AppendLine("  }");
        _ = sb.AppendLine("]");

        return sb.ToString();
    }
}
