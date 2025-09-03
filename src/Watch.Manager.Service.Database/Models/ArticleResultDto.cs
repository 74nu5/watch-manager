namespace Watch.Manager.Service.Database.Models;

/// <summary>
/// Data Transfer Object representing the result of an article analysis.
/// </summary>
public sealed record ArticleResultDto
{
    /// <summary>
    /// Gets or sets the unique identifier of the article.
    /// </summary>
    public int Id { get; init; }

    /// <summary>
    /// Gets or sets the title of the article.
    /// </summary>
    public required string Title { get; init; }

    /// <summary>
    /// Gets or sets the tags associated with the article.
    /// </summary>
    public required string[] Tags { get; init; }

    /// <summary>
    /// Gets or sets the authors of the article.
    /// </summary>
    public required string[] Authors { get; init; }

    /// <summary>
    /// Gets or sets the summary of the article.
    /// </summary>
    public required string Summary { get; init; }

    /// <summary>
    /// Gets or sets the URL of the article.
    /// </summary>
    public required Uri Url { get; init; }

    /// <summary>
    /// Gets or sets the date when the article was analyzed.
    /// </summary>
    public DateTime AnalyzeDate { get; init; }

    /// <summary>
    /// Gets or sets the thumbnail image URL of the article.
    /// </summary>
    public required Uri Thumbnail { get; init; }

    /// <summary>
    /// Gets or sets the score assigned to the article, if any.
    /// </summary>
    public double? Score { get; init; }

    /// <summary>
    /// Gets or sets the categories assigned to this article.
    /// </summary>
    public string[] Categories { get; init; } = [];
}
