namespace Watch.Manager.ApiService.ViewModels.SearchFacets;

/// <summary>
///     View model for an author facet.
/// </summary>
public sealed class AuthorFacetViewModel
{
    /// <summary>
    ///     Gets author name.
    /// </summary>
    public required string AuthorName { get; init; }

    /// <summary>
    ///     Gets number of articles by this author.
    /// </summary>
    public int Count { get; init; }
}
