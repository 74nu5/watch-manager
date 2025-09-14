namespace Watch.Manager.ApiService.Extensions;

using System.Globalization;
using System.Net;

using Watch.Manager.ApiService.Parameters.Articles;
using Watch.Manager.ApiService.ViewModels;
using Watch.Manager.ApiService.ViewModels.SearchFacets;
using Watch.Manager.Service.Analyse.Models;
using Watch.Manager.Service.Database.Entities;

/// <summary>
///     Provides extension methods for mapping article-related endpoints to a <see cref="WebApplication" />.
/// </summary>
public static class ArticleEndpoints
{
    /// <summary>
    ///     Configures the application's endpoints for managing articles, including operations for saving, searching, retrieving thumbnails, and getting all tags.
    /// </summary>
    /// <param name="app">The <see cref="WebApplication" /> instance to which the article endpoints will be added.</param>
    public static void MapArticleEndpoints(this WebApplication app)
    {
        var vApi = app.NewVersionedApi("Articles");
        var api = vApi.MapGroup("api/articles");

        _ = api.MapPost("/save", SaveAndAnalyzeArticleAsync)
               .WithName("SaveAndAnalyzeArticle")
               .WithSummary("Analyse et sauvegarde un nouvel article, puis tente une classification automatique.");

        _ = api.MapGet("/search", SearchArticleAsync)
               .WithName("SearchArticle")
               .WithSummary("Recherche les articles par texte ou tag.");

        _ = api.MapGet("/search/advanced", AdvancedSearchArticleAsync)
               .WithName("AdvancedSearchArticle")
               .WithSummary("Recherche avancée d'articles avec filtres multicritères et facettes.");

        _ = api.MapGet("/thumbnail/{id:int}.png", GetArticleThumbnailAsync)
               .WithName("GetArticleThumbnail")
               .WithSummary("Récupère la miniature d'un article au format PNG.");

        _ = api.MapGet("tags", GetAllTagsAsync)
               .WithName("GetAllTags")
               .WithSummary("Récupère tous les tags utilisés dans les articles.");
    }

    private static async Task<IResult> SaveAndAnalyzeArticleAsync([AsParameters] AnalyzeParameter analyzeParameter, CancellationToken cancellationToken)
    {
        try
        {
            var urlAlreadyExists = await analyzeParameter.ArticleAnalyseStore.IsArticleExistsAsync(analyzeParameter.AnalyzeModel.UriToAnalyze, cancellationToken).ConfigureAwait(false);
            if (urlAlreadyExists)
                return Results.Conflict("Url already exists");

            var webSiteSource = await analyzeParameter.WebSiteService.GetWebSiteSource(analyzeParameter.AnalyzeModel.UriToAnalyze, CancellationToken.None).ConfigureAwait(false);
            var embeddingsHead = await analyzeParameter.ExtractEmbeddingAi.GetEmbeddingAsync(webSiteSource.Head, cancellationToken).ConfigureAwait(false);
            var embeddingsBody = await analyzeParameter.ExtractEmbeddingAi.GetEmbeddingAsync(webSiteSource.Body, cancellationToken).ConfigureAwait(false);

            if (embeddingsHead is null || embeddingsBody is null)
                return Results.Problem("Embedding failed");

            var analyzeResult = await analyzeParameter.ExtractDataAi.ExtractDatasAsync(webSiteSource.Body, cancellationToken).ConfigureAwait(false);

            if (analyzeResult is null)
                return Results.Problem("Analyze failed");

            Article article = new()
            {
                Summary = analyzeResult.Summary,
                Authors = analyzeResult.Authors,
                Url = analyzeParameter.AnalyzeModel.UriToAnalyze,
                Thumbnail = webSiteSource.Thumbnail,
                ThumbnailBase64 = webSiteSource.ThumbnailBase64,
                Title = analyzeResult.Title,
                EmbeddingHead = new(embeddingsHead),
                EmbeddingBody = new(embeddingsBody),
                Tags = [.. analyzeResult.Tags],
                AnalyzeDate = DateTime.UtcNow,
            };

            await analyzeParameter.ArticleAnalyseStore.StoreArticleAnalyzeAsync(article, cancellationToken).ConfigureAwait(false);

            // Classification automatique après l'ajout de l'article
            if (analyzeParameter.ClassificationService.IsEnabled)
            {
                try
                {
                    // Récupérer les catégories disponibles pour la classification
                    var categories = await analyzeParameter.CategoryStore.GetAllCategoriesAsync(false, cancellationToken).ConfigureAwait(false);
                    var categoryForClassification = categories.Where(c => c.IsActive)
                                                              .Select(c => new CategoryForClassification
                                                              {
                                                                  Id = c.Id,
                                                                  Name = c.Name,
                                                                  Description = c.Description,
                                                                  Keywords = c.Keywords,
                                                                  AutoThreshold = 0.7, // Seuil par défaut pour l'auto-assignation
                                                                  ManualThreshold = 0.5, // Seuil par défaut pour les suggestions
                                                                  IsActive = c.IsActive,
                                                              });

                    // Classification de l'article
                    var articleContent = $"{article.Title}\n\n{article.Summary}";
                    var suggestions = await analyzeParameter.ClassificationService.ClassifyArticleAsync(articleContent, categoryForClassification, cancellationToken).ConfigureAwait(false);

                    // Auto-assigner les catégories qui dépassent le seuil automatique
                    var autoAssignedCategories = suggestions.Where(s => s.ExceedsAutoThreshold)
                                                            .ToList();

                    foreach (var suggestion in autoAssignedCategories)
                    {
                        _ = await analyzeParameter.CategoryStore.AssignCategoryToArticleAsync(
                                                       article.Id,
                                                       suggestion.CategoryId,
                                                       false, // Pas manuel, c'est automatique
                                                       suggestion.ConfidenceScore,
                                                       cancellationToken)
                                                  .ConfigureAwait(false);
                    }

                    analyzeParameter.Logger.LogInformation("Article {ArticleId} automatiquement classifié avec {Count} catégories", article.Id, autoAssignedCategories.Count);
                }
                catch (Exception classificationException)
                {
                    // Ne pas faire échouer l'ajout d'article si la classification échoue
                    analyzeParameter.Logger.LogWarning(classificationException, "Échec de la classification automatique pour l'article {ArticleId}", article.Id);
                }
            }

            return Results.Ok(analyzeResult);
        }
        catch (HttpRequestException httpRequestException) when (httpRequestException.StatusCode == HttpStatusCode.NotFound)
        {
            analyzeParameter.Logger.LogWarning(httpRequestException, "Url not found");
            return Results.NotFound("Url not found");
        }
        catch (HttpRequestException httpRequestException) when (httpRequestException.StatusCode == HttpStatusCode.Forbidden)
        {
            analyzeParameter.Logger.LogWarning(httpRequestException, "Url is forbidden");
            return Results.StatusCode(403);
        }
        catch (Exception e)
        {
            analyzeParameter.Logger.LogError(e, "Failed to save article");
            return Results.InternalServerError(e);
        }
    }

    private static async IAsyncEnumerable<ArticleViewModel> SearchArticleAsync([AsParameters] SearchArticleParameter p)
    {
        await foreach (var article in p.AnalyseStore.SearchArticleAsync(p.Text, p.Tag, p.CancellationToken).ConfigureAwait(false))
        {
            yield return new()
            {
                Id = article.Id,
                Summary = article.Summary,
                Authors = article.Authors,
                Url = article.Url,
                Title = article.Title,
                Tags = article.Tags,
                AnalyzeDate = article.AnalyzeDate,
                Thumbnail = article.Thumbnail,
                Score = article.Score,
                Categories = article.Categories,
            };
        }
    }

    private static async Task<IResult> GetArticleThumbnailAsync([AsParameters] GetArticleThumbnailParameter p)
    {
        var (memoryStream, fileName) = await p.AnalyseStore.GetThumbnailAsync(p.Id, p.CancellationToken).ConfigureAwait(false);
        p.HttpContext.Response.Headers.ContentDisposition = $"inline; filename=\"{fileName ?? "thumbnail.png"}\"";
        return Results.File(memoryStream, $"image/{Path.GetExtension(fileName)?.Replace(".", string.Empty).ToLower(CultureInfo.CurrentCulture) ?? "png"}");
    }

    private static async Task<IResult> GetAllTagsAsync([AsParameters] GetAllTagsParameter p)
    {
        var tags = await p.ArticleAnalyseStore.GetAllTagsAsync(p.CancellationToken).ConfigureAwait(false);
        return Results.Ok(tags.Distinct(StringComparer.InvariantCultureIgnoreCase));
    }

    private static async Task<IResult> AdvancedSearchArticleAsync([AsParameters] AdvancedSearchArticleParameter p)
    {
        try
        {
            var filters = p.ToFilters();
            var result = await p.AnalyseStore.AdvancedSearchArticlesAsync(filters, p.IncludeFacets, p.CancellationToken).ConfigureAwait(false);

            var viewModel = new ArticleSearchResultViewModel
            {
                Articles =
                [
                    .. result.Articles.Select(article => new ArticleViewModel
                    {
                        Id = article.Id,
                        Title = article.Title,
                        Summary = article.Summary,
                        Authors = article.Authors,
                        Url = article.Url,
                        Tags = article.Tags,
                        AnalyzeDate = article.AnalyzeDate,
                        Thumbnail = article.Thumbnail,
                        Score = article.Score,
                        Categories = article.Categories,
                    }),
                ],
                TotalCount = result.TotalCount,
                Count = result.Count,
                Offset = result.Offset,
                Limit = result.Limit,
                Facets = result.Facets != null
                                 ? new SearchFacetsViewModel
                                 {
                                     Categories =
                                     [
                                         .. result.Facets.Categories.Select(f => new CategoryFacetViewModel
                                         {
                                             CategoryId = f.CategoryId,
                                             CategoryName = f.CategoryName,
                                             Count = f.Count,
                                             Color = f.Color,
                                             Icon = f.Icon,
                                         }),
                                     ],
                                     Tags =
                                     [
                                         .. result.Facets.Tags.Select(f => new TagFacetViewModel
                                         {
                                             TagName = f.TagName,
                                             Count = f.Count,
                                         }),
                                     ],
                                     Authors =
                                     [
                                         .. result.Facets.Authors.Select(f => new AuthorFacetViewModel
                                         {
                                             AuthorName = f.AuthorName,
                                             Count = f.Count,
                                         }),
                                     ],
                                     DateDistribution =
                                     [
                                         .. result.Facets.DateDistribution.Select(f => new DateFacetViewModel
                                         {
                                             Period = f.Period,
                                             Count = f.Count,
                                             PeriodStart = f.PeriodStart,
                                             PeriodEnd = f.PeriodEnd,
                                         }),
                                     ],
                                 }
                                 : null,
            };

            return Results.Ok(viewModel);
        }
        catch (Exception e)
        {
            return Results.Problem($"Erreur lors de la recherche avancée: {e.Message}");
        }
    }
}
