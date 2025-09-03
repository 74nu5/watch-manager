namespace Watch.Manager.ApiService.ViewModels;

/// <summary>
/// ViewModel for creating or updating a category.
/// </summary>
public sealed class CreateCategoryModel
{
    /// <summary>
    /// Gets or sets the name of the category.
    /// </summary>
    public required string Name { get; set; }

    /// <summary>
    /// Gets or sets the description of the category.
    /// </summary>
    public string? Description { get; set; }

    /// <summary>
    /// Gets or sets the color associated with the category (hex format).
    /// </summary>
    public string? Color { get; set; }

    /// <summary>
    /// Gets or sets the icon associated with the category.
    /// </summary>
    public string? Icon { get; set; }

    /// <summary>
    /// Gets or sets the keywords for automatic classification.
    /// </summary>
    public string[] Keywords { get; set; } = [];

    /// <summary>
    /// Gets or sets the parent category identifier.
    /// </summary>
    public int? ParentId { get; set; }

    /// <summary>
    /// Gets or sets the confidence threshold for automatic classification.
    /// </summary>
    public double? ConfidenceThreshold { get; set; }

    /// <summary>
    /// Gets or sets a value indicating whether this category inherits properties from its parent.
    /// </summary>
    public bool InheritFromParent { get; set; } = true;

    /// <summary>
    /// Gets or sets the display order in the hierarchy.
    /// </summary>
    public int DisplayOrder { get; set; } = 0;

    /// <summary>
    /// Gets or sets a value indicating whether this category is active.
    /// </summary>
    public bool? IsActive { get; set; }
}
