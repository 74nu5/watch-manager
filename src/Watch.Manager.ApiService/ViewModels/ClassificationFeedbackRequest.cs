namespace Watch.Manager.ApiService.ViewModels;

/// <summary>
/// Modèle de requête pour fournir un retour sur la classification
/// </summary>
public sealed class ClassificationFeedbackRequest
{
    /// <summary>
    /// Identifiant de l'article classifié
    /// </summary>
    public required int ArticleId { get; init; }

    /// <summary>
    /// Identifiants des catégories correctes
    /// </summary>
    public required int[] CorrectCategories { get; init; }

    /// <summary>
    /// Identifiants des catégories incorrectes suggérées par l'IA
    /// </summary>
    public required int[] IncorrectCategories { get; init; }
}
