namespace Watch.Manager.Service.Database.Entities;

using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text.Json.Serialization;

using Microsoft.Extensions.VectorData;

public sealed class Article
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

    [Column(TypeName = "vector(1536)")]
    [JsonIgnore]
    public float[] EmbeddingHead { get; set; }

    [Column(TypeName = "vector(1536)")]
    [JsonIgnore]
    public float[] EmbeddingBody { get; set; }

    [Required]
    public Uri Thumbnail { get; set; }

    [Required]
    public string ThumbnailBase64 { get; set; }
}
