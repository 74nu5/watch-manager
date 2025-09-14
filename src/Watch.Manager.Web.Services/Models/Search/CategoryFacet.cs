namespace Watch.Manager.Web.Services.Models.Search;

/// <summary>
///     Facet representing the number of articles per category.
/// </summary>
public sealed class CategoryFacet
{
    /// <summary>
    ///     Gets or sets the category ID.
    /// </summary>
    public int CategoryId { get; set; }

    /// <summary>
    ///     Gets or sets the name of the category.
    /// </summary>
    public required string CategoryName { get; set; }

    /// <summary>
    ///     Gets or sets the number of articles in this category.
    /// </summary>
    public int Count { get; set; }

    /// <summary>
    ///     Gets or sets the color associated with the category.
    /// </summary>
    public string? Color { get; set; }

    /// <summary>
    ///     Gets or sets the icon associated with the category.
    /// </summary>
    public string? Icon { get; set; }
}
