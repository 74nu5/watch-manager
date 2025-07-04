namespace Watch.Manager.Service.Database.Entities;

using System.ComponentModel.DataAnnotations;
using System.Text.Json.Serialization;

using Pgvector;

public class Article
{
    public int Id { get; set; }

    [StringLength(500)]
    [Required]
    public string Title { get; set; }

    [Required]
    public string[] Tags { get; set; }

    [Required]
    public string[] Authors { get; set; }

    [Required]
    public string Summary { get; set; }

    [Required]
    public Uri Url { get; set; }

    [Required]
    public DateTime AnalyzeDate { get; set; }

    [JsonIgnore]
    public Vector EmbeddingHead { get; set; }

    [JsonIgnore]
    public Vector EmbeddingBody { get; set; }

    [Required]
    public Uri Thumbnail { get; set; }
}
