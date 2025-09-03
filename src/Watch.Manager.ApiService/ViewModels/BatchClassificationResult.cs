namespace Watch.Manager.ApiService.ViewModels;

/// <summary>
/// Modèle pour représenter le résultat de la classification par lot d'un article.
/// </summary>
public class BatchClassificationResult
{
    /// <summary>
    /// Identifiant de l'article classifié.
    /// </summary>
    public required int ArticleId { get; set; }

    /// <summary>
    /// Titre de l'article classifié.
    /// </summary>
    public required string ArticleTitle { get; set; }

    /// <summary>
    /// Catégories assignées automatiquement.
    /// </summary>
    public CategoryAssignmentModel[] AssignedCategories { get; set; } = [];

    /// <summary>
    /// Indique si la classification a réussi.
    /// </summary>
    public bool Success { get; set; }

    /// <summary>
    /// Message d'erreur en cas d'échec.
    /// </summary>
    public string? ErrorMessage { get; set; }
}
