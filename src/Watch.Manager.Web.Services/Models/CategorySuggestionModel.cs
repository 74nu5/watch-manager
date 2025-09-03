namespace Watch.Manager.Web.Services.Models;

/// <summary>
/// Modèle pour représenter une suggestion de catégorie avec un score de confiance.
/// </summary>
public class CategorySuggestionModel
{
    /// <summary>
    /// Identifiant de la catégorie suggérée.
    /// </summary>
    public int CategoryId { get; set; }

    /// <summary>
    /// Nom de la catégorie suggérée.
    /// </summary>
    public required string CategoryName { get; set; }

    /// <summary>
    /// Score de confiance de la suggestion (0.0 à 1.0).
    /// </summary>
    public double ConfidenceScore { get; set; }

    /// <summary>
    /// Raison de la suggestion.
    /// </summary>
    public string? Reason { get; set; }

    /// <summary>
    /// Mots-clés qui ont déclenché cette suggestion.
    /// </summary>
    public string[] MatchedKeywords { get; set; } = [];

    /// <summary>
    /// Indique si cette catégorie a été assignée automatiquement.
    /// </summary>
    public bool IsAutoAssigned { get; set; }
}
