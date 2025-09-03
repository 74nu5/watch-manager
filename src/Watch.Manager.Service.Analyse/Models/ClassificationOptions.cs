namespace Watch.Manager.Service.Analyse.Models;

/// <summary>
/// Options pour la classification automatique.
/// </summary>
public sealed class ClassificationOptions
{
    /// <summary>
    /// Score minimum pour une classification automatique (par défaut 0.7).
    /// </summary>
    public double MinAutoClassificationScore { get; set; } = 0.7;

    /// <summary>
    /// Score minimum pour une suggestion de classification (par défaut 0.5).
    /// </summary>
    public double MinSuggestionScore { get; set; } = 0.5;

    /// <summary>
    /// Nombre maximum de catégories à suggérer par article (par défaut 5).
    /// </summary>
    public int MaxSuggestionsPerArticle { get; set; } = 5;

    /// <summary>
    /// Indique si la classification automatique est activée (par défaut true).
    /// </summary>
    public bool EnableAutoClassification { get; set; } = true;

    /// <summary>
    /// Poids accordé à la correspondance des mots-clés (par défaut 0.3).
    /// </summary>
    public double KeywordMatchWeight { get; set; } = 0.3;

    /// <summary>
    /// Poids accordé à la similarité sémantique (par défaut 0.7).
    /// </summary>
    public double SemanticSimilarityWeight { get; set; } = 0.7;
}
