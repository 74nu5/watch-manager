namespace Watch.Manager.Service.Database.Entities;

using System.ComponentModel.DataAnnotations;

/// <summary>
/// Représente une catégorie pour organiser les articles.
/// </summary>
public sealed class Category
{
    /// <summary>
    /// Identifiant unique de la catégorie.
    /// </summary>
    public int Id { get; set; }

    /// <summary>
    /// Nom de la catégorie.
    /// </summary>
    [Required]
    [StringLength(200)]
    public string Name { get; set; } = string.Empty;

    /// <summary>
    /// Description détaillée de la catégorie.
    /// </summary>
    [StringLength(1000)]
    public string? Description { get; set; }

    /// <summary>
    /// Couleur associée à la catégorie (format hex).
    /// </summary>
    [StringLength(7)]
    public string? Color { get; set; }

    /// <summary>
    /// Icône associée à la catégorie.
    /// </summary>
    [StringLength(50)]
    public string? Icon { get; set; }

    /// <summary>
    /// Mots-clés pour la classification automatique.
    /// </summary>
    public string[] Keywords { get; set; } = [];

    /// <summary>
    /// Identifiant de la catégorie parente (pour la hiérarchie).
    /// </summary>
    public int? ParentId { get; set; }

    /// <summary>
    /// Catégorie parente.
    /// </summary>
    public Category? Parent { get; set; }

    /// <summary>
    /// Sous-catégories.
    /// </summary>
    public ICollection<Category> Children { get; set; } = new List<Category>();

    /// <summary>
    /// Articles associés à cette catégorie.
    /// </summary>
    public ICollection<ArticleCategory> ArticleCategories { get; set; } = new List<ArticleCategory>();

    /// <summary>
    /// Date de création de la catégorie.
    /// </summary>
    [Required]
    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

    /// <summary>
    /// Date de dernière modification.
    /// </summary>
    [Required]
    public DateTime UpdatedAt { get; set; } = DateTime.UtcNow;

    /// <summary>
    /// Indique si la catégorie est active.
    /// </summary>
    [Required]
    public bool IsActive { get; set; } = true;

    /// <summary>
    /// Score de confiance pour la classification automatique.
    /// </summary>
    public double? ConfidenceThreshold { get; set; } = 0.7;
}
