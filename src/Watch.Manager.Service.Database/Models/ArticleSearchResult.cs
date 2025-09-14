namespace Watch.Manager.Service.Database.Models;

/// <summary>
///     Represents the result of an article search, including pagination metadata.
/// </summary>
public sealed record ArticleSearchResult
{
    /// <summary>
    ///     Gets the articles found that match the search criteria.
    /// </summary>
    public required ArticleResultDto[] Articles { get; init; }

    /// <summary>
    ///     Gets the total number of articles matching the filters (without pagination).
    /// </summary>
    public int TotalCount { get; init; }

    /// <summary>
    ///     Gets the number of articles returned in this page.
    /// </summary>
    public int Count { get; init; }

    /// <summary>
    ///     Gets the offset used for this search.
    /// </summary>
    public int Offset { get; init; }

    /// <summary>
    ///     Gets the limit used for this search.
    /// </summary>
    public int Limit { get; init; }

    /// <summary>
    ///     Gets the search facets with counts by category.
    /// </summary>
    public SearchFacets.SearchFacets? Facets { get; init; }
}
