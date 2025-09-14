namespace Watch.Manager.ApiService.Parameters.Articles;

using Microsoft.AspNetCore.Mvc;

using Watch.Manager.Common.Enumerations;
using Watch.Manager.Service.Database.Abstractions;
using Watch.Manager.Service.Database.Models;

/// <summary>
///     Parameters for advanced article search.
/// </summary>
public sealed class AdvancedSearchArticleParameter
{
    /// <summary>
    ///     Gets store for analyzed articles.
    /// </summary>
    [FromServices]
    public required IArticleAnalyseStore AnalyseStore { get; init; }

    /// <summary>
    ///     Gets textual search terms.
    /// </summary>
    [FromQuery(Name = "q")]
    public string? SearchTerms { get; init; }

    /// <summary>
    ///     Gets tags to filter by (comma-separated).
    /// </summary>
    [FromQuery(Name = "tags")]
    public string? Tags { get; init; }

    /// <summary>
    ///     Gets authors to filter by (comma-separated).
    /// </summary>
    [FromQuery(Name = "authors")]
    public string? Authors { get; init; }

    /// <summary>
    ///     Gets category IDs to filter by (comma-separated).
    /// </summary>
    [FromQuery(Name = "categories")]
    public string? CategoryIds { get; init; }

    /// <summary>
    ///     Gets category names to filter by (comma-separated).
    /// </summary>
    [FromQuery(Name = "categoryNames")]
    public string? CategoryNames { get; init; }

    /// <summary>
    ///     Gets start date (ISO format).
    /// </summary>
    [FromQuery(Name = "dateFrom")]
    public DateTime? DateFrom { get; init; }

    /// <summary>
    ///     Gets end date (ISO format).
    /// </summary>
    [FromQuery(Name = "dateTo")]
    public DateTime? DateTo { get; init; }

    /// <summary>
    ///     Gets minimum score for vector search.
    /// </summary>
    [FromQuery(Name = "minScore")]
    public double? MinScore { get; init; }

    /// <summary>
    ///     Gets maximum number of results to return.
    /// </summary>
    [FromQuery(Name = "limit")]
    public int? Limit { get; init; }

    /// <summary>
    ///     Gets offset for pagination.
    /// </summary>
    [FromQuery(Name = "offset")]
    public int? Offset { get; init; }

    /// <summary>
    ///     Gets sort criterion.
    /// </summary>
    [FromQuery(Name = "sortBy")]
    public ArticleSortBy? SortBy { get; init; }

    /// <summary>
    ///     Gets sort order.
    /// </summary>
    [FromQuery(Name = "sortOrder")]
    public SortOrder? SortOrder { get; init; }

    /// <summary>
    ///     Gets a value indicating whether whether to include facets in the response.
    /// </summary>
    [FromQuery(Name = "includeFacets")]
    public bool IncludeFacets { get; init; } = false;

    /// <summary>
    ///     Gets cancellation token.
    /// </summary>
    public CancellationToken CancellationToken { get; init; }

    /// <summary>
    ///     Converts the parameters to an <see cref="ArticleSearchFilters" /> object.
    /// </summary>
    /// <returns>A configured <see cref="ArticleSearchFilters" /> object.</returns>
    public ArticleSearchFilters ToFilters()
        => new()
        {
            SearchTerms = this.SearchTerms,
            Tags = ParseStringArray(this.Tags),
            Authors = ParseStringArray(this.Authors),
            CategoryIds = ParseIntArray(this.CategoryIds),
            CategoryNames = ParseStringArray(this.CategoryNames),
            DateFrom = this.DateFrom,
            DateTo = this.DateTo,
            MinScore = this.MinScore,
            Limit = this.Limit,
            Offset = this.Offset,
            SortBy = this.SortBy,
            SortOrder = this.SortOrder,
        };

    /// <summary>
    ///     Parses a string into a string array.
    /// </summary>
    /// <param name="input">The input string to parse.</param>
    /// <returns>An array of strings, or null if input is null or empty.</returns>
    private static string[]? ParseStringArray(string? input)
    {
        if (string.IsNullOrWhiteSpace(input))
            return null;

        return input.Split(',', StringSplitOptions.RemoveEmptyEntries | StringSplitOptions.TrimEntries);
    }

    /// <summary>
    ///     Parses a string into an integer array.
    /// </summary>
    /// <param name="input">The input string to parse.</param>
    /// <returns>An array of integers, or null if input is null or empty.</returns>
    private static int[]? ParseIntArray(string? input)
    {
        if (string.IsNullOrWhiteSpace(input))
            return null;

        var parts = input.Split(',', StringSplitOptions.RemoveEmptyEntries | StringSplitOptions.TrimEntries);
        var result = new List<int>();

        foreach (var part in parts)
        {
            if (int.TryParse(part, out var value))
                result.Add(value);
        }

        return result.Count > 0 ? [.. result] : null;
    }
}
