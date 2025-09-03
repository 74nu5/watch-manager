using Watch.Manager.Service.Analyse.Models;

namespace Watch.Manager.ApiService.ViewModels;

/// <summary>
/// Modèle de requête pour la classification en lot d'articles
/// </summary>
public sealed class BatchClassificationRequest
{
    /// <summary>
    /// Identifiants des articles à classifier
    /// </summary>
    public required int[] ArticleIds { get; init; }

    /// <summary>
    /// Options de classification
    /// </summary>
    public ClassificationOptions? Options { get; init; }
}
