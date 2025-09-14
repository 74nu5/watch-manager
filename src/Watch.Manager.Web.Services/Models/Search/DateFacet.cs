namespace Watch.Manager.Web.Services.Models.Search;

/// <summary>
///     Facet representing the temporal distribution of articles.
/// </summary>
public sealed class DateFacet
{
    /// <summary>
    ///     Gets or sets the period (YYYY-MM for monthly, YYYY for yearly).
    /// </summary>
    public required string Period { get; set; }

    /// <summary>
    ///     Gets or sets the number of articles in this period.
    /// </summary>
    public int Count { get; set; }

    /// <summary>
    ///     Gets or sets the start date of the period.
    /// </summary>
    public DateTime PeriodStart { get; set; }

    /// <summary>
    ///     Gets or sets the end date of the period.
    /// </summary>
    public DateTime PeriodEnd { get; set; }
}
