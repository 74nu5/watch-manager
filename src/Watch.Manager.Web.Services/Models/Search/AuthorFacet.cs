namespace Watch.Manager.Web.Services.Models.Search;

/// <summary>
///     Facet representing the number of articles per author.
/// </summary>
public sealed class AuthorFacet
{
    /// <summary>
    ///     Gets or sets the author's name.
    /// </summary>
    public required string AuthorName { get; set; }

    /// <summary>
    ///     Gets or sets the number of articles for this author.
    /// </summary>
    public int Count { get; set; }
}
