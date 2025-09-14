namespace Watch.Manager.Web.Services.Models.Search;

/// <summary>
///     Result of an advanced article search.
/// </summary>
public sealed class AdvancedSearchResult
{
    /// <summary>
    ///     Gets or sets articles found matching the search criteria.
    /// </summary>
    public ArticleModel[] Articles { get; set; } = [];

    /// <summary>
    ///     Gets or sets total number of articles matching the filters (without pagination).
    /// </summary>
    public int TotalCount { get; set; }

    /// <summary>
    ///     Gets or sets number of articles returned in this page.
    /// </summary>
    public int Count { get; set; }

    /// <summary>
    ///     Gets or sets offset used for this search.
    /// </summary>
    public int Offset { get; set; }

    /// <summary>
    ///     Gets or sets limit used for this search.
    /// </summary>
    public int Limit { get; set; }

    /// <summary>
    ///     Gets or sets search facets with counts by category.
    /// </summary>
    public SearchFacets? Facets { get; set; }
}
