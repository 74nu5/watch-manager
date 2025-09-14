namespace Watch.Manager.ApiService.ViewModels.SearchFacets;

/// <summary>
///     View model for search facets.
/// </summary>
public sealed class SearchFacetsViewModel
{
    /// <summary>
    ///     Gets article counts per category.
    /// </summary>
    public CategoryFacetViewModel[] Categories { get; init; } = [];

    /// <summary>
    ///     Gets article counts per tag.
    /// </summary>
    public TagFacetViewModel[] Tags { get; init; } = [];

    /// <summary>
    ///     Gets article counts per author.
    /// </summary>
    public AuthorFacetViewModel[] Authors { get; init; } = [];

    /// <summary>
    ///     Gets distribution by period.
    /// </summary>
    public DateFacetViewModel[] DateDistribution { get; init; } = [];
}
