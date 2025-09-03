namespace Watch.Manager.Web.Services.Models;

/// <summary>
/// Modèle pour représenter une suggestion de nouvelle catégorie.
/// </summary>
public sealed class NewCategorySuggestion
{
    /// <summary>
    /// Nom suggéré pour la nouvelle catégorie.
    /// </summary>
    public required string Name { get; set; }

    /// <summary>
    /// Description suggérée pour la nouvelle catégorie.
    /// </summary>
    public string? Description { get; set; }

    /// <summary>
    /// Mots-clés suggérés pour la nouvelle catégorie.
    /// </summary>
    public string[] Keywords { get; set; } = [];

    /// <summary>
    /// Score de confiance de la suggestion (0.0 à 1.0).
    /// </summary>
    public double ConfidenceScore { get; set; }

    /// <summary>
    /// Raison de la suggestion.
    /// </summary>
    public string? Reason { get; set; }

    /// <summary>
    /// Identifiant de la catégorie parente suggérée.
    /// </summary>
    public int? ParentId { get; set; }

    /// <summary>
    /// Nom de la catégorie parente suggérée.
    /// </summary>
    public string? ParentName { get; set; }

    /// <summary>
    /// Couleur suggérée pour la nouvelle catégorie.
    /// </summary>
    public string? Color { get; set; }

    /// <summary>
    /// Icône suggérée pour la nouvelle catégorie.
    /// </summary>
    public string? Icon { get; set; }

    /// <summary>
    /// Seuil de confiance suggéré.
    /// </summary>
    public double? ConfidenceThreshold { get; set; }
}
