namespace Watch.Manager.Service.Database.Models;

using Watch.Manager.Common.Enumerations;

/// <summary>
///     Model representing advanced search filters for articles.
/// </summary>
public sealed record ArticleSearchFilters
{
    /// <summary>
    ///     Gets text search terms (title, summary, content).
    /// </summary>
    public string? SearchTerms { get; init; }

    /// <summary>
    ///     Gets tags to include in the search.
    /// </summary>
    public string[]? Tags { get; init; }

    /// <summary>
    ///     Gets authors to filter by.
    /// </summary>
    public string[]? Authors { get; init; }

    /// <summary>
    ///     Gets category IDs to include.
    /// </summary>
    public int[]? CategoryIds { get; init; }

    /// <summary>
    ///     Gets category names to include (alternative to IDs).
    /// </summary>
    public string[]? CategoryNames { get; init; }

    /// <summary>
    ///     Gets start date for filtering by analysis date.
    /// </summary>
    public DateTime? DateFrom { get; init; }

    /// <summary>
    ///     Gets end date for filtering by analysis date.
    /// </summary>
    public DateTime? DateTo { get; init; }

    /// <summary>
    ///     Gets minimum score for vector search.
    /// </summary>
    public double? MinScore { get; init; }

    /// <summary>
    ///     Gets maximum number of results to return.
    /// </summary>
    public int? Limit { get; init; }

    /// <summary>
    ///     Gets offset for pagination.
    /// </summary>
    public int? Offset { get; init; }

    /// <summary>
    ///     Gets result sorting criteria.
    /// </summary>
    public ArticleSortBy? SortBy { get; init; }

    /// <summary>
    ///     Gets sort order (ascending or descending).
    /// </summary>
    public SortOrder? SortOrder { get; init; }
}
