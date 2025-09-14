namespace Watch.Manager.Service.Database.Models.SearchFacets;

/// <summary>
///     Facet representing the number of articles per category.
/// </summary>
public sealed record CategoryFacet
{
    /// <summary>
    ///     Gets the category ID.
    /// </summary>
    public int CategoryId { get; init; }

    /// <summary>
    ///     Gets the category name.
    /// </summary>
    public required string CategoryName { get; init; }

    /// <summary>
    ///     Gets the number of articles in this category.
    /// </summary>
    public int Count { get; init; }

    /// <summary>
    ///     Gets the color associated with the category.
    /// </summary>
    public string? Color { get; init; }

    /// <summary>
    ///     Gets the icon associated with the category.
    /// </summary>
    public string? Icon { get; init; }
}
