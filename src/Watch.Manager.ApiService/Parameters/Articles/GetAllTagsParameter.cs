namespace Watch.Manager.ApiService.Parameters.Articles;

using Microsoft.AspNetCore.Mvc;

using Watch.Manager.Service.Database.Abstractions;

/// <summary>
/// Represents the parameters required to retrieve all tags for articles.
/// </summary>
public record GetAllTagsParameter
{
    /// <summary>
    /// Gets store for article analysis data.
    /// </summary>
    [FromServices]
    public required IArticleAnalyseStore ArticleAnalyseStore { get; init; }

    /// <summary>
    /// Gets token to cancel the operation if needed.
    /// </summary>
    public CancellationToken CancellationToken { get; init; }
}
