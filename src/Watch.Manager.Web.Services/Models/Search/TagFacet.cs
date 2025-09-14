namespace Watch.Manager.Web.Services.Models.Search;

/// <summary>
///     Facet representing the number of articles per tag.
/// </summary>
public sealed class TagFacet
{
    /// <summary>
    ///     Gets or sets the tag name.
    /// </summary>
    public required string TagName { get; set; }

    /// <summary>
    ///     Gets or sets the number of articles with this tag.
    /// </summary>
    public int Count { get; set; }
}
