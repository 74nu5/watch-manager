namespace Watch.Manager.ApiService.ViewModels;

/// <summary>
///     Represents an article with metadata, authors, tags, and analysis information.
/// </summary>
public sealed class ArticleViewModel
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
    ///     Gets or sets the score assigned to the article, if any.
    /// </summary>
    public double? Score { get; set; }

    /// <summary>
    ///     Gets or sets the base64-encoded thumbnail image URI, if available.
    /// </summary>
    public Uri? ThumbnailBase64 { get; set; }

    /// <summary>
    ///     Gets or sets the categories assigned to this article.
    /// </summary>
    public string[] Categories { get; set; } = [];
}
