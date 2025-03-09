namespace Watch.Manager.Service.Database.Entities;

using System.Text.Json.Serialization;

using Pgvector;

public class Article
{
    public int Id { get; set; }

    public string[] Tags { get; set; }

    public string[] Authors { get; set; }

    public string Summary { get; set; }

    public Uri Url { get; set; }

    public DateTime AnalyzeDate { get; set; }

    [JsonIgnore]
    public Vector EmbeddingHead { get; set; }

    [JsonIgnore]
    public Vector EmbeddingBody { get; set; }
}
