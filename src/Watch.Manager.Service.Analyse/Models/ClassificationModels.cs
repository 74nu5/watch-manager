namespace Watch.Manager.Service.Analyse.Models;

/// <summary>
///     Represents a category used for classification.
/// </summary>
public sealed class CategoryForClassification
{
    /// <summary>
    ///     Gets or sets the unique identifier of the category.
    /// </summary>
    public int Id { get; set; }

    /// <summary>
    ///     Gets or sets the name of the category.
    /// </summary>
    public required string Name { get; set; }

    /// <summary>
    ///     Gets or sets the description of the category.
    /// </summary>
    public string? Description { get; set; }

    /// <summary>
    ///     Gets or sets the keywords associated with the category.
    /// </summary>
    public string[] Keywords { get; set; } = [];

    /// <summary>
    ///     Gets or sets the confidence threshold for automatic classification.
    /// </summary>
    public double AutoThreshold { get; set; }

    /// <summary>
    ///     Gets or sets the confidence threshold for manual classification.
    /// </summary>
    public double ManualThreshold { get; set; }

    /// <summary>
    ///     Gets or sets the vector embedding of the category for semantic comparison.
    /// </summary>
    public float[]? Embedding { get; set; }

    /// <summary>
    ///     Gets or sets a value indicating whether the category is active.
    /// </summary>
    public bool IsActive { get; set; } = true;
}

/// <summary>
///     Represents a category suggestion with a confidence score.
/// </summary>
public sealed class CategorySuggestionResult
{
    /// <summary>
    ///     Gets or sets the identifier of the suggested category.
    /// </summary>
    public int CategoryId { get; set; }

    /// <summary>
    ///     Gets or sets the name of the suggested category.
    /// </summary>
    public required string CategoryName { get; set; }

    /// <summary>
    ///     Gets or sets the confidence score between 0 and 1.
    /// </summary>
    public double ConfidenceScore { get; set; }

    /// <summary>
    ///     Gets or sets the reason for the suggestion (matching keywords, semantic similarity, etc.).
    /// </summary>
    public string? Reason { get; set; }

    /// <summary>
    ///     Gets or sets a value indicating whether the suggestion exceeds the automatic threshold.
    /// </summary>
    public bool ExceedsAutoThreshold { get; set; }

    /// <summary>
    ///     Gets or sets a value indicating whether the suggestion exceeds the manual threshold.
    /// </summary>
    public bool ExceedsManualThreshold { get; set; }
}

/// <summary>
///     Represents a suggestion for a new category to be created.
/// </summary>
public sealed class NewCategorySuggestion
{
    /// <summary>
    ///     Gets or sets the suggested name for the new category.
    /// </summary>
    public required string SuggestedName { get; set; }

    /// <summary>
    ///     Gets or sets the suggested description for the new category.
    /// </summary>
    public string? SuggestedDescription { get; set; }

    /// <summary>
    ///     Gets or sets the suggested keywords for the new category.
    /// </summary>
    public string[] SuggestedKeywords { get; set; } = [];

    /// <summary>
    ///     Gets or sets the relevance score of the suggestion between 0 and 1.
    /// </summary>
    public double RelevanceScore { get; set; }

    /// <summary>
    ///     Gets or sets the justification for the new category suggestion.
    /// </summary>
    public string? Justification { get; set; }
}

/// <summary>
///     Represents the results of the automatic classification of an article.
/// </summary>
public sealed class ArticleClassificationResult
{
    /// <summary>
    ///     Gets or sets the identifier of the classified article.
    /// </summary>
    public int ArticleId { get; set; }

    /// <summary>
    ///     Gets or sets the suggestions for existing categories.
    /// </summary>
    public IEnumerable<CategorySuggestionResult> CategorySuggestions { get; set; } = [];

    /// <summary>
    ///     Gets or sets the suggestions for new categories to be created.
    /// </summary>
    public IEnumerable<NewCategorySuggestion> NewCategorySuggestions { get; set; } = [];

    /// <summary>
    ///     Gets or sets the overall confidence score of the classification.
    /// </summary>
    public double OverallConfidence { get; set; }

    /// <summary>
    ///     Gets or sets the timestamp of the classification.
    /// </summary>
    public DateTime ClassificationDate { get; set; } = DateTime.UtcNow;
}
