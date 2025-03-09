namespace Watch.Manager.Service.Analyse.Models;

public class ExtractAnalyseModel
{
    public required string[] Tags { get; set; }

    public required string[] Authors { get; set; }

    public required string Summary { get; set; }

    public string TagsJoin => string.Join(",", this.Tags);

    public string AuthorsJoin => string.Join(",", this.Authors);
}
