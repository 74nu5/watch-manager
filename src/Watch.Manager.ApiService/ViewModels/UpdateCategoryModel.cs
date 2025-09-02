namespace Watch.Manager.ApiService.ViewModels;

/// <summary>
/// ViewModel pour mettre à jour une catégorie.
/// </summary>
public class UpdateCategoryModel
{
    /// <summary>
    /// Nom de la catégorie.
    /// </summary>
    public string? Name { get; set; }

    /// <summary>
    /// Description de la catégorie.
    /// </summary>
    public string? Description { get; set; }

    /// <summary>
    /// Couleur associée à la catégorie (format hex).
    /// </summary>
    public string? Color { get; set; }

    /// <summary>
    /// Icône associée à la catégorie.
    /// </summary>
    public string? Icon { get; set; }

    /// <summary>
    /// Mots-clés pour la classification automatique.
    /// </summary>
    public string[]? Keywords { get; set; }

    /// <summary>
    /// Identifiant de la catégorie parente.
    /// </summary>
    public int? ParentId { get; set; }

    /// <summary>
    /// Indique si la catégorie est active.
    /// </summary>
    public bool? IsActive { get; set; }

    /// <summary>
    /// Seuil de confiance pour la classification automatique.
    /// </summary>
    public double? ConfidenceThreshold { get; set; }
}
