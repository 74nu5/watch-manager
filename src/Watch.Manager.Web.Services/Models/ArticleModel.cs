namespace Watch.Manager.Web.Services.Models;

public class ArticleModel
{
    public int Id { get; set; }
    public string Title { get; set; }

    public string[] Tags { get; set; }

    public string[] Authors { get; set; }

    public string Summary { get; set; }

    public Uri Url { get; set; }

    public DateTime AnalyzeDate { get; set; }

    public string TagsJoin => string.Join(",", this.Tags);

    public string AuthorsJoin => string.Join(",", this.Authors);
}
