namespace Watch.Manager.Service.Analyse.Models;

/// <summary>
/// Suggestion pour une nouvelle catégorie basée sur l'analyse des articles.
/// </summary>
public class CategorySuggestion
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
    /// Score de pertinence de la suggestion (0.0 à 1.0).
    /// </summary>
    public double RelevanceScore { get; set; }

    /// <summary>
    /// Nombre d'articles qui seraient classés dans cette catégorie.
    /// </summary>
    public int PotentialArticleCount { get; set; }

    /// <summary>
    /// Exemples d'articles qui seraient dans cette catégorie.
    /// </summary>
    public int[] ExampleArticleIds { get; set; } = [];

    /// <summary>
    /// Raison de la suggestion.
    /// </summary>
    public required string Reason { get; set; }
}
