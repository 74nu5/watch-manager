namespace Watch.Manager.ApiService.ViewModels;

/// <summary>
/// Model representing the assignment of a category to an article.
/// </summary>
public sealed class CategoryAssignmentModel
{
    /// <summary>
    /// Gets or sets the identifier of the category.
    /// </summary>
    public required int CategoryId { get; set; }

    /// <summary>
    /// Gets or sets the name of the category.
    /// </summary>
    public required string CategoryName { get; set; }

    /// <summary>
    /// Gets or sets the confidence score of the assignment.
    /// </summary>
    public double ConfidenceScore { get; set; }

    /// <summary>
    /// Gets or sets a value indicating whether the assignment was automatic or manual.
    /// </summary>
    public bool IsAutomatic { get; set; }

    /// <summary>
    /// Gets or sets the date of the assignment.
    /// </summary>
    public DateTime AssignedAt { get; set; } = DateTime.UtcNow;
}
