namespace Watch.Manager.ApiService.ViewModels;

/// <summary>
/// Request model for providing feedback on classification.
/// </summary>
public sealed class ClassificationFeedbackRequest
{
    /// <summary>
    /// Gets the identifier of the classified article.
    /// </summary>
    public required int ArticleId { get; init; }

    /// <summary>
    /// Gets the identifiers of the correct categories.
    /// </summary>
    public required int[] CorrectCategories { get; init; }

    /// <summary>
    /// Gets the identifiers of the incorrect categories suggested by AI.
    /// </summary>
    public required int[] IncorrectCategories { get; init; }
}
