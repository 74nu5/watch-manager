namespace Watch.Manager.Service.Database.Models.SearchFacets;

/// <summary>
///     Facet representing the number of articles per author.
/// </summary>
public sealed record AuthorFacet
{
    /// <summary>
    ///     Gets the author's name.
    /// </summary>
    public required string AuthorName { get; init; }

    /// <summary>
    ///     Gets the number of articles for this author.
    /// </summary>
    public int Count { get; init; }
}
