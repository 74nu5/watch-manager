namespace Watch.Manager.Web.Services.Models.Search;

using Watch.Manager.Common.Enumerations;

/// <summary>
///     Parameters for advanced article search.
/// </summary>
public sealed class AdvancedSearchParameters
{
    /// <summary>
    ///     Gets or sets textual search terms.
    /// </summary>
    public string? SearchTerms { get; set; }

    /// <summary>
    ///     Gets or sets tags to include in the search.
    /// </summary>
    public string[]? Tags { get; set; }

    /// <summary>
    ///     Gets or sets authors to filter by.
    /// </summary>
    public string[]? Authors { get; set; }

    /// <summary>
    ///     Gets or sets category IDs to include.
    /// </summary>
    public int[]? CategoryIds { get; set; }

    /// <summary>
    ///     Gets or sets category names to include.
    /// </summary>
    public string[]? CategoryNames { get; set; }

    /// <summary>
    ///     Gets or sets start date for filtering by analysis date.
    /// </summary>
    public DateTime? DateFrom { get; set; }

    /// <summary>
    ///     Gets or sets end date for filtering by analysis date.
    /// </summary>
    public DateTime? DateTo { get; set; }

    /// <summary>
    ///     Gets or sets minimum score for vector search.
    /// </summary>
    public double? MinScore { get; set; }

    /// <summary>
    ///     Gets or sets maximum number of results to return.
    /// </summary>
    public int? Limit { get; set; }

    /// <summary>
    ///     Gets or sets offset for pagination.
    /// </summary>
    public int? Offset { get; set; }

    /// <summary>
    ///     Gets or sets result sorting criterion.
    /// </summary>
    public ArticleSortBy? SortBy { get; set; }

    /// <summary>
    ///     Gets or sets sort order (ascending or descending).
    /// </summary>
    public SortOrder? SortOrder { get; set; }

    /// <summary>
    ///     Gets or sets a value indicating whether whether to include facets in the response.
    /// </summary>
    public bool IncludeFacets { get; set; } = true;
}
