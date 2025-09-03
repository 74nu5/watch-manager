namespace Watch.Manager.Service.Database.Entities;

using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text.Json.Serialization;

/// <summary>
///     Représente un article analysé et stocké dans le système.
/// </summary>
public sealed class Article
{
    /// <summary>
    ///     Identifiant unique de l'article.
    /// </summary>
    public int Id { get; set; }

    /// <summary>
    ///     Titre de l'article.
    /// </summary>
    [StringLength(500)]
    [Required]
    public required string Title { get; set; }

    /// <summary>
    ///     Tags associés à l'article.
    /// </summary>
    [Required]
    public required string[] Tags { get; set; }

    /// <summary>
    ///     Auteurs de l'article.
    /// </summary>
    [Required]
    public required string[] Authors { get; set; }

    /// <summary>
    ///     Résumé de l'article.
    /// </summary>
    [Required]
    [MaxLength(5000)]
    public required string Summary { get; set; }

    /// <summary>
    ///     URL de l'article.
    /// </summary>
    [Required]
    public required Uri Url { get; set; }

    /// <summary>
    ///     Date d'analyse de l'article.
    /// </summary>
    [Required]
    public DateTime AnalyzeDate { get; set; }

    /// <summary>
    ///     Embedding vectoriel de l'en-tête de l'article.
    /// </summary>
    [Column(TypeName = "vector(1536)")]
    [JsonIgnore]
    public required float[] EmbeddingHead { get; set; }

    /// <summary>
    ///     Embedding vectoriel du corps de l'article.
    /// </summary>
    [Column(TypeName = "vector(1536)")]
    [JsonIgnore]
    public required float[] EmbeddingBody { get; set; }

    /// <summary>
    ///     URL de la miniature de l'article.
    /// </summary>
    [Required]
    public required Uri Thumbnail { get; set; }

    /// <summary>
    ///     Miniature de l'article encodée en base64.
    /// </summary>
    [Required]
    // ReSharper disable once EntityFramework.ModelValidation.UnlimitedStringLength
    public required string ThumbnailBase64 { get; set; }

    /// <summary>
    ///     Catégories associées à cet article.
    /// </summary>
    public ICollection<ArticleCategory> ArticleCategories { get; set; } = new List<ArticleCategory>();
}
