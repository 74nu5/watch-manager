namespace Watch.Manager.Service.Database.Entities;

using System.ComponentModel.DataAnnotations;

/// <summary>
/// Représente la relation many-to-many entre Article et Category.
/// </summary>
public sealed class ArticleCategory
{
    /// <summary>
    /// Identifiant de l'article.
    /// </summary>
    [Required]
    public int ArticleId { get; set; }

    /// <summary>
    /// Article associé.
    /// </summary>
    public Article Article { get; set; } = null!;

    /// <summary>
    /// Identifiant de la catégorie.
    /// </summary>
    [Required]
    public int CategoryId { get; set; }

    /// <summary>
    /// Catégorie associée.
    /// </summary>
    public Category Category { get; set; } = null!;

    /// <summary>
    /// Score de confiance de la classification (pour les classifications automatiques).
    /// </summary>
    public double? ConfidenceScore { get; set; }

    /// <summary>
    /// Indique si la classification a été faite manuellement ou automatiquement.
    /// </summary>
    [Required]
    public bool IsManual { get; set; } = false;

    /// <summary>
    /// Date d'assignation de la catégorie à l'article.
    /// </summary>
    [Required]
    public DateTime AssignedAt { get; set; } = DateTime.UtcNow;
}
