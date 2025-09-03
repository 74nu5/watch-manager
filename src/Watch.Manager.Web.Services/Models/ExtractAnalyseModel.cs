namespace Watch.Manager.Web.Services.Models;

public sealed record ExtractAnalyseModel
{
    public required string Title { get; set; }

    public required string[] Tags { get; set; }

    public required string[] Authors { get; set; }

    public required string Summary { get; set; }

    public string TagsJoin => string.Join(",", this.Tags);

    public string AuthorsJoin => string.Join(",", this.Authors);
}
