namespace Watch.Manager.Service.Database.Models.SearchFacets;

/// <summary>
///     Facet representing the temporal distribution of articles.
/// </summary>
public sealed record DateFacet
{
    /// <summary>
    ///     Gets period (YYYY-MM for monthly, YYYY for yearly).
    /// </summary>
    public required string Period { get; init; }

    /// <summary>
    ///     Gets number of articles in this period.
    /// </summary>
    public int Count { get; init; }

    /// <summary>
    ///     Gets start date of the period.
    /// </summary>
    public DateTime PeriodStart { get; init; }

    /// <summary>
    ///     Gets end date of the period.
    /// </summary>
    public DateTime PeriodEnd { get; init; }
}
