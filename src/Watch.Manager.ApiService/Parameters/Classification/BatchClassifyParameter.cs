namespace Watch.Manager.ApiService.Parameters.Classification;

using Microsoft.AspNetCore.Mvc;

using Watch.Manager.Service.Analyse.Services;
using Watch.Manager.Service.Database.Abstractions;
using Watch.Manager.Service.Database.Context;

/// <summary>
/// Represents the parameters required for a batch classification operation on articles.
/// </summary>
public record BatchClassifyParameter
{
    /// <summary>
    /// Gets service for article classification using AI.
    /// </summary>
    [FromServices]
    public required IArticleClassificationAI ClassificationService { get; init; }

    /// <summary>
    /// Gets store for category data.
    /// </summary>
    [FromServices]
    public required ICategoryStore CategoryStore { get; init; }

    /// <summary>
    /// Gets database context for articles.
    /// </summary>
    [FromServices]
    public required ArticlesContext Context { get; init; }

    /// <summary>
    /// Gets token to cancel the operation if needed.
    /// </summary>
    public CancellationToken CancellationToken { get; init; }
}
