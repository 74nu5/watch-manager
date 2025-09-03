namespace Watch.Manager.ApiService.Parameters.Articles;

using Microsoft.AspNetCore.Mvc;

using Watch.Manager.Service.Database.Abstractions;

/// <summary>
/// Represents the parameters required to retrieve the thumbnail for a specific article.
/// </summary>
public record GetArticleThumbnailParameter
{
    /// <summary>
    /// Gets the ID of the article whose thumbnail is requested.
    /// </summary>
    [FromRoute]
    public int Id { get; init; }

    /// <summary>
    /// Gets store for article analysis data.
    /// </summary>
    [FromServices]
    public required IArticleAnalyseStore AnalyseStore { get; init; }

    /// <summary>
    /// Gets the HTTP context for the request.
    /// </summary>
    public required HttpContext HttpContext { get; init; }

    /// <summary>
    /// Gets token to cancel the operation if needed.
    /// </summary>
    public CancellationToken CancellationToken { get; init; }
}
