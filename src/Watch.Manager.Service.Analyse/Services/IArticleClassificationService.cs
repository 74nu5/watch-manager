namespace Watch.Manager.Service.Analyse.Services;

using Watch.Manager.Service.Analyse.Models;
using Watch.Manager.Service.Database.Entities;

/// <summary>
/// Service de classification automatique des articles par IA.
/// </summary>
public interface IArticleClassificationService
{
    /// <summary>
    /// Classifie automatiquement un article en suggérant des catégories appropriées.
    /// </summary>
    /// <param name="articleId">Identifiant de l'article à classifier.</param>
    /// <param name="options">Options de classification.</param>
    /// <param name="cancellationToken">Token d'annulation.</param>
    /// <returns>Liste des catégories suggérées avec leurs scores de confiance.</returns>
    Task<IReadOnlyList<CategoryClassificationResult>> ClassifyArticleAsync(
        int articleId,
        ClassificationOptions? options = null,
        CancellationToken cancellationToken = default);

    /// <summary>
    /// Classifie automatiquement un article en suggérant des catégories appropriées.
    /// </summary>
    /// <param name="article">Article à classifier.</param>
    /// <param name="options">Options de classification.</param>
    /// <param name="cancellationToken">Token d'annulation.</param>
    /// <returns>Liste des catégories suggérées avec leurs scores de confiance.</returns>
    Task<IReadOnlyList<CategoryClassificationResult>> ClassifyArticleAsync(
        Article article,
        ClassificationOptions? options = null,
        CancellationToken cancellationToken = default);

    /// <summary>
    /// Classifie automatiquement plusieurs articles en lot.
    /// </summary>
    /// <param name="articleIds">Identifiants des articles à classifier.</param>
    /// <param name="options">Options de classification.</param>
    /// <param name="cancellationToken">Token d'annulation.</param>
    /// <returns>Dictionnaire associant chaque article à ses catégories suggérées.</returns>
    Task<IReadOnlyDictionary<int, IReadOnlyList<CategoryClassificationResult>>> ClassifyArticlesBatchAsync(
        IEnumerable<int> articleIds,
        ClassificationOptions? options = null,
        CancellationToken cancellationToken = default);

    /// <summary>
    /// Applique automatiquement les classifications recommandées pour un article.
    /// </summary>
    /// <param name="articleId">Identifiant de l'article.</param>
    /// <param name="options">Options de classification.</param>
    /// <param name="cancellationToken">Token d'annulation.</param>
    /// <returns>Nombre de catégories assignées automatiquement.</returns>
    Task<int> AutoAssignCategoriesAsync(
        int articleId,
        ClassificationOptions? options = null,
        CancellationToken cancellationToken = default);

    /// <summary>
    /// Génère des suggestions de nouvelles catégories basées sur l'analyse des articles non classifiés.
    /// </summary>
    /// <param name="maxSuggestions">Nombre maximum de suggestions à retourner.</param>
    /// <param name="cancellationToken">Token d'annulation.</param>
    /// <returns>Liste des suggestions de nouvelles catégories.</returns>
    Task<IReadOnlyList<CategorySuggestion>> SuggestNewCategoriesAsync(
        int maxSuggestions = 10,
        CancellationToken cancellationToken = default);

    /// <summary>
    /// Améliore les embeddings des catégories basés sur les articles assignés manuellement.
    /// </summary>
    /// <param name="cancellationToken">Token d'annulation.</param>
    /// <returns>Nombre de catégories dont les embeddings ont été mis à jour.</returns>
    Task<int> UpdateCategoryEmbeddingsAsync(CancellationToken cancellationToken = default);

    /// <summary>
    /// Évalue la qualité des classifications existantes et suggère des améliorations.
    /// </summary>
    /// <param name="cancellationToken">Token d'annulation.</param>
    /// <returns>Rapport d'évaluation des classifications.</returns>
    Task<ClassificationQualityReport> EvaluateClassificationQualityAsync(CancellationToken cancellationToken = default);
}

/// <summary>
/// Rapport d'évaluation de la qualité des classifications.
/// </summary>
public sealed class ClassificationQualityReport
{
    /// <summary>
    /// Nombre total d'articles évalués.
    /// </summary>
    public int TotalArticles { get; set; }

    /// <summary>
    /// Nombre d'articles avec classifications automatiques.
    /// </summary>
    public int AutoClassifiedArticles { get; set; }

    /// <summary>
    /// Nombre d'articles avec classifications manuelles.
    /// </summary>
    public int ManuallyClassifiedArticles { get; set; }

    /// <summary>
    /// Score moyen de confiance des classifications automatiques.
    /// </summary>
    public double AverageConfidenceScore { get; set; }

    /// <summary>
    /// Catégories les plus utilisées.
    /// </summary>
    public Dictionary<string, int> TopCategories { get; set; } = [];

    /// <summary>
    /// Articles suggérés pour révision manuelle.
    /// </summary>
    public int[] ArticlesSuggestedForReview { get; set; } = [];

    /// <summary>
    /// Suggestions d'amélioration du système de classification.
    /// </summary>
    public string[] ImprovementSuggestions { get; set; } = [];
}
