namespace Watch.Manager.Service.Database.Models.SearchFacets;

/// <summary>
///     Facet representing the number of articles per tag.
/// </summary>
public sealed record TagFacet
{
    /// <summary>
    ///     Gets tag name.
    /// </summary>
    public required string TagName { get; init; }

    /// <summary>
    ///     Gets number of articles with this tag.
    /// </summary>
    public int Count { get; init; }
}
