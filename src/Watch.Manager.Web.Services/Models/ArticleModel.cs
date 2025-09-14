namespace Watch.Manager.Web.Services.Models;

/// <summary>
///     Represents a technical article with metadata, analysis information, and categorization.
/// </summary>
public class ArticleModel
{
    /// <summary>
    ///     Gets or sets the unique identifier of the article.
    /// </summary>
    public int Id { get; set; }

    /// <summary>
    ///     Gets or sets the title of the article.
    /// </summary>
    public required string Title { get; set; }

    /// <summary>
    ///     Gets or sets the tags associated with the article.
    /// </summary>
    public required string[] Tags { get; set; }

    /// <summary>
    ///     Gets or sets the authors of the article.
    /// </summary>
    public required string[] Authors { get; set; }

    /// <summary>
    ///     Gets or sets the summary of the article.
    /// </summary>
    public required string Summary { get; set; }

    /// <summary>
    ///     Gets or sets the URL of the article.
    /// </summary>
    public required Uri Url { get; set; }

    /// <summary>
    ///     Gets or sets the date when the article was analyzed.
    /// </summary>
    public DateTime AnalyzeDate { get; set; }

    /// <summary>
    ///     Gets or sets the thumbnail image URL for the article.
    /// </summary>
    public required Uri Thumbnail { get; set; }

    /// <summary>
    ///     Gets or sets the categories assigned to this article.
    /// </summary>
    public List<string> Categories { get; set; } = [];

    /// <summary>
    ///     Gets or sets the relevance score for search (optional).
    /// </summary>
    public double? Score { get; set; }

    /// <summary>
    ///     Gets the tags as a single comma-separated string.
    /// </summary>
    public string TagsJoin => string.Join(",", this.Tags);

    /// <summary>
    ///     Gets the authors as a single comma-separated string.
    /// </summary>
    public string AuthorsJoin => string.Join(",", this.Authors);

    /// <summary>
    ///     Gets the categories as a single comma-separated string.
    /// </summary>
    public string CategoriesJoin => string.Join(", ", this.Categories);
}
