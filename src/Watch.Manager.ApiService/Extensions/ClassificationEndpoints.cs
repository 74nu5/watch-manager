namespace Watch.Manager.ApiService.Extensions;

using Microsoft.EntityFrameworkCore;

using Watch.Manager.ApiService.Parameters.Classification;
using Watch.Manager.ApiService.ViewModels;
using Watch.Manager.Service.Analyse.Models;
using Watch.Manager.Service.Database.Entities;

using NewCategorySuggestion = Watch.Manager.Service.Analyse.Models.NewCategorySuggestion;

/// <summary>
///     Provides extension methods for configuring endpoints related to article classification.
/// </summary>
/// <remarks>
///     This class defines a set of endpoints for managing article classification operations, including:
///     <list type="bullet">
///         <item>Classifying individual articles.</item>
///         <item>
///             Automatically assigning categories to
///             articles based on classification results.
///         </item>
///         <item>Batch classification of multiple articles.</item>
///         <item>Providing suggestions for new categories based on uncategorized articles.</item>
///         <item>
///             Submitting feedback to
///             improve classification accuracy.
///         </item>
///     </list>
///     These endpoints are added to the application's routing pipeline and
///     are grouped under the "/api/classification" base path.
/// </remarks>
public static class ClassificationEndpoints
{
    /// <summary>
    ///     Configures the application's endpoints for managing article classification, including operations for classifying articles, auto-assigning categories, batch classification, suggestions, and feedback.
    /// </summary>
    /// <param name="app">The <see cref="WebApplication" /> instance to which the classification endpoints will be added.</param>
    public static void MapClassificationEndpoints(this WebApplication app)
    {
        var vApi = app.NewVersionedApi("Classification");

        var classificationGroup = vApi.MapGroup("/api/classification")
                                     .WithTags("Classification")
                                     .WithOpenApi();

        _ = classificationGroup.MapPost("/articles/{id:int}/classify", ClassifyArticleAsync)
                               .WithName("ClassifyArticle")
                               .WithSummary("Classifie automatiquement un article")
                               .WithDescription("Analyse un article et suggère des catégories appropriées en utilisant l'IA");

        _ = classificationGroup.MapPost("/articles/{id:int}/auto-assign", AutoAssignArticleAsync)
                               .WithName("AutoAssignArticle")
                               .WithSummary("Auto-assigne un article aux catégories appropriées")
                               .WithDescription("Classifie un article et l'assigne automatiquement aux catégories qui dépassent le seuil de confiance");

        _ = classificationGroup.MapPost("/articles/batch-classify", BatchClassifyArticlesAsync)
                               .WithName("BatchClassifyArticles")
                               .WithSummary("Classifie plusieurs articles en lot")
                               .WithDescription("Analyse plusieurs articles et suggère des catégories appropriées pour chacun");

        _ = classificationGroup.MapGet("/suggestions/new-categories", GetNewCategorySuggestionsAsync)
                               .WithName("GetNewCategorySuggestions")
                               .WithSummary("Obtient des suggestions de nouvelles catégories")
                               .WithDescription("Analyse les articles non catégorisés et suggère de nouvelles catégories à créer");

        _ = classificationGroup.MapPost("/feedback", ProvideClassificationFeedbackAsync)
                               .WithName("ProvideClassificationFeedback")
                               .WithSummary("Fournit un retour sur la classification")
                               .WithDescription("Permet d'améliorer l'IA en fournissant des retours sur la précision des classifications");

        _ = classificationGroup.MapGet("/suggestions", GetSuggestionsAsync)
                               .WithName("GetSuggestions")
                               .WithSummary("Obtient des suggestions de nouvelles catégories (endpoint simplifié)")
                               .WithDescription("Endpoint simplifié pour obtenir des suggestions de nouvelles catégories");

        _ = classificationGroup.MapPost("/batch", BatchClassifyAsync)
                               .WithName("BatchClassify")
                               .WithSummary("Classifie automatiquement tous les articles non catégorisés par lot")
                               .WithDescription("Endpoint simplifié pour classifier automatiquement tous les articles non catégorisés");
    }

    private static async Task<IResult> ClassifyArticleAsync([AsParameters] ClassifyArticleParameter p)
    {
        var article = await p.Context.Articles.FirstOrDefaultAsync(a => a.Id == p.Id, p.CancellationToken).ConfigureAwait(false);
        if (article == null)
            return Results.NotFound($"Article {p.Id} not found");

        var categories = await p.CategoryStore.GetAllCategoriesAsync(false, p.CancellationToken).ConfigureAwait(false);
        var categoryForClassification = categories.Where(c => c.IsActive)
                                                  .Select(c => new CategoryForClassification
                                                   {
                                                       Id = c.Id,
                                                       Name = c.Name,
                                                       Description = c.Description,
                                                       Keywords = c.Keywords,
                                                       AutoThreshold = p.Options?.MinAutoClassificationScore ?? 0.7,
                                                       ManualThreshold = p.Options?.MinSuggestionScore ?? 0.5,
                                                       IsActive = c.IsActive,
                                                   });

        var articleContent = $"{article.Title}\n\n{article.Summary}";
        var suggestions = await p.ClassificationService.ClassifyArticleAsync(articleContent, categoryForClassification, p.CancellationToken).ConfigureAwait(false);
        return Results.Ok(suggestions);
    }

    private static async Task<IResult> AutoAssignArticleAsync([AsParameters] AutoAssignArticleParameter p)
    {
        var article = await p.Context.Articles.FirstOrDefaultAsync(a => a.Id == p.Id, p.CancellationToken).ConfigureAwait(false);
        if (article == null)
            return Results.NotFound($"Article {p.Id} not found");

        var categories = await p.CategoryStore.GetAllCategoriesAsync(false, p.CancellationToken).ConfigureAwait(false);
        var categoryForClassification = categories.Where(c => c.IsActive)
                                                  .Select(c => new CategoryForClassification
                                                   {
                                                       Id = c.Id,
                                                       Name = c.Name,
                                                       Description = c.Description,
                                                       Keywords = c.Keywords,
                                                       AutoThreshold = p.Options?.MinAutoClassificationScore ?? 0.7,
                                                       ManualThreshold = p.Options?.MinSuggestionScore ?? 0.5,
                                                       IsActive = c.IsActive,
                                                   });

        var articleContent = $"{article.Title}\n\n{article.Summary}";
        CategorySuggestionResult[] suggestions = [.. await p.ClassificationService.ClassifyArticleAsync(articleContent, categoryForClassification, p.CancellationToken).ConfigureAwait(false)];
        var autoAssignedCategories = suggestions.Where(s => s.ExceedsAutoThreshold).ToList();
        foreach (var suggestion in autoAssignedCategories)
            _ = await p.CategoryStore.AssignCategoryToArticleAsync(p.Id, suggestion.CategoryId, false, suggestion.ConfidenceScore, p.CancellationToken).ConfigureAwait(false);

        return Results.Ok(new { AutoAssignedCategories = autoAssignedCategories.Count, Suggestions = suggestions });
    }

    private static async Task<IResult> BatchClassifyArticlesAsync([AsParameters] BatchClassifyArticlesParameter p)
    {
        var categories = await p.CategoryStore.GetAllCategoriesAsync(false, p.CancellationToken).ConfigureAwait(false);
        CategoryForClassification[] categoryForClassification =
        [
            .. categories.Where(c => c.IsActive)
                         .Select(c => new CategoryForClassification
                          {
                              Id = c.Id,
                              Name = c.Name,
                              Description = c.Description,
                              Keywords = c.Keywords,
                              AutoThreshold = p.Request.Options?.MinAutoClassificationScore ?? 0.7,
                              ManualThreshold = p.Request.Options?.MinSuggestionScore ?? 0.5,
                              IsActive = c.IsActive,
                          }),
        ];

        var results = new List<ArticleClassificationResult>();

        foreach (var articleId in p.Request.ArticleIds)
        {
            var article = await p.Context.Articles.FirstOrDefaultAsync(a => a.Id == articleId, p.CancellationToken).ConfigureAwait(false);

            if (article == null)
                continue;

            var articleContent = $"{article.Title}\n\n{article.Summary}";
            CategorySuggestionResult[] suggestions = [.. await p.ClassificationService.ClassifyArticleAsync(articleContent, categoryForClassification, p.CancellationToken).ConfigureAwait(false)];
            results.Add(new()
            {
                ArticleId = articleId,
                CategorySuggestions = suggestions,
                NewCategorySuggestions = [],
                OverallConfidence = suggestions.MaxBy(s => s.ConfidenceScore)?.ConfidenceScore ?? 0,
            });
        }

        return Results.Ok(results);
    }

    private static async Task<IResult> GetNewCategorySuggestionsAsync([AsParameters] GetNewCategorySuggestionsParameter p)
    {
        var uncategorizedArticles = await p.Context.Articles.Where(a => !a.ArticleCategories.Any())
                                           .OrderByDescending(a => a.AnalyzeDate)
                                           .Take(p.Limit ?? 50)
                                           .ToListAsync(p.CancellationToken)
                                           .ConfigureAwait(false);

        var existingCategoryNames = await p.Context.Categories.Where(c => c.IsActive)
                                           .Select(c => c.Name)
                                           .ToListAsync(p.CancellationToken)
                                           .ConfigureAwait(false);

        var allSuggestions = new List<NewCategorySuggestion>();

        foreach (var articleContent in uncategorizedArticles.Select(article => $"{article.Title}\n\n{article.Summary}"))
        {
            var suggestions = await p.ClassificationService.SuggestNewCategoriesAsync(articleContent, existingCategoryNames, p.CancellationToken).ConfigureAwait(false);
            allSuggestions.AddRange(suggestions);
        }

        var consolidatedSuggestions = allSuggestions.GroupBy(s => s.SuggestedName.ToLowerInvariant())
                                                    .Select(g => new ViewModels.NewCategorySuggestion
                                                     {
                                                         SuggestedName = g.First().SuggestedName,
                                                         SuggestedDescription = g.First().SuggestedDescription,
                                                         SuggestedKeywords = [.. g.SelectMany(s => s.SuggestedKeywords).Distinct()],
                                                         RelevanceScore = g.Average(s => s.RelevanceScore),
                                                         Justification = string.Join("; ", g.Select(s => s.Justification).Where(j => !string.IsNullOrEmpty(j))),
                                                     })
                                                    .OrderByDescending(s => s.RelevanceScore)
                                                    .Take(20)
                                                    .ToList();

        return Results.Ok(consolidatedSuggestions);
    }

    private static async Task<IResult> ProvideClassificationFeedbackAsync([AsParameters] ProvideClassificationFeedbackParameter p)
    {
        var article = await p.Context.Articles.FirstOrDefaultAsync(a => a.Id == p.Request.ArticleId, p.CancellationToken).ConfigureAwait(false);
        if (article == null)
            return Results.NotFound($"Article {p.Request.ArticleId} not found");

        var articleContent = $"{article.Title}\n\n{article.Summary}";
        await p.ClassificationService.LearnFromFeedbackAsync(articleContent,
                    p.Request.CorrectCategories,
                    p.Request.IncorrectCategories,
                    p.CancellationToken)
               .ConfigureAwait(false);

        return Results.Ok(new { Message = "Feedback enregistré avec succès" });
    }

    private static async Task<IResult> GetSuggestionsAsync([AsParameters] GetSuggestionsParameter p)
    {
        var uncategorizedArticles = await p.Context.Articles.Where(a => !a.ArticleCategories.Any())
                                           .OrderByDescending(a => a.AnalyzeDate)
                                           .Take(p.Limit ?? 50)
                                           .ToListAsync(p.CancellationToken)
                                           .ConfigureAwait(false);

        var existingCategoryNames = await p.Context.Categories.Where(c => c.IsActive)
                                           .Select(c => c.Name)
                                           .ToListAsync(p.CancellationToken)
                                           .ConfigureAwait(false);

        var allSuggestions = new List<ViewModels.NewCategorySuggestion>();

        foreach (var articleContent in uncategorizedArticles.Select(article => $"{article.Title}\n\n{article.Summary}"))
        {
            var suggestions = await p.ClassificationService.SuggestNewCategoriesAsync(articleContent, existingCategoryNames, p.CancellationToken).ConfigureAwait(false);

            allSuggestions.AddRange(suggestions.Select(suggestion => new ViewModels.NewCategorySuggestion()
            {
                SuggestedName = suggestion.SuggestedName,
                SuggestedDescription = suggestion.SuggestedDescription,
                SuggestedKeywords = suggestion.SuggestedKeywords,
                RelevanceScore = suggestion.RelevanceScore,
                Justification = suggestion.Justification,
                SuggestedColor = null,
                SuggestedIcon = null,
                SuggestedParentId = null,
                SuggestedParentName = null,
            }));
        }

        var consolidatedSuggestions = allSuggestions.GroupBy(s => s.SuggestedName.ToLowerInvariant())
                                                    .Select(g => new ViewModels.NewCategorySuggestion
                                                     {
                                                         SuggestedName = g.First().SuggestedName,
                                                         SuggestedDescription = g.First().SuggestedDescription,
                                                         SuggestedKeywords = [.. g.SelectMany(s => s.SuggestedKeywords).Distinct()],
                                                         RelevanceScore = g.Average(s => s.RelevanceScore),
                                                         Justification = string.Join("; ", g.Select(s => s.Justification).Where(j => !string.IsNullOrEmpty(j))),
                                                         SuggestedColor = null,
                                                         SuggestedIcon = null,
                                                         SuggestedParentId = null,
                                                         SuggestedParentName = null,
                                                     })
                                                    .OrderByDescending(s => s.RelevanceScore)
                                                    .Take(20)
                                                    .ToArray();

        return Results.Ok(consolidatedSuggestions);
    }

    private static async Task<IResult> BatchClassifyAsync([AsParameters] BatchClassifyParameter p)
    {
        var uncategorizedArticles = await p.Context.Articles.Where(a => !a.ArticleCategories.Any())
                                           .OrderByDescending(a => a.AnalyzeDate)
                                           .Take(100)
                                           .ToListAsync(p.CancellationToken)
                                           .ConfigureAwait(false);

        Category[] categories = [.. await p.CategoryStore.GetAllCategoriesAsync(false, p.CancellationToken).ConfigureAwait(false)];
        CategoryForClassification[] categoryForClassification =
        [
            .. categories.Where(c => c.IsActive)
                         .Select(c => new CategoryForClassification
                          {
                              Id = c.Id,
                              Name = c.Name,
                              Description = c.Description,
                              Keywords = c.Keywords,
                              AutoThreshold = 0.7,
                              ManualThreshold = 0.5,
                              IsActive = c.IsActive,
                          }),
        ];

        var results = new List<BatchClassificationResult>();

        foreach (var article in uncategorizedArticles)
        {
            try
            {
                var articleContent = $"{article.Title}\n\n{article.Summary}";
                CategorySuggestionResult[] suggestions = [.. await p.ClassificationService.ClassifyArticleAsync(articleContent, categoryForClassification, p.CancellationToken).ConfigureAwait(false)];
                var autoAssigned = new List<CategoryAssignmentModel>();

                foreach (var suggestion in suggestions.Where(s => s.ExceedsAutoThreshold))
                {
                    _ = await p.CategoryStore.AssignCategoryToArticleAsync(article.Id, suggestion.CategoryId, false, suggestion.ConfidenceScore, p.CancellationToken).ConfigureAwait(false);
                    autoAssigned.Add(new()
                    {
                        CategoryId = suggestion.CategoryId,
                        CategoryName = categories.First(c => c.Id == suggestion.CategoryId).Name,
                        ConfidenceScore = suggestion.ConfidenceScore,
                        IsAutomatic = true,
                    });
                }

                results.Add(new()
                {
                    ArticleId = article.Id,
                    ArticleTitle = article.Title,
                    AssignedCategories = [.. autoAssigned],
                    Success = true,
                });
            }
            catch (Exception ex)
            {
                results.Add(new()
                {
                    ArticleId = article.Id,
                    ArticleTitle = article.Title,
                    AssignedCategories = [],
                    Success = false,
                    ErrorMessage = ex.Message,
                });
            }
        }

        return Results.Ok(results.ToArray());
    }
}
