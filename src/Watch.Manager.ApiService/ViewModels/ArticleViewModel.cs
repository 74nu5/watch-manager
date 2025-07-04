namespace Watch.Manager.ApiService.ViewModels;

public class ArticleViewModel
{
    public int Id { get; set; }
    public string Title { get; set; }

    public string[] Tags { get; set; }

    public string[] Authors { get; set; }

    public string Summary { get; set; }

    public Uri Url { get; set; }

    public DateTime AnalyzeDate { get; set; }

    public Uri Thumbnail { get; set; }
}
