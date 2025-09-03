namespace Watch.Manager.Service.Database.Abstractions;

using Watch.Manager.Service.Database.Entities;
using Watch.Manager.Service.Database.Models;

/// <summary>
///     Interface for storing and retrieving analyzed articles and related data.
/// </summary>
public interface IArticleAnalyseStore
{
    /// <summary>
    ///     Stores an analyzed article asynchronously.
    /// </summary>
    /// <param name="analyzeModel">The analyzed article model to store.</param>
    /// <param name="cancellationToken">A token to monitor for cancellation requests.</param>
    /// <returns>A task representing the asynchronous operation.</returns>
    Task StoreArticleAnalyzeAsync(Article analyzeModel, CancellationToken cancellationToken);

    /// <summary>
    ///     Searches for articles asynchronously based on search terms and tag.
    /// </summary>
    /// <param name="searchTerms">The search terms to filter articles, or null to ignore.</param>
    /// <param name="tag">The tag to filter articles, or null to ignore.</param>
    /// <param name="cancellationToken">A token to monitor for cancellation requests.</param>
    /// <returns>An asynchronous stream of article result DTOs matching the criteria.</returns>
    IAsyncEnumerable<ArticleResultDto> SearchArticleAsync(string? searchTerms, string? tag, CancellationToken cancellationToken);

    /// <summary>
    ///     Checks asynchronously if an article exists by its URL.
    /// </summary>
    /// <param name="url">The URL of the article to check.</param>
    /// <param name="cancellationToken">A token to monitor for cancellation requests.</param>
    /// <returns>A task that returns true if the article exists, otherwise false.</returns>
    Task<bool> IsArticleExistsAsync(Uri url, CancellationToken cancellationToken);

    /// <summary>
    ///     Retrieves all tags used in articles asynchronously.
    /// </summary>
    /// <param name="cancellationToken">A token to monitor for cancellation requests.</param>
    /// <returns>A task that returns an array of all tags.</returns>
    Task<string[]> GetAllTagsAsync(CancellationToken cancellationToken);

    /// <summary>
    ///     Retrieves the thumbnail image for a given article asynchronously.
    /// </summary>
    /// <param name="id">The unique identifier of the article.</param>
    /// <param name="cancellationToken">A token to monitor for cancellation requests.</param>
    /// <returns>
    ///     A value task that returns a tuple containing the thumbnail as a memory stream and the file name (if available).
    /// </returns>
    ValueTask<(MemoryStream, string? FileName)> GetThumbnailAsync(int id, CancellationToken cancellationToken);
}
