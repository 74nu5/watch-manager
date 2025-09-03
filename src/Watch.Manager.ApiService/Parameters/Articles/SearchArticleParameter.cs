namespace Watch.Manager.ApiService.Parameters.Articles;

using Microsoft.AspNetCore.Mvc;

using Watch.Manager.Service.Analyse.Services;
using Watch.Manager.Service.Database.Abstractions;

/// <summary>
/// Represents the parameters required to search for articles by text or tag.
/// </summary>
public record SearchArticleParameter
{
    /// <summary>
    /// Gets the text to search for in articles.
    /// </summary>
    [FromQuery]
    public string? Text { get; init; }

    /// <summary>
    /// Gets the tag to filter articles by.
    /// </summary>
    [FromQuery]
    public string? Tag { get; init; }

    /// <summary>
    /// Gets service for extracting AI embeddings from articles.
    /// </summary>
    [FromServices]
    public required IExtractEmbeddingAI ExtractEmbedding { get; init; }

    /// <summary>
    /// Gets store for article analysis data.
    /// </summary>
    [FromServices]
    public required IArticleAnalyseStore AnalyseStore { get; init; }

    /// <summary>
    /// Gets token to cancel the operation if needed.
    /// </summary>
    public CancellationToken CancellationToken { get; init; }
}
