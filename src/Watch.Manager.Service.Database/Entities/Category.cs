namespace Watch.Manager.Service.Database.Entities;

using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text.Json.Serialization;

using Microsoft.Data.SqlTypes;

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
    public ICollection<Category> Children { get; set; } = [];

    /// <summary>
    /// Articles associés à cette catégorie.
    /// </summary>
    public ICollection<ArticleCategory> ArticleCategories { get; set; } = [];

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

    /// <summary>
    /// Embedding vectoriel de la catégorie pour la classification sémantique.
    /// </summary>
    [Column(TypeName = "vector(1536)")]
    [JsonIgnore]
    public SqlVector<float>? Embedding { get; set; }

    /// <summary>
    /// Seuil automatique pour la classification.
    /// </summary>
    public double AutoThreshold { get; set; } = 0.8;

    /// <summary>
    /// Seuil manuel pour la classification.
    /// </summary>
    public double ManualThreshold { get; set; } = 0.6;

    /// <summary>
    /// Indique si cette catégorie hérite des propriétés de son parent.
    /// </summary>
    public bool InheritFromParent { get; set; } = true;

    /// <summary>
    /// Ordre d'affichage dans la hiérarchie.
    /// </summary>
    public int DisplayOrder { get; set; } = 0;

    /// <summary>
    /// Chemin complet dans la hiérarchie (ex: "Parent/Enfant/Sous-enfant").
    /// </summary>
    [StringLength(1000)]
    public string HierarchyPath { get; set; } = string.Empty;

    /// <summary>
    /// Niveau de profondeur dans la hiérarchie (0 = racine).
    /// </summary>
    public int HierarchyLevel { get; set; } = 0;
}
