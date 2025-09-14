namespace Watch.Manager.Web.Services.Models.Search;

/// <summary>
///     Represents search facets with article counts per criteria.
/// </summary>
public class SearchFacets
{
    /// <summary>
    ///     Gets or sets the article counts per category.
    /// </summary>
    public CategoryFacet[] Categories { get; set; } = [];

    /// <summary>
    ///     Gets or sets the article counts per tag.
    /// </summary>
    public TagFacet[] Tags { get; set; } = [];

    /// <summary>
    ///     Gets or sets the article counts per author.
    /// </summary>
    public AuthorFacet[] Authors { get; set; } = [];

    /// <summary>
    ///     Gets or sets the article distribution per period.
    /// </summary>
    public DateFacet[] DateDistribution { get; set; } = [];
}
