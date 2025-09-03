namespace Watch.Manager.Web.Services.Models;

/// <summary>
/// Modèle interne pour représenter une suggestion de nouvelle catégorie telle que renvoyée par l'API.
/// </summary>
internal class ApiSuggestion
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
    /// Score de pertinence de la suggestion entre 0 et 1.
    /// </summary>
    public double RelevanceScore { get; set; }

    /// <summary>
    /// Justification de la suggestion de nouvelle catégorie.
    /// </summary>
    public string? Justification { get; set; }

    /// <summary>
    /// Couleur suggérée pour la nouvelle catégorie.
    /// </summary>
    public string? SuggestedColor { get; set; }

    /// <summary>
    /// Icône suggérée pour la nouvelle catégorie.
    /// </summary>
    public string? SuggestedIcon { get; set; }

    /// <summary>
    /// Identifiant de la catégorie parente suggérée.
    /// </summary>
    public int? SuggestedParentId { get; set; }

    /// <summary>
    /// Nom de la catégorie parente suggérée.
    /// </summary>
    public string? SuggestedParentName { get; set; }
}
