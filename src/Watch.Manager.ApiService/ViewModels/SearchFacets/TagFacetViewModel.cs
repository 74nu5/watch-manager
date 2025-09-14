namespace Watch.Manager.ApiService.ViewModels.SearchFacets;

/// <summary>
///     View model for a tag facet.
/// </summary>
public sealed class TagFacetViewModel
{
    /// <summary>
    ///     Gets tag name.
    /// </summary>
    public required string TagName { get; init; }

    /// <summary>
    ///     Gets number of articles with this tag.
    /// </summary>
    public int Count { get; init; }
}
