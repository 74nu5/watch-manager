namespace Watch.Manager.ApiService.ViewModels;

/// <summary>
/// Model representing the result of batch classification for an article.
/// </summary>
public sealed class BatchClassificationResult
{
    /// <summary>
    /// Gets or sets the identifier of the classified article.
    /// </summary>
    public required int ArticleId { get; set; }

    /// <summary>
    /// Gets or sets the title of the classified article.
    /// </summary>
    public required string ArticleTitle { get; set; }

    /// <summary>
    /// Gets or sets the categories automatically assigned.
    /// </summary>
    public CategoryAssignmentModel[] AssignedCategories { get; set; } = [];

    /// <summary>
    /// Gets or sets a value indicating whether the classification succeeded.
    /// </summary>
    public bool Success { get; set; }

    /// <summary>
    /// Gets or sets the error message if the classification failed.
    /// </summary>
    public string? ErrorMessage { get; set; }
}
