namespace Watch.Manager.Service.Database.Models.SearchFacets;

/// <summary>
///     Search facets with article counts by criteria.
/// </summary>
public sealed record SearchFacets
{
    /// <summary>
    ///     Gets article counts by category.
    /// </summary>
    public CategoryFacet[] Categories { get; internal init; } = [];

    /// <summary>
    ///     Gets article counts by tag.
    /// </summary>
    public TagFacet[] Tags { get; internal init; } = [];

    /// <summary>
    ///     Gets article counts by author.
    /// </summary>
    public AuthorFacet[] Authors { get; internal init; } = [];

    /// <summary>
    ///     Gets article distribution by period.
    /// </summary>
    public DateFacet[] DateDistribution { get; internal init; } = [];
}
