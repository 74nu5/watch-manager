namespace Watch.Manager.Common.Enumerations;

/// <summary>
///     Enumeration of sorting criteria for articles.
/// </summary>
public enum ArticleSortBy
{
    /// <summary>
    ///     Sort by analysis date.
    /// </summary>
    AnalyzeDate,

    /// <summary>
    ///     Sort by title.
    /// </summary>
    Title,

    /// <summary>
    ///     Sort by relevance score.
    /// </summary>
    Score,

    /// <summary>
    ///     Sort by number of categories.
    /// </summary>
    CategoryCount,
}
