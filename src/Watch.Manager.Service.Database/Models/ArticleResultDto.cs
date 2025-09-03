namespace Watch.Manager.Service.Database.Models;

public sealed record ArticleResultDto
{
    public int Id { get; set; }

    public string Title { get; set; }

    public string[] Tags { get; set; }

    public string[] Authors { get; set; }

    public string Summary { get; set; }

    public Uri Url { get; set; }

    public DateTime AnalyzeDate { get; set; }

    public Uri Thumbnail { get; set; }

    public double? Score { get; set; }

    /// <summary>
    /// Catégories assignées à cet article.
    /// </summary>
    public string[] Categories { get; set; } = [];
}
