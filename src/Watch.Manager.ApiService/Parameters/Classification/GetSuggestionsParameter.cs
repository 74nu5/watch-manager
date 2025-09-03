namespace Watch.Manager.ApiService.Parameters.Classification;

using Microsoft.AspNetCore.Mvc;

using Watch.Manager.Service.Analyse.Services;
using Watch.Manager.Service.Database.Context;

/// <summary>
/// Represents the parameters required to get article suggestions using AI classification.
/// </summary>
public record GetSuggestionsParameter
{
    /// <summary>
    /// Gets optional limit for the number of suggestions to return.
    /// </summary>
    [FromQuery]
    public int? Limit { get; init; }

    /// <summary>
    /// Gets service for article classification using AI.
    /// </summary>
    [FromServices]
    public required IArticleClassificationAI ClassificationService { get; init; }

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
