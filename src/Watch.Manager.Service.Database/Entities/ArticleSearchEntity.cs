namespace Watch.Manager.Service.Database.Entities;

using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text.Json.Serialization;

using Microsoft.Extensions.VectorData;

/// <summary>
///     Represents an article entity used for search and vector storage operations.
/// </summary>
public class ArticleSearchEntity
{
    /// <summary>
    ///     Represents the fixed number of dimensions for a vector.
    /// </summary>
    /// <remarks>
    ///     This constant defines the dimensionality of vectors used in the application.
    ///     It is set to 1536, indicating that all vectors are 1536-dimensional.
    /// </remarks>
    private const int VectorDimensions = 1536;

    /// <summary>
    ///     Represents the distance function used for vector calculations.
    /// </summary>
    /// <remarks>
    ///     This constant specifies the use of the cosine distance function for vector distance
    ///     calculations. It is intended to be used in scenarios where cosine similarity or distance is required.
    /// </remarks>
    private const string VectorDistanceFunction = DistanceFunction.CosineDistance;

    /// <summary>
    ///     Gets or sets the unique identifier for the article.
    /// </summary>
    [VectorStoreKey]
    public int Id { get; set; }

    /// <summary>
    ///     Gets or sets the title of the article.
    /// </summary>
    [StringLength(500)]
    [VectorStoreData]
    [Required]
    public required string Title { get; set; }

    /// <summary>
    ///     Gets or sets the tags associated with the article.
    /// </summary>
    [Required]
    [VectorStoreData]
    public required string Tags { get; set; }

    /// <summary>
    ///     Gets or sets the authors of the article.
    /// </summary>
    [Required]
    [VectorStoreData]
    public required string Authors { get; set; }

    /// <summary>
    ///     Gets or sets the summary of the article.
    /// </summary>
    [Required]
    [VectorStoreData]
    public required string Summary { get; set; }

    /// <summary>
    ///     Gets or sets the URL of the article.
    /// </summary>
    [Required]
    [VectorStoreData]
    public required string Url { get; set; }

    /// <summary>
    ///     Gets or sets the date when the article was analyzed.
    /// </summary>
    [Required]
    [VectorStoreData]
    public DateTime AnalyzeDate { get; set; }

    /// <summary>
    ///     Gets or sets the vector embedding representing the head (main content) of the article.
    /// </summary>
    [VectorStoreVector(VectorDimensions, DistanceFunction = VectorDistanceFunction)]
    [Column(TypeName = "vector(1536)")]
    [JsonIgnore]
    public required float[] EmbeddingHead { get; set; }

    /// <summary>
    ///     Gets or sets the vector embedding representing the body of the article.
    /// </summary>
    [VectorStoreVector(VectorDimensions, DistanceFunction = VectorDistanceFunction)]
    [Column(TypeName = "vector(1536)")]
    [JsonIgnore]
    public required float[] EmbeddingBody { get; set; }

    /// <summary>
    ///     Gets or sets the thumbnail image URL for the article.
    /// </summary>
    [Required]
    [VectorStoreData]
    public required string Thumbnail { get; set; }

    /// <summary>
    ///     Gets or sets the base64-encoded thumbnail image for the article.
    /// </summary>
    [Required]
    [VectorStoreData]
    public required string ThumbnailBase64 { get; set; }
}
