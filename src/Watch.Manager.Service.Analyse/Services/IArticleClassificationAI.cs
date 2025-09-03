namespace Watch.Manager.Service.Analyse.Services;

using Watch.Manager.Service.Analyse.Models;

/// <summary>
/// Interface pour la classification automatique d'articles par IA.
/// </summary>
public interface IArticleClassificationAI
{
    /// <summary>
    /// Indique si le service de classification IA est activé.
    /// </summary>
    bool IsEnabled { get; }

    /// <summary>
    /// Classifie automatiquement un article et suggère des catégories avec des scores de confiance.
    /// </summary>
    /// <param name="articleContent">Contenu de l'article à classifier.</param>
    /// <param name="availableCategories">Catégories disponibles pour la classification.</param>
    /// <param name="cancellationToken">Token d'annulation.</param>
    /// <returns>Liste des suggestions de catégories avec scores de confiance.</returns>
    Task<IEnumerable<CategorySuggestionResult>> ClassifyArticleAsync(
        string articleContent,
        IEnumerable<CategoryForClassification> availableCategories,
        CancellationToken cancellationToken = default);

    /// <summary>
    /// Suggère des nouvelles catégories basées sur le contenu d'un article.
    /// </summary>
    /// <param name="articleContent">Contenu de l'article.</param>
    /// <param name="existingCategories">Catégories existantes pour éviter les doublons.</param>
    /// <param name="cancellationToken">Token d'annulation.</param>
    /// <returns>Liste des suggestions de nouvelles catégories.</returns>
    Task<IEnumerable<NewCategorySuggestion>> SuggestNewCategoriesAsync(
        string articleContent,
        IEnumerable<string> existingCategories,
        CancellationToken cancellationToken = default);

    /// <summary>
    /// Calcule la similarité sémantique entre un article et une catégorie en utilisant les embeddings.
    /// </summary>
    /// <param name="articleEmbedding">Embedding de l'article.</param>
    /// <param name="categoryEmbedding">Embedding de la catégorie.</param>
    /// <returns>Score de similarité entre 0 et 1.</returns>
    double CalculateSemanticSimilarity(float[] articleEmbedding, float[] categoryEmbedding);

    /// <summary>
    /// Améliore les modèles de classification basés sur les corrections manuelles.
    /// </summary>
    /// <param name="articleContent">Contenu de l'article.</param>
    /// <param name="correctCategories">Catégories correctes assignées manuellement.</param>
    /// <param name="incorrectCategories">Catégories incorrectes suggérées par l'IA.</param>
    /// <param name="cancellationToken">Token d'annulation.</param>
    /// <returns>Task représentant l'opération d'apprentissage.</returns>
    Task LearnFromFeedbackAsync(
        string articleContent,
        IEnumerable<int> correctCategories,
        IEnumerable<int> incorrectCategories,
        CancellationToken cancellationToken = default);
}
