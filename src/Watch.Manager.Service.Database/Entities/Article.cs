namespace Watch.Manager.Service.Database.Entities;

using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text.Json.Serialization;

using Microsoft.Data.SqlTypes;

/// <summary>
///     Represents an analyzed article stored in the system.
/// </summary>
public sealed class Article
{
    /// <summary>
    ///     Gets or sets the unique identifier of the article.
    /// </summary>
    public int Id { get; set; }

    /// <summary>
    ///     Gets or sets the title of the article.
    /// </summary>
    [StringLength(500)]
    [Required]
    public required string Title { get; set; }

    /// <summary>
    ///     Gets or sets the tags associated with the article.
    /// </summary>
    [Required]
    public required string[] Tags { get; set; }

    /// <summary>
    ///     Gets or sets the authors of the article.
    /// </summary>
    [Required]
    public required string[] Authors { get; set; }

    /// <summary>
    ///     Gets or sets the summary of the article.
    /// </summary>
    [Required]
    [MaxLength(5000)]
    public required string Summary { get; set; }

    /// <summary>
    ///     Gets or sets the URL of the article.
    /// </summary>
    [Required]
    public required Uri Url { get; set; }

    /// <summary>
    ///     Gets or sets the analysis date of the article.
    /// </summary>
    [Required]
    public DateTime AnalyzeDate { get; set; }

    /// <summary>
    ///     Gets or sets the vector embedding of the article header for semantic search.
    /// </summary>
    [Column(TypeName = "vector(1536)")]
    [JsonIgnore]
    public required SqlVector<float> EmbeddingHead { get; set; }

    /// <summary>
    ///     Gets or sets the vector embedding of the article body for semantic search.
    /// </summary>
    [Column(TypeName = "vector(1536)")]
    [JsonIgnore]
    public required SqlVector<float> EmbeddingBody { get; set; }

    /// <summary>
    ///     Gets or sets the URL of the article thumbnail.
    /// </summary>
    [Required]
    public required Uri Thumbnail { get; set; }

    /// <summary>
    ///     Gets or sets the article thumbnail encoded in base64 format.
    /// </summary>
    [Required]
    // ReSharper disable once EntityFramework.ModelValidation.UnlimitedStringLength
    public required string ThumbnailBase64 { get; set; }

    /// <summary>
    ///     Gets or sets the categories associated with this article.
    /// </summary>
    public ICollection<ArticleCategory> ArticleCategories { get; set; } = [];
}
