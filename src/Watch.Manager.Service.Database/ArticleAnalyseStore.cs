namespace Watch.Manager.Service.Database;

using System.Runtime.CompilerServices;
using System.Text.Json;

using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.VectorData;

using Watch.Manager.Service.Database.Abstractions;
using Watch.Manager.Service.Database.Context;
using Watch.Manager.Service.Database.Entities;
using Watch.Manager.Service.Database.Models;

/// <summary>
///     Provides storage and retrieval operations for analyzed articles, including search and thumbnail access.
/// </summary>
/// <param name="articlesContext">The database context used for accessing and managing article data.</param>
/// <param name="searchable">The vector searchable interface used for performing vector-based searches on article data.</param>
internal sealed class ArticleAnalyseStore(ArticlesContext articlesContext, IVectorSearchable<ArticleSearchEntity> searchable) : IArticleAnalyseStore
{
    /// <summary>
    ///     Stores an analyzed article asynchronously in the database.
    /// </summary>
    /// <param name="analyzeModel">The analyzed article model to store.</param>
    /// <param name="cancellationToken">A token to monitor for cancellation requests.</param>
    /// <returns>A task representing the asynchronous operation.</returns>
    public async Task StoreArticleAnalyzeAsync(Article analyzeModel, CancellationToken cancellationToken)
    {
        _ = articlesContext.Articles.Add(analyzeModel);
        _ = await articlesContext.SaveChangesAsync(cancellationToken).ConfigureAwait(false);
    }

    /// <summary>
    ///     Checks asynchronously if an article exists by its URL.
    /// </summary>
    /// <param name="url">The URL of the article to check.</param>
    /// <param name="cancellationToken">A token to monitor for cancellation requests.</param>
    /// <returns>A task that returns true if the article exists, otherwise false.</returns>
    public async Task<bool> IsArticleExistsAsync(Uri url, CancellationToken cancellationToken)
        => await articlesContext.Articles.AnyAsync(p => p.Url == url, cancellationToken).ConfigureAwait(false);

    /// <summary>
    ///     Searches for articles asynchronously based on search terms and/or tag.
    /// </summary>
    /// <param name="searchTerms">The search terms to filter articles, or null to ignore.</param>
    /// <param name="tag">The tag to filter articles, or null to ignore.</param>
    /// <param name="cancellationToken">A token to monitor for cancellation requests.</param>
    /// <returns>An asynchronous stream of article result DTOs matching the criteria.</returns>
    public async IAsyncEnumerable<ArticleResultDto> SearchArticleAsync(string? searchTerms,
                                                                       string? tag,
                                                                       [EnumeratorCancellation] CancellationToken cancellationToken)
    {
        if (!string.IsNullOrWhiteSpace(tag))
        {
            // Filter by tag if provided
            await foreach (var article in articlesContext.Articles.AsNoTracking()
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

            await foreach (var result in searchable.SearchAsync(searchTerms, 10, vectorSearchOptions, cancellationToken).ConfigureAwait(false))
            {
                // Retrieve categories for this specific article
                var articleCategories = await articlesContext.ArticleCategories
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
        await foreach (var article in articlesContext.Articles.AsNoTracking()
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

    /// <summary>
    ///     Retrieves all unique tags used in articles asynchronously.
    /// </summary>
    /// <param name="cancellationToken">A token to monitor for cancellation requests.</param>
    /// <returns>A task that returns an array of all tags.</returns>
    public async Task<string[]> GetAllTagsAsync(CancellationToken cancellationToken)
        => await articlesContext.Articles.SelectMany(p => p.Tags).Distinct().OrderBy(s => s).ToArrayAsync(cancellationToken).ConfigureAwait(false);

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
        var articleThumbnail = await articlesContext.Articles.Where(article => article.Id == id).Select(article => new
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
}
