namespace Watch.Manager.ApiService.Parameters.Classification;

using Microsoft.AspNetCore.Mvc;

using Watch.Manager.Service.Analyse.Models;
using Watch.Manager.Service.Analyse.Services;
using Watch.Manager.Service.Database.Abstractions;
using Watch.Manager.Service.Database.Context;

/// <summary>
/// Represents the parameters required to classify a single article using AI classification.
/// </summary>
public record ClassifyArticleParameter
{
    /// <summary>
    /// Gets the ID of the article to classify.
    /// </summary>
    [FromRoute]
    public int Id { get; init; }

    /// <summary>
    /// Gets optional classification options for the operation.
    /// </summary>
    [FromBody]
    public ClassificationOptions? Options { get; init; }

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
