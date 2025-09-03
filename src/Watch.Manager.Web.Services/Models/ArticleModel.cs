namespace Watch.Manager.Web.Services.Models;

public class ArticleModel
{
    public int Id { get; set; }
    public required string Title { get; set; }

    public required string[] Tags { get; set; }

    public required string[] Authors { get; set; }

    public required string Summary { get; set; }

    public required Uri Url { get; set; }

    public DateTime AnalyzeDate { get; set; }
    public required Uri Thumbnail { get; set; }

    /// <summary>
    /// Catégories assignées à cet article.
    /// </summary>
    public List<string> Categories { get; set; } = new();

    public string TagsJoin => string.Join(",", this.Tags);

    public string AuthorsJoin => string.Join(",", this.Authors);

    public string CategoriesJoin => string.Join(", ", this.Categories);
}
