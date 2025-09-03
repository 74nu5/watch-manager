namespace Watch.Manager.Service.Database.Abstractions;

using Watch.Manager.Service.Database.Entities;

/// <summary>
/// Interface pour la gestion des catégories.
/// </summary>
public interface ICategoryStore
{
    /// <summary>
    /// Récupère toutes les catégories.
    /// </summary>
    /// <param name="includeInactive">Inclut les catégories inactives.</param>
    /// <param name="cancellationToken">Token d'annulation.</param>
    /// <returns>Liste des catégories.</returns>
    Task<IEnumerable<Category>> GetAllCategoriesAsync(bool includeInactive = false, CancellationToken cancellationToken = default);

    /// <summary>
    /// Récupère une catégorie par son identifiant.
    /// </summary>
    /// <param name="id">Identifiant de la catégorie.</param>
    /// <param name="cancellationToken">Token d'annulation.</param>
    /// <returns>La catégorie ou null si non trouvée.</returns>
    Task<Category?> GetCategoryByIdAsync(int id, CancellationToken cancellationToken = default);

    /// <summary>
    /// Crée une nouvelle catégorie.
    /// </summary>
    /// <param name="category">La catégorie à créer.</param>
    /// <param name="cancellationToken">Token d'annulation.</param>
    /// <returns>La catégorie créée.</returns>
    Task<Category> CreateCategoryAsync(Category category, CancellationToken cancellationToken = default);

    /// <summary>
    /// Met à jour une catégorie existante.
    /// </summary>
    /// <param name="category">La catégorie à mettre à jour.</param>
    /// <param name="cancellationToken">Token d'annulation.</param>
    /// <returns>La catégorie mise à jour.</returns>
    Task<Category> UpdateCategoryAsync(Category category, CancellationToken cancellationToken = default);

    /// <summary>
    /// Supprime une catégorie.
    /// </summary>
    /// <param name="id">Identifiant de la catégorie à supprimer.</param>
    /// <param name="cancellationToken">Token d'annulation.</param>
    /// <returns>True si la suppression a réussi.</returns>
    Task<bool> DeleteCategoryAsync(int id, CancellationToken cancellationToken = default);

    /// <summary>
    /// Récupère les catégories racines (sans parent).
    /// </summary>
    /// <param name="includeInactive">Inclut les catégories inactives.</param>
    /// <param name="cancellationToken">Token d'annulation.</param>
    /// <returns>Liste des catégories racines avec leurs enfants.</returns>
    Task<IEnumerable<Category>> GetRootCategoriesAsync(bool includeInactive = false, CancellationToken cancellationToken = default);

    /// <summary>
    /// Vérifie si une catégorie existe.
    /// </summary>
    /// <param name="id">Identifiant de la catégorie.</param>
    /// <param name="cancellationToken">Token d'annulation.</param>
    /// <returns>True si la catégorie existe.</returns>
    Task<bool> CategoryExistsAsync(int id, CancellationToken cancellationToken = default);

    /// <summary>
    /// Vérifie si un nom de catégorie existe déjà.
    /// </summary>
    /// <param name="name">Nom de la catégorie.</param>
    /// <param name="excludeId">Identifiant à exclure de la vérification.</param>
    /// <param name="cancellationToken">Token d'annulation.</param>
    /// <returns>True si le nom existe déjà.</returns>
    Task<bool> CategoryNameExistsAsync(string name, int? excludeId = null, CancellationToken cancellationToken = default);

    /// <summary>
    /// Assigne une catégorie à un article.
    /// </summary>
    /// <param name="articleId">Identifiant de l'article.</param>
    /// <param name="categoryId">Identifiant de la catégorie.</param>
    /// <param name="isManual">Indique si l'assignation est manuelle.</param>
    /// <param name="confidenceScore">Score de confiance pour les assignations automatiques.</param>
    /// <param name="cancellationToken">Token d'annulation.</param>
    /// <returns>True si l'assignation a réussi.</returns>
    Task<bool> AssignCategoryToArticleAsync(int articleId, int categoryId, bool isManual = true, double? confidenceScore = null, CancellationToken cancellationToken = default);

    /// <summary>
    /// Retire une catégorie d'un article.
    /// </summary>
    /// <param name="articleId">Identifiant de l'article.</param>
    /// <param name="categoryId">Identifiant de la catégorie.</param>
    /// <param name="cancellationToken">Token d'annulation.</param>
    /// <returns>True si la suppression a réussi.</returns>
    Task<bool> RemoveCategoryFromArticleAsync(int articleId, int categoryId, CancellationToken cancellationToken = default);

    /// <summary>
    /// Récupère les catégories d'un article.
    /// </summary>
    /// <param name="articleId">Identifiant de l'article.</param>
    /// <param name="cancellationToken">Token d'annulation.</param>
    /// <returns>Liste des catégories de l'article.</returns>
    Task<IEnumerable<Category>> GetArticleCategoriesAsync(int articleId, CancellationToken cancellationToken = default);

    /// <summary>
    /// Compte le nombre d'articles dans une catégorie.
    /// </summary>
    /// <param name="categoryId">Identifiant de la catégorie.</param>
    /// <param name="includeChildren">Inclut les articles des sous-catégories.</param>
    /// <param name="cancellationToken">Token d'annulation.</param>
    /// <returns>Nombre d'articles.</returns>
    Task<int> GetArticleCountInCategoryAsync(int categoryId, bool includeChildren = false, CancellationToken cancellationToken = default);

    /// <summary>
    /// Récupère les titres des articles liés à une catégorie.
    /// </summary>
    /// <param name="categoryId">Identifiant de la catégorie.</param>
    /// <param name="cancellationToken">Token d'annulation.</param>
    /// <returns>Liste des titres d'articles.</returns>
    Task<string[]> GetLinkedArticleTitlesAsync(int categoryId, CancellationToken cancellationToken = default);

    /// <summary>
    /// Met à jour les chemins hiérarchiques de toutes les catégories.
    /// </summary>
    /// <param name="cancellationToken">Token d'annulation.</param>
    /// <returns>Nombre de catégories mises à jour.</returns>
    Task<int> UpdateAllHierarchyPathsAsync(CancellationToken cancellationToken = default);

    /// <summary>
    /// Récupère les catégories organisées en arbre hiérarchique.
    /// </summary>
    /// <param name="includeInactive">Inclut les catégories inactives.</param>
    /// <param name="cancellationToken">Token d'annulation.</param>
    /// <returns>Catégories organisées en arbre.</returns>
    Task<IEnumerable<Category>> GetCategoriesAsTreeAsync(bool includeInactive = false, CancellationToken cancellationToken = default);

    /// <summary>
    /// Récupère tous les descendants d'une catégorie.
    /// </summary>
    /// <param name="categoryId">Identifiant de la catégorie parent.</param>
    /// <param name="includeInactive">Inclut les catégories inactives.</param>
    /// <param name="cancellationToken">Token d'annulation.</param>
    /// <returns>Liste des descendants.</returns>
    Task<IEnumerable<Category>> GetCategoryDescendantsAsync(int categoryId, bool includeInactive = false, CancellationToken cancellationToken = default);

    /// <summary>
    /// Récupère les ancêtres d'une catégorie (breadcrumb).
    /// </summary>
    /// <param name="categoryId">Identifiant de la catégorie.</param>
    /// <param name="cancellationToken">Token d'annulation.</param>
    /// <returns>Liste des ancêtres (du plus proche au plus éloigné).</returns>
    Task<IEnumerable<Category>> GetCategoryAncestorsAsync(int categoryId, CancellationToken cancellationToken = default);

    /// <summary>
    /// Vérifie s'il y aurait des références circulaires lors du changement de parent.
    /// </summary>
    /// <param name="categoryId">Identifiant de la catégorie.</param>
    /// <param name="newParentId">Identifiant du nouveau parent.</param>
    /// <param name="cancellationToken">Token d'annulation.</param>
    /// <returns>True s'il y aurait une référence circulaire.</returns>
    Task<bool> WouldCreateCircularReferenceAsync(int categoryId, int newParentId, CancellationToken cancellationToken = default);

    /// <summary>
    /// Réorganise l'ordre d'affichage des catégories.
    /// </summary>
    /// <param name="categoryOrders">Dictionnaire des ordres d'affichage par ID de catégorie.</param>
    /// <param name="cancellationToken">Token d'annulation.</param>
    /// <returns>Nombre de catégories mises à jour.</returns>
    Task<int> ReorderCategoriesAsync(Dictionary<int, int> categoryOrders, CancellationToken cancellationToken = default);
}
