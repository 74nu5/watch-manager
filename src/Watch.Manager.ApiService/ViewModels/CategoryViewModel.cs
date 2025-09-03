namespace Watch.Manager.ApiService.ViewModels;

/// <summary>
/// ViewModel representing a category in the API.
/// </summary>
public sealed class CategoryViewModel
{
    /// <summary>
    /// Gets or sets the unique identifier of the category.
    /// </summary>
    public int Id { get; set; }

    /// <summary>
    /// Gets or sets the name of the category.
    /// </summary>
    public required string Name { get; set; }

    /// <summary>
    /// Gets or sets the description of the category.
    /// </summary>
    public string? Description { get; set; }

    /// <summary>
    /// Gets or sets the color associated with the category.
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
    /// Gets or sets the name of the parent category.
    /// </summary>
    public string? ParentName { get; set; }

    /// <summary>
    /// Gets or sets the child categories.
    /// </summary>
    public List<CategoryViewModel> Children { get; set; } = new();

    /// <summary>
    /// Gets or sets the number of articles in this category.
    /// </summary>
    public int ArticleCount { get; set; }

    /// <summary>
    /// Gets or sets the names of articles linked to this category.
    /// </summary>
    public string[] LinkedArticles { get; set; } = [];

    /// <summary>
    /// Gets or sets the creation date.
    /// </summary>
    public DateTime CreatedAt { get; set; }

    /// <summary>
    /// Gets or sets the last modification date.
    /// </summary>
    public DateTime UpdatedAt { get; set; }

    /// <summary>
    /// Gets or sets a value indicating whether the category is active.
    /// </summary>
    public bool IsActive { get; set; }

    /// <summary>
    /// Gets or sets the confidence threshold for automatic classification.
    /// </summary>
    public double? ConfidenceThreshold { get; set; }

    /// <summary>
    /// Gets or sets a value indicating whether this category inherits properties from its parent.
    /// </summary>
    public bool InheritFromParent { get; set; }

    /// <summary>
    /// Gets or sets the display order in the hierarchy.
    /// </summary>
    public int DisplayOrder { get; set; }

    /// <summary>
    /// Gets or sets the full hierarchy path (e.g., "Parent/Child/SubChild").
    /// </summary>
    public string HierarchyPath { get; set; } = string.Empty;

    /// <summary>
    /// Gets or sets the hierarchy level (0 = root).
    /// </summary>
    public int HierarchyLevel { get; set; }

    /// <summary>
    /// Gets or sets the breadcrumb navigation path.
    /// </summary>
    public List<CategoryBreadcrumbViewModel> Breadcrumbs { get; set; } = new();
}
