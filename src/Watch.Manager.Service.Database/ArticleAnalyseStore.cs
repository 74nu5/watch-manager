namespace Watch.Manager.Service.Database;

using System.Runtime.CompilerServices;
using System.Text.Json;

using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.VectorData;

using Watch.Manager.Common.Enumerations;
using Watch.Manager.Service.Database.Abstractions;
using Watch.Manager.Service.Database.Context;
using Watch.Manager.Service.Database.Entities;
using Watch.Manager.Service.Database.Models;
using Watch.Manager.Service.Database.Models.SearchFacets;

/// <summary>
///     Provides storage and retrieval operations for analyzed articles, including search and thumbnail access.
/// </summary>
/// <param name="logger">The logger instance used for logging information and errors.</param>
/// <param name="articlesContext">The database context used for accessing and managing article data.</param>
/// <param name="searchable">The vector searchable interface used for performing vector-based searches on article data.</param>
internal sealed class ArticleAnalyseStore(ILogger<ArticleAnalyseStore> logger, ArticlesContext articlesContext, IVectorSearchable<ArticleSearchEntity> searchable) : IArticleAnalyseStore
{
    private readonly ILogger<ArticleAnalyseStore> logger = logger;
    private readonly ArticlesContext articlesContext = articlesContext;
    private readonly IVectorSearchable<ArticleSearchEntity> searchable = searchable;

    /// <summary>
    ///     Stores an analyzed article asynchronously in the database.
    /// </summary>
    /// <param name="analyzeModel">The analyzed article model to store.</param>
    /// <param name="cancellationToken">A token to monitor for cancellation requests.</param>
    /// <returns>A task representing the asynchronous operation.</returns>
    public async Task StoreArticleAnalyzeAsync(Article analyzeModel, CancellationToken cancellationToken)
    {
        _ = this.articlesContext.Articles.Add(analyzeModel);
        _ = await this.articlesContext.SaveChangesAsync(cancellationToken).ConfigureAwait(false);
    }

    /// <summary>
    ///     Checks asynchronously if an article exists by its URL.
    /// </summary>
    /// <param name="url">The URL of the article to check.</param>
    /// <param name="cancellationToken">A token to monitor for cancellation requests.</param>
    /// <returns>A task that returns true if the article exists, otherwise false.</returns>
    public async Task<bool> IsArticleExistsAsync(Uri url, CancellationToken cancellationToken)
        => await this.articlesContext.Articles.AnyAsync(p => p.Url == url, cancellationToken).ConfigureAwait(false);

    /// <summary>
    ///     Searches for articles asynchronously based on search terms and/or tag.
    /// </summary>
    /// <param name="searchTerms">The search terms to filter articles, or null to ignore.</param>
    /// <param name="tag">The tag to filter articles, or null to ignore.</param>
    /// <param name="cancellationToken">A token to monitor for cancellation requests.</param>
    /// <returns>An asynchronous stream of article result DTOs matching the criteria.</returns>
    public async IAsyncEnumerable<ArticleResultDto> SearchArticleAsync(string? searchTerms, string? tag, [EnumeratorCancellation] CancellationToken cancellationToken)
    {
        if (!string.IsNullOrWhiteSpace(tag))
        {
            // Filter by tag if provided
            await foreach (var article in this.articlesContext.Articles.AsNoTracking()
                                              .Include(a => a.ArticleCategories)
                                              .ThenInclude(ac => ac.Category)
                                              .AsQueryable()
                                              .Where(a => a.Tags.Contains(tag))
                                              .OrderByDescending(a => a.AnalyzeDate)
                                              .AsAsyncEnumerable()
                                              .WithCancellation(cancellationToken)
                                              .ConfigureAwait(false))
            {
                yield return new()
                {
                    Id = article.Id,
                    Title = article.Title,
                    Tags = article.Tags,
                    Authors = article.Authors,
                    Summary = article.Summary,
                    Url = article.Url,
                    AnalyzeDate = article.AnalyzeDate,
                    Thumbnail = article.Thumbnail,
                    Score = 0, // Default score when no search terms are provided
                    Categories = [.. article.ArticleCategories.Where(ac => ac.Category != null).Select(ac => ac.Category!.Name)],
                };
            }

            yield break;
        }

        if (!string.IsNullOrWhiteSpace(searchTerms))
        {
            VectorSearchOptions<ArticleSearchEntity> vectorSearchOptions = new()
            {
                VectorProperty = article => article.EmbeddingBody,
            };

            await foreach (var result in this.searchable.SearchAsync(searchTerms, 10, vectorSearchOptions, cancellationToken).ConfigureAwait(false))
            {
                // Retrieve categories for this specific article
                var articleCategories = await this.articlesContext.ArticleCategories
                                                  .AsNoTracking()
                                                  .Where(ac => ac.ArticleId == result.Record.Id)
                                                  .Include(ac => ac.Category)
                                                  .Select(ac => ac.Category!.Name)
                                                  .ToArrayAsync(cancellationToken)
                                                  .ConfigureAwait(false);

                yield return new()
                {
                    Id = result.Record.Id,
                    Title = result.Record.Title,
                    Tags = JsonSerializer.Deserialize<string[]>(result.Record.Tags) ?? [],
                    Authors = JsonSerializer.Deserialize<string[]>(result.Record.Authors) ?? [],
                    Summary = result.Record.Summary,
                    Url = new(result.Record.Url),
                    AnalyzeDate = result.Record.AnalyzeDate,
                    Thumbnail = new(result.Record.Thumbnail),
                    Score = result.Score,
                    Categories = articleCategories,
                };
            }

            yield break;
        }

        // Return all articles if no search terms or tag are provided
        await foreach (var article in this.articlesContext.Articles.AsNoTracking()
                                          .Include(a => a.ArticleCategories)
                                          .ThenInclude(ac => ac.Category)
                                          .OrderByDescending(a => a.AnalyzeDate)
                                          .AsAsyncEnumerable()
                                          .WithCancellation(cancellationToken)
                                          .ConfigureAwait(false))
        {
            yield return new()
            {
                Id = article.Id,
                Title = article.Title,
                Tags = article.Tags,
                Authors = article.Authors,
                Summary = article.Summary,
                Url = article.Url,
                AnalyzeDate = article.AnalyzeDate,
                Thumbnail = article.Thumbnail,
                Score = 0, // Default score when no search terms are provided
                Categories = [.. article.ArticleCategories.Where(ac => ac.Category != null).Select(ac => ac.Category!.Name)],
            };
        }
    }

    /// <inheritdoc />
    public async Task<ArticleSearchResult> AdvancedSearchArticlesAsync(ArticleSearchFilters filters, bool includeFacets = false, CancellationToken cancellationToken = default)
    {
        // Build the base query
        var query = this.articlesContext.Articles.AsNoTracking()
                        .Include(a => a.ArticleCategories)
                        .ThenInclude(ac => ac.Category)
                        .AsQueryable();

        // Apply filters
        if (filters.CategoryIds?.Length > 0)
            query = query.Where(a => a.ArticleCategories.Any(ac => filters.CategoryIds.Contains(ac.CategoryId)));

        if (filters.CategoryNames?.Length > 0)
            query = query.Where(a => a.ArticleCategories.Any(ac => ac.Category != null && filters.CategoryNames.Contains(ac.Category.Name)));

        if (filters.Tags?.Length > 0)
            query = query.Where(a => filters.Tags.Any(tag => a.Tags.Contains(tag)));

        if (filters.Authors?.Length > 0)
            query = query.Where(a => filters.Authors.Any(author => a.Authors.Contains(author)));

        if (filters.DateFrom.HasValue)
            query = query.Where(a => a.AnalyzeDate >= filters.DateFrom.Value);

        if (filters.DateTo.HasValue)
            query = query.Where(a => a.AnalyzeDate <= filters.DateTo.Value);

        // Handle text search
        if (!string.IsNullOrWhiteSpace(filters.SearchTerms))
        {
            // For text search, use EF Core for basic search, then refine with vector search
            var textLower = filters.SearchTerms.ToLowerInvariant();
            query = query.Where(a =>
                    a.Title.Contains(textLower) ||
                    a.Summary.Contains(textLower) ||
                    a.Tags.Any(tag => tag.Contains(textLower)));
        }

        // Count total before pagination
        var totalCount = await query.CountAsync(cancellationToken).ConfigureAwait(false);

        // Apply sorting
        query = filters.SortBy switch
        {
            ArticleSortBy.Title => filters.SortOrder == SortOrder.Descending ? query.OrderByDescending(a => a.Title) : query.OrderBy(a => a.Title),
            ArticleSortBy.CategoryCount => filters.SortOrder == SortOrder.Descending ? query.OrderByDescending(a => a.ArticleCategories.Count) : query.OrderBy(a => a.ArticleCategories.Count),
            _ => filters.SortOrder == SortOrder.Ascending ? query.OrderBy(a => a.AnalyzeDate) : query.OrderByDescending(a => a.AnalyzeDate),
        };

        // Apply pagination
        var offset = filters.Offset ?? 0;
        var limit = filters.Limit ?? 50; // Default limit

        query = query.Skip(offset).Take(limit);

        // Execute the query
        var articles = await query.ToArrayAsync(cancellationToken).ConfigureAwait(false);

        // Convert to ArticleResultDto
        var articleResults = articles.Select(article => new ArticleResultDto
        {
            Id = article.Id,
            Title = article.Title,
            Tags = article.Tags,
            Authors = article.Authors,
            Summary = article.Summary,
            Url = article.Url,
            AnalyzeDate = article.AnalyzeDate,
            Thumbnail = article.Thumbnail,
            Score = null, // Score will be handled separately for vector search
            Categories =
            [
                .. article.ArticleCategories
                          .Where(ac => ac.Category != null)
                          .Select(ac => ac.Category!.Name),
            ],
        }).ToArray();

        // Handle vector search if search terms are provided
        if (!string.IsNullOrWhiteSpace(filters.SearchTerms))
            await this.EnhanceWithVectorSearchAsync(articleResults, filters.SearchTerms, cancellationToken).ConfigureAwait(false);

        // Generate facets if requested
        SearchFacets? facets = null;
        if (includeFacets)
            facets = await this.GenerateFacetsAsync(filters, cancellationToken).ConfigureAwait(false);

        return new()
        {
            Articles = articleResults,
            TotalCount = totalCount,
            Count = articleResults.Length,
            Offset = offset,
            Limit = limit,
            Facets = facets,
        };
    }

    /// <summary>
    ///     Retrieves all unique tags used in articles asynchronously.
    /// </summary>
    /// <param name="cancellationToken">A token to monitor for cancellation requests.</param>
    /// <returns>A task that returns an array of all tags.</returns>
    public async Task<string[]> GetAllTagsAsync(CancellationToken cancellationToken)
        => await this.articlesContext.Articles.SelectMany(p => p.Tags).Distinct().OrderBy(s => s).ToArrayAsync(cancellationToken).ConfigureAwait(false);

    /// <summary>
    ///     Retrieves the thumbnail image for a given article asynchronously.
    /// </summary>
    /// <param name="id">The unique identifier of the article.</param>
    /// <param name="cancellationToken">A token to monitor for cancellation requests.</param>
    /// <returns>
    ///     A value task that returns a tuple containing the thumbnail as a memory stream and the file name (if available).
    /// </returns>
    public async ValueTask<(MemoryStream, string? FileName)> GetThumbnailAsync(int id, CancellationToken cancellationToken)
    {
        var articleThumbnail = await this.articlesContext.Articles.Where(article => article.Id == id).Select(article => new
        {
            FileName = Path.GetFileName(article.Thumbnail.LocalPath),
            article.ThumbnailBase64,
        }).FirstOrDefaultAsync(cancellationToken).ConfigureAwait(false);

        if (articleThumbnail is null)
            return ((MemoryStream, string? FileName))(Stream.Null, null);

        // Convert base64 string to stream
        var thumbnailBytes = Convert.FromBase64String(articleThumbnail.ThumbnailBase64);
        return (new(thumbnailBytes) { Position = 0 }, articleThumbnail.FileName);
    }

    /// <summary>
    ///     Enhance search results with vector search scores.
    /// </summary>
    private async Task EnhanceWithVectorSearchAsync(ArticleResultDto[] articles, string searchTerms, CancellationToken cancellationToken)
    {
        try
        {
            VectorSearchOptions<ArticleSearchEntity> vectorSearchOptions = new()
            {
                VectorProperty = article => article.EmbeddingBody,
            };

            var vectorResults = new Dictionary<int, double>();

            await foreach (var result in this.searchable.SearchAsync(searchTerms, 100, vectorSearchOptions, cancellationToken).ConfigureAwait(false))
            {
                if (result.Score.HasValue)
                    vectorResults[result.Record.Id] = result.Score.Value;
            }

            // Update articles with vector scores
            for (var i = 0; i < articles.Length; i++)
            {
                if (vectorResults.TryGetValue(articles[i].Id, out var score))
                    articles[i] = articles[i] with { Score = score };
            }
        }
        catch (Exception)
        {
            // In case of an error with vector search, continue without scores
        }
    }

    /// <summary>
    ///     Generates search facets based on the current filters.
    /// </summary>
    private async Task<SearchFacets> GenerateFacetsAsync(ArticleSearchFilters filters, CancellationToken cancellationToken)
    {
        // Base query for facets (without pagination)
        var baseQuery = this.articlesContext.Articles.AsNoTracking()
                            .Include(a => a.ArticleCategories)
                            .ThenInclude(ac => ac.Category)
                            .AsQueryable();

        // Apply the same filters as the main search (except for the facets themselves)
        if (filters.DateFrom.HasValue)
            baseQuery = baseQuery.Where(a => a.AnalyzeDate >= filters.DateFrom.Value);

        if (filters.DateTo.HasValue)
            baseQuery = baseQuery.Where(a => a.AnalyzeDate <= filters.DateTo.Value);

        if (!string.IsNullOrWhiteSpace(filters.SearchTerms))
        {
            var textLower = filters.SearchTerms.ToLowerInvariant();
            baseQuery = baseQuery.Where(a =>
                    a.Title.Contains(textLower) ||
                    a.Summary.Contains(textLower) ||
                    a.Tags.Any(tag => tag.Contains(textLower)));
        }

        CategoryFacet[] categoryFacets;
        TagFacet[] tagFacets;
        AuthorFacet[] authorFacets;
        DateFacet[] dateFacets;

        // Facets by category
        try
        {
            categoryFacets = await baseQuery
                                  .SelectMany(a => a.ArticleCategories)
                                  .Where(ac => ac.Category != null)
                                  .GroupBy(ac => new { ac.Category!.Id, ac.Category.Name, ac.Category.Color, ac.Category.Icon })
                                  .Select(g => new CategoryFacet
                                  {
                                      CategoryId = g.Key.Id,
                                      CategoryName = g.Key.Name,
                                      Color = g.Key.Color,
                                      Icon = g.Key.Icon,
                                      Count = g.Count(),
                                  })
                                  .OrderByDescending(cf => cf.Count)
                                  .ToArrayAsync(cancellationToken)
                                  .ConfigureAwait(false);
        }
        catch (Exception e)
        {
            this.logger.LogWarning(e, "Failed to generate category facets.");
            categoryFacets = [];
        }

        // Facets by tag
        try
        {
            tagFacets = await baseQuery
                             .SelectMany(a => a.Tags)
                             .GroupBy(tag => tag)
                             .Select(g => new TagFacet
                             {
                                 TagName = g.Key,
                                 Count = g.Count(),
                             })
                             .OrderByDescending(tf => tf.Count)
                             .Take(20) // Limiter à 20 tags les plus populaires
                             .ToArrayAsync(cancellationToken)
                             .ConfigureAwait(false);
        }
        catch (Exception e)
        {
            this.logger.LogWarning(e, "Failed to generate tag facets.");
            tagFacets = [];
        }

        // Facets by author
        try
        {
            authorFacets = await baseQuery
                                .SelectMany(a => a.Authors)
                                .GroupBy(author => author)
                                .Select(g => new AuthorFacet
                                {
                                    AuthorName = g.Key,
                                    Count = g.Count(),
                                })
                                .OrderByDescending(af => af.Count)
                                .Take(20) // Limiter à 20 auteurs les plus populaires
                                .ToArrayAsync(cancellationToken)
                                .ConfigureAwait(false);
        }
        catch (Exception e)
        {
            this.logger.LogWarning(e, "Failed to generate author facets.");
            authorFacets = [];
        }

        // Facets by date (manual grouping by year and month)
        try
        {
            dateFacets = baseQuery
                        .GroupBy(a => new { a.AnalyzeDate.Year, a.AnalyzeDate.Month })
                        .AsEnumerable()
                        .Select(g => new DateFacet
                        {
                            Period = $"{g.Key.Year:D4}-{g.Key.Month:D2}",
                            Count = g.Count(),
                            PeriodStart = new(g.Key.Year, g.Key.Month, 1),
                            PeriodEnd = new(g.Key.Year, g.Key.Month, DateTime.DaysInMonth(g.Key.Year, g.Key.Month)),
                        })
                        .OrderByDescending(df => df.PeriodStart)
                        .ToArray();
        }
        catch (Exception e)
        {
            this.logger.LogWarning(e, "Failed to generate date facets.");
            dateFacets = [];
        }

        return new()
        {
            Categories = categoryFacets,
            Tags = tagFacets,
            Authors = authorFacets,
            DateDistribution = dateFacets,
        };
    }
}
