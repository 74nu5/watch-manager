namespace Watch.Manager.Web.Services.Models;

/// <summary>
/// Modèle pour représenter une suggestion de nouvelle catégorie à créer.
/// </summary>
public class NewCategorySuggestionModel
{
    /// <summary>
    /// Nom suggéré pour la nouvelle catégorie.
    /// </summary>
    public required string SuggestedName { get; set; }

    /// <summary>
    /// Description suggérée pour la nouvelle catégorie.
    /// </summary>
    public string? SuggestedDescription { get; set; }

    /// <summary>
    /// Mots-clés suggérés pour la nouvelle catégorie.
    /// </summary>
    public string[] SuggestedKeywords { get; set; } = [];

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
    public int? SuggestedParentId { get; set; }

    /// <summary>
    /// Nom de la catégorie parente suggérée.
    /// </summary>
    public string? SuggestedParentName { get; set; }

    /// <summary>
    /// Couleur suggérée pour la nouvelle catégorie.
    /// </summary>
    public string? SuggestedColor { get; set; }

    /// <summary>
    /// Icône suggérée pour la nouvelle catégorie.
    /// </summary>
    public string? SuggestedIcon { get; set; }
}
