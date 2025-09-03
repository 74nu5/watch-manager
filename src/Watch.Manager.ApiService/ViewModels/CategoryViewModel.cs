namespace Watch.Manager.ApiService.ViewModels;

/// <summary>
/// ViewModel pour représenter une catégorie dans l'API.
/// </summary>
public class CategoryViewModel
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
    public List<CategoryViewModel> Children { get; set; } = new();

    /// <summary>
    /// Nombre d'articles dans cette catégorie.
    /// </summary>
    public int ArticleCount { get; set; }

    /// <summary>
    /// Noms des articles liés à cette catégorie.
    /// </summary>
    public string[] LinkedArticles { get; set; } = [];

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
}
