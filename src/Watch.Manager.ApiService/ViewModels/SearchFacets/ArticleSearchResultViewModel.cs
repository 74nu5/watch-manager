namespace Watch.Manager.ApiService.ViewModels.SearchFacets;

/// <summary>
///     View model for the result of an advanced article search.
/// </summary>
public sealed class ArticleSearchResultViewModel
{
    /// <summary>
    ///     Gets articles found matching the search criteria.
    /// </summary>
    public required ArticleViewModel[] Articles { get; init; }

    /// <summary>
    ///     Gets total number of articles matching the filters (without pagination).
    /// </summary>
    public int TotalCount { get; init; }

    /// <summary>
    ///     Gets number of articles returned in this page.
    /// </summary>
    public int Count { get; init; }

    /// <summary>
    ///     Gets offset used for this search.
    /// </summary>
    public int Offset { get; init; }

    /// <summary>
    ///     Gets limit used for this search.
    /// </summary>
    public int Limit { get; init; }

    /// <summary>
    ///     Gets search facets with counts per category.
    /// </summary>
    public SearchFacetsViewModel? Facets { get; init; }
}
