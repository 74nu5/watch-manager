namespace Watch.Manager.ApiService.ViewModels;

/// <summary>
/// Model representing a suggestion for a new category compatible with the Web API.
/// </summary>
public sealed class NewCategorySuggestion
{
    /// <summary>
    /// Gets or sets the suggested name for the new category.
    /// </summary>
    public required string SuggestedName { get; set; }

    /// <summary>
    /// Gets or sets the suggested description for the new category.
    /// </summary>
    public string? SuggestedDescription { get; set; }

    /// <summary>
    /// Gets or sets the suggested keywords for the new category.
    /// </summary>
    public string[] SuggestedKeywords { get; set; } = [];

    /// <summary>
    /// Gets or sets the relevance score of the suggestion (between 0 and 1).
    /// </summary>
    public double RelevanceScore { get; set; }

    /// <summary>
    /// Gets or sets the justification for the new category suggestion.
    /// </summary>
    public string? Justification { get; set; }

    /// <summary>
    /// Gets or sets the suggested color for the new category.
    /// </summary>
    public string? SuggestedColor { get; set; }

    /// <summary>
    /// Gets or sets the suggested icon for the new category.
    /// </summary>
    public string? SuggestedIcon { get; set; }

    /// <summary>
    /// Gets or sets the suggested parent category identifier.
    /// </summary>
    public int? SuggestedParentId { get; set; }

    /// <summary>
    /// Gets or sets the suggested parent category name.
    /// </summary>
    public string? SuggestedParentName { get; set; }
}
