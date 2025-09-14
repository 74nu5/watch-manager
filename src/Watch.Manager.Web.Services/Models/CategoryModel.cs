namespace Watch.Manager.Web.Services.Models;

/// <summary>
/// Modèle pour représenter une catégorie dans l'interface web.
/// </summary>
public sealed class CategoryModel
{
    /// <summary>
    /// Identifiant unique de la catégorie.
    /// </summary>
    public int Id { get; set; }

    /// <summary>
    /// Nom de la catégorie.
    /// </summary>
    public required string Name { get; set; }

    /// <summary>
    /// Description de la catégorie.
    /// </summary>
    public string? Description { get; set; }

    /// <summary>
    /// Couleur associée à la catégorie.
    /// </summary>
    public string? Color { get; set; }

    /// <summary>
    /// Icône associée à la catégorie.
    /// </summary>
    public string? Icon { get; set; }

    /// <summary>
    /// Mots-clés pour la classification automatique.
    /// </summary>
    public string[] Keywords { get; set; } = [];

    /// <summary>
    /// Identifiant de la catégorie parente.
    /// </summary>
    public int? ParentId { get; set; }

    /// <summary>
    /// Nom de la catégorie parente.
    /// </summary>
    public string? ParentName { get; set; }

    /// <summary>
    /// Sous-catégories.
    /// </summary>
    public List<CategoryModel> Children { get; set; } = [];

    /// <summary>
    /// Nombre d'articles dans cette catégorie.
    /// </summary>
    public int ArticleCount { get; set; }

    /// <summary>
    /// Liste des articles liés à cette catégorie.
    /// </summary>
    public List<string> LinkedArticles { get; set; } = [];

    /// <summary>
    /// Date de création.
    /// </summary>
    public DateTime CreatedAt { get; set; }

    /// <summary>
    /// Date de dernière modification.
    /// </summary>
    public DateTime UpdatedAt { get; set; }

    /// <summary>
    /// Indique si la catégorie est active.
    /// </summary>
    public bool IsActive { get; set; }

    /// <summary>
    /// Seuil de confiance pour la classification automatique.
    /// </summary>
    public double? ConfidenceThreshold { get; set; }

    /// <summary>
    /// Mots-clés concaténés pour l'affichage.
    /// </summary>
    public string KeywordsJoin => string.Join(", ", this.Keywords);

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
    public string HierarchyPath { get; set; } = string.Empty;

    /// <summary>
    /// Niveau de profondeur dans la hiérarchie (0 = racine).
    /// </summary>
    public int HierarchyLevel { get; set; } = 0;
}
