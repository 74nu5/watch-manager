namespace Watch.Manager.ApiService.Parameters.Classification;

using Microsoft.AspNetCore.Mvc;

using Watch.Manager.ApiService.ViewModels;
using Watch.Manager.Service.Analyse.Services;
using Watch.Manager.Service.Database.Context;

/// <summary>
/// Represents the parameters required to provide feedback for article classification.
/// </summary>
public record ProvideClassificationFeedbackParameter
{
    /// <summary>
    /// Gets the feedback request model containing feedback data.
    /// </summary>
    [FromBody]
    public required ClassificationFeedbackRequest Request { get; init; }

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
