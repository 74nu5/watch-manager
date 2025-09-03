namespace Watch.Manager.ApiService.Extensions;

using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Watch.Manager.ApiService.ViewModels;
using Watch.Manager.Service.Analyse.Models;
using Watch.Manager.Service.Analyse.Services;
using Watch.Manager.Service.Database.Abstractions;
using Watch.Manager.Service.Database.Context;

/// <summary>
/// Extensions pour ajouter les endpoints de classification des articles.
/// </summary>
public static class ClassificationEndpoints
{
    /// <summary>
    /// Ajoute les endpoints de classification des articles à l'application.
    /// </summary>
    /// <param name="app">L'application web.</param>
    /// <returns>L'application web pour le chaînage.</returns>
    public static WebApplication MapClassificationEndpoints(this WebApplication app)
    {
        var classificationGroup = app.MapGroup("/api/classification")
            .WithTags("Classification")
            .WithOpenApi();

        // Classification automatique d'un article spécifique
        _ = classificationGroup.MapPost("/articles/{id:int}/classify", async (
            [FromRoute] int id,
            [FromBody] ClassificationOptions? options,
            [FromServices] IArticleClassificationAI classificationService,
            [FromServices] ICategoryStore categoryStore,
            [FromServices] ArticlesContext context,
            CancellationToken cancellationToken) =>
        {
            // Récupérer l'article
            var article = await context.Articles
                .FirstOrDefaultAsync(a => a.Id == id, cancellationToken)
                .ConfigureAwait(false);

            if (article == null)
            {
                return Results.NotFound($"Article {id} not found");
            }

            // Récupérer les catégories disponibles
            var categories = await categoryStore.GetAllCategoriesAsync(false, cancellationToken).ConfigureAwait(false);
            var categoryForClassification = categories
                .Where(c => c.IsActive)
                .Select(c => new CategoryForClassification
                {
                    Id = c.Id,
                    Name = c.Name,
                    Description = c.Description,
                    Keywords = c.Keywords ?? [],
                    AutoThreshold = options?.MinAutoClassificationScore ?? 0.7,
                    ManualThreshold = options?.MinSuggestionScore ?? 0.5,
                    IsActive = c.IsActive
                });

            // Classification de l'article
            var articleContent = $"{article.Title}\n\n{article.Summary}";
            var suggestions = await classificationService.ClassifyArticleAsync(articleContent, categoryForClassification, cancellationToken).ConfigureAwait(false);

            return Results.Ok(suggestions);
        })
        .WithName("ClassifyArticle")
        .WithSummary("Classifie automatiquement un article")
        .WithDescription("Analyse un article et suggère des catégories appropriées en utilisant l'IA");

        // Auto-assignation d'un article à une catégorie
        _ = classificationGroup.MapPost("/articles/{id:int}/auto-assign", async (
            [FromRoute] int id,
            [FromBody] ClassificationOptions? options,
            [FromServices] IArticleClassificationAI classificationService,
            [FromServices] ICategoryStore categoryStore,
            [FromServices] ArticlesContext context,
            CancellationToken cancellationToken) =>
        {
            // Récupérer l'article
            var article = await context.Articles
                .FirstOrDefaultAsync(a => a.Id == id, cancellationToken)
                .ConfigureAwait(false);

            if (article == null)
            {
                return Results.NotFound($"Article {id} not found");
            }

            // Récupérer les catégories disponibles
            var categories = await categoryStore.GetAllCategoriesAsync(false, cancellationToken).ConfigureAwait(false);
            var categoryForClassification = categories
                .Where(c => c.IsActive)
                .Select(c => new CategoryForClassification
                {
                    Id = c.Id,
                    Name = c.Name,
                    Description = c.Description,
                    Keywords = c.Keywords ?? [],
                    AutoThreshold = options?.MinAutoClassificationScore ?? 0.7,
                    ManualThreshold = options?.MinSuggestionScore ?? 0.5,
                    IsActive = c.IsActive
                });

            // Classification de l'article
            var articleContent = $"{article.Title}\n\n{article.Summary}";
            var suggestions = await classificationService.ClassifyArticleAsync(articleContent, categoryForClassification, cancellationToken).ConfigureAwait(false);

            // Auto-assigner les catégories qui dépassent le seuil automatique
            var autoAssignedCategories = suggestions
                .Where(s => s.ExceedsAutoThreshold)
                .ToList();

            foreach (var suggestion in autoAssignedCategories)
            {
                _ = await categoryStore.AssignCategoryToArticleAsync(id, suggestion.CategoryId, false, suggestion.ConfidenceScore, cancellationToken).ConfigureAwait(false);
            }

            return Results.Ok(new { AutoAssignedCategories = autoAssignedCategories.Count, Suggestions = suggestions });
        })
        .WithName("AutoAssignArticle")
        .WithSummary("Auto-assigne un article aux catégories appropriées")
        .WithDescription("Classifie un article et l'assigne automatiquement aux catégories qui dépassent le seuil de confiance");

        // Classification en lot d'articles
        _ = classificationGroup.MapPost("/articles/batch-classify", async (
            [FromBody] BatchClassificationRequest request,
            [FromServices] IArticleClassificationAI classificationService,
            [FromServices] ICategoryStore categoryStore,
            [FromServices] ArticlesContext context,
            CancellationToken cancellationToken) =>
        {
            // Récupérer les catégories disponibles
            var categories = await categoryStore.GetAllCategoriesAsync(false, cancellationToken).ConfigureAwait(false);
            var categoryForClassification = categories
                .Where(c => c.IsActive)
                .Select(c => new CategoryForClassification
                {
                    Id = c.Id,
                    Name = c.Name,
                    Description = c.Description,
                    Keywords = c.Keywords ?? [],
                    AutoThreshold = request.Options?.MinAutoClassificationScore ?? 0.7,
                    ManualThreshold = request.Options?.MinSuggestionScore ?? 0.5,
                    IsActive = c.IsActive
                });

            var results = new List<ArticleClassificationResult>();

            foreach (var articleId in request.ArticleIds)
            {
                var article = await context.Articles
                    .FirstOrDefaultAsync(a => a.Id == articleId, cancellationToken)
                    .ConfigureAwait(false);

                if (article != null)
                {
                    var articleContent = $"{article.Title}\n\n{article.Summary}";
                    var suggestions = await classificationService.ClassifyArticleAsync(articleContent, categoryForClassification, cancellationToken).ConfigureAwait(false);

                    results.Add(new ArticleClassificationResult
                    {
                        ArticleId = articleId,
                        CategorySuggestions = suggestions,
                        NewCategorySuggestions = [],
                        OverallConfidence = suggestions.MaxBy(s => s.ConfidenceScore)?.ConfidenceScore ?? 0
                    });
                }
            }

            return Results.Ok(results);
        })
        .WithName("BatchClassifyArticles")
        .WithSummary("Classifie plusieurs articles en lot")
        .WithDescription("Analyse plusieurs articles et suggère des catégories appropriées pour chacun");

        // Suggestions de nouvelles catégories
        _ = classificationGroup.MapGet("/suggestions/new-categories", async (
            [FromQuery] int? limit,
            [FromServices] IArticleClassificationAI classificationService,
            [FromServices] ArticlesContext context,
            CancellationToken cancellationToken) =>
        {
            // Récupérer les articles récents non catégorisés
            var uncategorizedArticles = await context.Articles
                .Where(a => !a.ArticleCategories.Any())
                .OrderByDescending(a => a.AnalyzeDate)
                .Take(limit ?? 50)
                .ToListAsync(cancellationToken)
                .ConfigureAwait(false);

            // Récupérer les noms de catégories existantes
            var existingCategoryNames = await context.Categories
                .Where(c => c.IsActive)
                .Select(c => c.Name)
                .ToListAsync(cancellationToken)
                .ConfigureAwait(false);

            var allSuggestions = new List<Service.Analyse.Models.NewCategorySuggestion>();

            // Traiter chaque article
            foreach (var article in uncategorizedArticles)
            {
                var articleContent = $"{article.Title}\n\n{article.Summary}";
                var suggestions = await classificationService.SuggestNewCategoriesAsync(articleContent, existingCategoryNames, cancellationToken).ConfigureAwait(false);
                allSuggestions.AddRange(suggestions);
            }

            // Grouper et consolider les suggestions
            var consolidatedSuggestions = allSuggestions
                .GroupBy(s => s.SuggestedName.ToLowerInvariant())
                .Select(g => new ViewModels.NewCategorySuggestion
                {
                    SuggestedName = g.First().SuggestedName,
                    SuggestedDescription = g.First().SuggestedDescription,
                    SuggestedKeywords = g.SelectMany(s => s.SuggestedKeywords).Distinct().ToArray(),
                    RelevanceScore = g.Average(s => s.RelevanceScore),
                    Justification = string.Join("; ", g.Select(s => s.Justification).Where(j => !string.IsNullOrEmpty(j)))
                })
                .OrderByDescending(s => s.RelevanceScore)
                .Take(20)
                .ToList();

            return Results.Ok(consolidatedSuggestions);
        })
        .WithName("GetNewCategorySuggestions")
        .WithSummary("Obtient des suggestions de nouvelles catégories")
        .WithDescription("Analyse les articles non catégorisés et suggère de nouvelles catégories à créer");

        // Feedback sur la classification
        _ = classificationGroup.MapPost("/feedback", async (
            [FromBody] ClassificationFeedbackRequest request,
            [FromServices] IArticleClassificationAI classificationService,
            [FromServices] ArticlesContext context,
            CancellationToken cancellationToken) =>
        {
            // Récupérer le contenu de l'article
            var article = await context.Articles
                .FirstOrDefaultAsync(a => a.Id == request.ArticleId, cancellationToken)
                .ConfigureAwait(false);

            if (article == null)
            {
                return Results.NotFound($"Article {request.ArticleId} not found");
            }

            var articleContent = $"{article.Title}\n\n{article.Summary}";

            await classificationService.LearnFromFeedbackAsync(
                articleContent,
                request.CorrectCategories,
                request.IncorrectCategories,
                cancellationToken).ConfigureAwait(false);

            return Results.Ok(new { Message = "Feedback enregistré avec succès" });
        })
        .WithName("ProvideClassificationFeedback")
        .WithSummary("Fournit un retour sur la classification")
        .WithDescription("Permet d'améliorer l'IA en fournissant des retours sur la précision des classifications");

        // Endpoint simplifié pour les suggestions de nouvelles catégories (pour compatibilité avec AnalyzeService)
        _ = classificationGroup.MapGet("/suggestions", async (
            [FromQuery] int? limit,
            [FromServices] IArticleClassificationAI classificationService,
            [FromServices] ArticlesContext context,
            CancellationToken cancellationToken) =>
        {
            // Récupérer les articles récents non catégorisés
            var uncategorizedArticles = await context.Articles
                .Where(a => !a.ArticleCategories.Any())
                .OrderByDescending(a => a.AnalyzeDate)
                .Take(limit ?? 50)
                .ToListAsync(cancellationToken)
                .ConfigureAwait(false);

            // Récupérer les noms de catégories existantes
            var existingCategoryNames = await context.Categories
                .Where(c => c.IsActive)
                .Select(c => c.Name)
                .ToListAsync(cancellationToken)
                .ConfigureAwait(false);

            var allSuggestions = new List<ViewModels.NewCategorySuggestion>();

            // Traiter chaque article
            foreach (var article in uncategorizedArticles)
            {
                var articleContent = $"{article.Title}\n\n{article.Summary}";
                var suggestions = await classificationService.SuggestNewCategoriesAsync(articleContent, existingCategoryNames, cancellationToken).ConfigureAwait(false);

                // Convertir les suggestions du service vers le modèle attendu par l'API
                foreach (var suggestion in suggestions)
                {
                    allSuggestions.Add(new ViewModels.NewCategorySuggestion
                    {
                        SuggestedName = suggestion.SuggestedName,
                        SuggestedDescription = suggestion.SuggestedDescription,
                        SuggestedKeywords = suggestion.SuggestedKeywords,
                        RelevanceScore = suggestion.RelevanceScore,
                        Justification = suggestion.Justification,
                        SuggestedColor = null, // Pas disponible dans le modèle du service
                        SuggestedIcon = null, // Pas disponible dans le modèle du service
                        SuggestedParentId = null, // Pas disponible dans le modèle du service
                        SuggestedParentName = null // Pas disponible dans le modèle du service
                    });
                }
            }

            // Grouper et consolider les suggestions
            var consolidatedSuggestions = allSuggestions
                .GroupBy(s => s.SuggestedName.ToLowerInvariant())
                .Select(g => new ViewModels.NewCategorySuggestion
                {
                    SuggestedName = g.First().SuggestedName,
                    SuggestedDescription = g.First().SuggestedDescription,
                    SuggestedKeywords = g.SelectMany(s => s.SuggestedKeywords).Distinct().ToArray(),
                    RelevanceScore = g.Average(s => s.RelevanceScore),
                    Justification = string.Join("; ", g.Select(s => s.Justification).Where(j => !string.IsNullOrEmpty(j))),
                    SuggestedColor = null,
                    SuggestedIcon = null,
                    SuggestedParentId = null,
                    SuggestedParentName = null
                })
                .OrderByDescending(s => s.RelevanceScore)
                .Take(20)
                .ToArray();

            return Results.Ok(consolidatedSuggestions);
        })
        .WithName("GetSuggestions")
        .WithSummary("Obtient des suggestions de nouvelles catégories (endpoint simplifié)")
        .WithDescription("Endpoint simplifié pour obtenir des suggestions de nouvelles catégories");

        // Endpoint simplifié pour la classification par lot (pour compatibilité avec AnalyzeService)
        _ = classificationGroup.MapPost("/batch", async (
            [FromServices] IArticleClassificationAI classificationService,
            [FromServices] ICategoryStore categoryStore,
            [FromServices] ArticlesContext context,
            CancellationToken cancellationToken) =>
        {
            // Récupérer tous les articles non catégorisés
            var uncategorizedArticles = await context.Articles
                .Where(a => !a.ArticleCategories.Any())
                .OrderByDescending(a => a.AnalyzeDate)
                .Take(100) // Limiter à 100 articles pour éviter des temps de traitement trop longs
                .ToListAsync(cancellationToken)
                .ConfigureAwait(false);

            // Récupérer les catégories disponibles
            var categories = await categoryStore.GetAllCategoriesAsync(false, cancellationToken).ConfigureAwait(false);
            var categoryForClassification = categories
                .Where(c => c.IsActive)
                .Select(c => new CategoryForClassification
                {
                    Id = c.Id,
                    Name = c.Name,
                    Description = c.Description,
                    Keywords = c.Keywords ?? [],
                    AutoThreshold = 0.7,
                    ManualThreshold = 0.5,
                    IsActive = c.IsActive
                });

            var results = new List<ViewModels.BatchClassificationResult>();

            foreach (var article in uncategorizedArticles)
            {
                try
                {
                    var articleContent = $"{article.Title}\n\n{article.Summary}";
                    var suggestions = await classificationService.ClassifyArticleAsync(articleContent, categoryForClassification, cancellationToken).ConfigureAwait(false);

                    // Auto-assigner les catégories qui dépassent le seuil automatique
                    var autoAssigned = new List<ViewModels.CategoryAssignmentModel>();
                    foreach (var suggestion in suggestions.Where(s => s.ExceedsAutoThreshold))
                    {
                        _ = await categoryStore.AssignCategoryToArticleAsync(article.Id, suggestion.CategoryId, false, suggestion.ConfidenceScore, cancellationToken).ConfigureAwait(false);
                        autoAssigned.Add(new ViewModels.CategoryAssignmentModel
                        {
                            CategoryId = suggestion.CategoryId,
                            CategoryName = categories.First(c => c.Id == suggestion.CategoryId).Name,
                            ConfidenceScore = suggestion.ConfidenceScore,
                            IsAutomatic = true
                        });
                    }

                    results.Add(new ViewModels.BatchClassificationResult
                    {
                        ArticleId = article.Id,
                        ArticleTitle = article.Title,
                        AssignedCategories = autoAssigned.ToArray(),
                        Success = true
                    });
                }
                catch (Exception ex)
                {
                    results.Add(new ViewModels.BatchClassificationResult
                    {
                        ArticleId = article.Id,
                        ArticleTitle = article.Title,
                        AssignedCategories = [],
                        Success = false,
                        ErrorMessage = ex.Message
                    });
                }
            }

            return Results.Ok(results.ToArray());
        })
        .WithName("BatchClassify")
        .WithSummary("Classifie automatiquement tous les articles non catégorisés par lot")
        .WithDescription("Endpoint simplifié pour classifier automatiquement tous les articles non catégorisés");

        return app;
    }
}
