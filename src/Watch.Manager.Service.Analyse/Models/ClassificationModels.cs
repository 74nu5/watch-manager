namespace Watch.Manager.Service.Analyse.Models;

/// <summary>
/// Représente une catégorie utilisée pour la classification.
/// </summary>
public sealed class CategoryForClassification
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
    /// Mots-clés associés à la catégorie.
    /// </summary>
    public string[] Keywords { get; set; } = [];

    /// <summary>
    /// Seuil de confiance pour la classification automatique.
    /// </summary>
    public double AutoThreshold { get; set; }

    /// <summary>
    /// Seuil de confiance pour la classification manuelle.
    /// </summary>
    public double ManualThreshold { get; set; }

    /// <summary>
    /// Embedding vectoriel de la catégorie pour la comparaison sémantique.
    /// </summary>
    public float[]? Embedding { get; set; }

    /// <summary>
    /// Indique si la catégorie est active.
    /// </summary>
    public bool IsActive { get; set; } = true;
}

/// <summary>
/// Représente une suggestion de catégorie avec un score de confiance.
/// </summary>
public sealed class CategorySuggestionResult
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
    /// Score de confiance entre 0 et 1.
    /// </summary>
    public double ConfidenceScore { get; set; }

    /// <summary>
    /// Raison de la suggestion (mots-clés correspondants, similarité sémantique, etc.).
    /// </summary>
    public string? Reason { get; set; }

    /// <summary>
    /// Indique si la suggestion dépasse le seuil automatique.
    /// </summary>
    public bool ExceedsAutoThreshold { get; set; }

    /// <summary>
    /// Indique si la suggestion dépasse le seuil manuel.
    /// </summary>
    public bool ExceedsManualThreshold { get; set; }
}

/// <summary>
/// Représente une suggestion de nouvelle catégorie à créer.
/// </summary>
public sealed class NewCategorySuggestion
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
}

/// <summary>
/// Représente les résultats de la classification automatique d'un article.
/// </summary>
public sealed class ArticleClassificationResult
{
    /// <summary>
    /// Identifiant de l'article classifié.
    /// </summary>
    public int ArticleId { get; set; }

    /// <summary>
    /// Suggestions de catégories existantes.
    /// </summary>
    public IEnumerable<CategorySuggestionResult> CategorySuggestions { get; set; } = [];

    /// <summary>
    /// Suggestions de nouvelles catégories à créer.
    /// </summary>
    public IEnumerable<NewCategorySuggestion> NewCategorySuggestions { get; set; } = [];

    /// <summary>
    /// Score de confiance global de la classification.
    /// </summary>
    public double OverallConfidence { get; set; }

    /// <summary>
    /// Horodatage de la classification.
    /// </summary>
    public DateTime ClassificationDate { get; set; } = DateTime.UtcNow;
}
