namespace Watch.Manager.Service.Analyse.Models;

/// <summary>
/// Résultat de la classification automatique d'un article.
/// </summary>
public sealed class CategoryClassificationResult
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
    /// Score de confiance de la classification (0.0 à 1.0).
    /// </summary>
    public double ConfidenceScore { get; set; }

    /// <summary>
    /// Raison de la classification (mots-clés matchés, similarité sémantique, etc.).
    /// </summary>
    public required string Reason { get; set; }

    /// <summary>
    /// Indique si cette catégorie est recommandée pour une assignation automatique.
    /// </summary>
    public bool IsRecommendedForAutoAssignment { get; set; }
}
