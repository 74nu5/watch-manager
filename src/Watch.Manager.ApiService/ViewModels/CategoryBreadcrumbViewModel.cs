namespace Watch.Manager.ApiService.ViewModels;

/// <summary>
/// ViewModel for category breadcrumb navigation.
/// </summary>
public sealed class CategoryBreadcrumbViewModel
{
    /// <summary>
    /// Gets or sets the category identifier.
    /// </summary>
    public int Id { get; set; }

    /// <summary>
    /// Gets or sets the category name.
    /// </summary>
    public required string Name { get; set; }

    /// <summary>
    /// Gets or sets the hierarchy level.
    /// </summary>
    public int Level { get; set; }
}
