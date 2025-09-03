namespace Watch.Manager.ApiService.ViewModels;

using Watch.Manager.Service.Analyse.Models;

/// <summary>
///     Request model for batch classification of articles.
/// </summary>
public sealed class BatchClassificationRequest
{
    /// <summary>
    ///     Gets the identifiers of the articles to classify.
    /// </summary>
    public required int[] ArticleIds { get; init; }

    /// <summary>
    ///     Gets the classification options.
    /// </summary>
    public ClassificationOptions? Options { get; init; }
}
