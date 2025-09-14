namespace Watch.Manager.ApiService.ViewModels.SearchFacets;

/// <summary>
///     View model for a category facet.
/// </summary>
public sealed class CategoryFacetViewModel
{
    /// <summary>
    ///     Gets category ID.
    /// </summary>
    public int CategoryId { get; init; }

    /// <summary>
    ///     Gets category name.
    /// </summary>
    public required string CategoryName { get; init; }

    /// <summary>
    ///     Gets number of articles in this category.
    /// </summary>
    public int Count { get; init; }

    /// <summary>
    ///     Gets color associated with the category.
    /// </summary>
    public string? Color { get; init; }

    /// <summary>
    ///     Gets icon associated with the category.
    /// </summary>
    public string? Icon { get; init; }
}
