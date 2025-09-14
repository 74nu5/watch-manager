namespace Watch.Manager.Service.Database.Abstractions;

using Watch.Manager.Service.Database.Entities;

/// <summary>
///     Interface for managing categories.
/// </summary>
public interface ICategoryStore
{
    /// <summary>
    ///     Retrieves all categories.
    /// </summary>
    /// <param name="includeInactive">Whether to include inactive categories.</param>
    /// <param name="cancellationToken">Cancellation token.</param>
    /// <returns>An array of all categories.</returns>
    Task<Category[]> GetAllCategoriesAsync(bool includeInactive = false, CancellationToken cancellationToken = default);

    /// <summary>
    ///     Retrieves a category by its identifier.
    /// </summary>
    /// <param name="id">The category identifier.</param>
    /// <param name="cancellationToken">Cancellation token.</param>
    /// <returns>The category if found; otherwise, null.</returns>
    Task<Category?> GetCategoryByIdAsync(int id, CancellationToken cancellationToken = default);

    /// <summary>
    ///     Creates a new category.
    /// </summary>
    /// <param name="category">The category to create.</param>
    /// <param name="cancellationToken">Cancellation token.</param>
    /// <returns>The created category.</returns>
    Task<Category> CreateCategoryAsync(Category category, CancellationToken cancellationToken = default);

    /// <summary>
    ///     Updates an existing category.
    /// </summary>
    /// <param name="category">The category to update.</param>
    /// <param name="cancellationToken">Cancellation token.</param>
    /// <returns>The updated category.</returns>
    Task<Category> UpdateCategoryAsync(Category category, CancellationToken cancellationToken = default);

    /// <summary>
    ///     Deletes a category.
    /// </summary>
    /// <param name="id">The identifier of the category to delete.</param>
    /// <param name="cancellationToken">Cancellation token.</param>
    /// <returns>True if the deletion succeeded; otherwise, false.</returns>
    Task<bool> DeleteCategoryAsync(int id, CancellationToken cancellationToken = default);

    /// <summary>
    ///     Retrieves root categories (categories without a parent).
    /// </summary>
    /// <param name="includeInactive">Whether to include inactive categories.</param>
    /// <param name="cancellationToken">Cancellation token.</param>
    /// <returns>A collection of root categories with their children.</returns>
    Task<IEnumerable<Category>> GetRootCategoriesAsync(bool includeInactive = false, CancellationToken cancellationToken = default);

    /// <summary>
    ///     Checks if a category exists.
    /// </summary>
    /// <param name="id">The category identifier.</param>
    /// <param name="cancellationToken">Cancellation token.</param>
    /// <returns>True if the category exists; otherwise, false.</returns>
    Task<bool> CategoryExistsAsync(int id, CancellationToken cancellationToken = default);

    /// <summary>
    ///     Checks if a category name already exists.
    /// </summary>
    /// <param name="name">The category name.</param>
    /// <param name="excludeId">An identifier to exclude from the check.</param>
    /// <param name="cancellationToken">Cancellation token.</param>
    /// <returns>True if the name already exists; otherwise, false.</returns>
    Task<bool> CategoryNameExistsAsync(string name, int? excludeId = null, CancellationToken cancellationToken = default);

    /// <summary>
    ///     Assigns a category to an article.
    /// </summary>
    /// <param name="articleId">The article identifier.</param>
    /// <param name="categoryId">The category identifier.</param>
    /// <param name="isManual">Indicates if the assignment is manual.</param>
    /// <param name="confidenceScore">Confidence score for automatic assignments.</param>
    /// <param name="cancellationToken">Cancellation token.</param>
    /// <returns>True if the assignment succeeded; otherwise, false.</returns>
    Task<bool> AssignCategoryToArticleAsync(int articleId, int categoryId, bool isManual = true, double? confidenceScore = null, CancellationToken cancellationToken = default);

    /// <summary>
    ///     Removes a category from an article.
    /// </summary>
    /// <param name="articleId">The article identifier.</param>
    /// <param name="categoryId">The category identifier.</param>
    /// <param name="cancellationToken">Cancellation token.</param>
    /// <returns>True if the removal succeeded; otherwise, false.</returns>
    Task<bool> RemoveCategoryFromArticleAsync(int articleId, int categoryId, CancellationToken cancellationToken = default);

    /// <summary>
    ///     Retrieves the categories assigned to an article.
    /// </summary>
    /// <param name="articleId">The article identifier.</param>
    /// <param name="cancellationToken">Cancellation token.</param>
    /// <returns>A collection of categories assigned to the article.</returns>
    Task<IEnumerable<Category>> GetArticleCategoriesAsync(int articleId, CancellationToken cancellationToken = default);

    /// <summary>
    ///     Counts the number of articles in a category.
    /// </summary>
    /// <param name="categoryId">The category identifier.</param>
    /// <param name="includeChildren">Whether to include articles from subcategories.</param>
    /// <param name="cancellationToken">Cancellation token.</param>
    /// <returns>The number of articles in the category.</returns>
    Task<int> GetArticleCountInCategoryAsync(int categoryId, bool includeChildren = false, CancellationToken cancellationToken = default);

    /// <summary>
    ///     Retrieves the titles of articles linked to a category.
    /// </summary>
    /// <param name="categoryId">The category identifier.</param>
    /// <param name="cancellationToken">Cancellation token.</param>
    /// <returns>An array of article titles.</returns>
    Task<string[]> GetLinkedArticleTitlesAsync(int categoryId, CancellationToken cancellationToken = default);

    /// <summary>
    ///     Updates the hierarchy paths of all categories.
    /// </summary>
    /// <param name="cancellationToken">Cancellation token.</param>
    /// <returns>The number of categories updated.</returns>
    Task<int> UpdateAllHierarchyPathsAsync(CancellationToken cancellationToken = default);

    /// <summary>
    ///     Retrieves categories organized as a hierarchical tree.
    /// </summary>
    /// <param name="includeInactive">Whether to include inactive categories.</param>
    /// <param name="cancellationToken">Cancellation token.</param>
    /// <returns>An array of categories organized as a tree.</returns>
    Task<Category[]> GetCategoriesAsTreeAsync(bool includeInactive = false, CancellationToken cancellationToken = default);

    /// <summary>
    ///     Retrieves all descendants of a category.
    /// </summary>
    /// <param name="categoryId">The parent category identifier.</param>
    /// <param name="includeInactive">Whether to include inactive categories.</param>
    /// <param name="cancellationToken">Cancellation token.</param>
    /// <returns>An array of descendant categories.</returns>
    Task<Category[]> GetCategoryDescendantsAsync(int categoryId, bool includeInactive = false, CancellationToken cancellationToken = default);

    /// <summary>
    ///     Retrieves the ancestors of a category (breadcrumb).
    /// </summary>
    /// <param name="categoryId">The category identifier.</param>
    /// <param name="cancellationToken">Cancellation token.</param>
    /// <returns>An array of ancestor categories, from closest to farthest.</returns>
    Task<Category[]> GetCategoryAncestorsAsync(int categoryId, CancellationToken cancellationToken = default);

    /// <summary>
    ///     Checks if changing the parent would create a circular reference.
    /// </summary>
    /// <param name="categoryId">The category identifier.</param>
    /// <param name="newParentId">The new parent identifier.</param>
    /// <param name="cancellationToken">Cancellation token.</param>
    /// <returns>True if a circular reference would be created; otherwise, false.</returns>
    Task<bool> WouldCreateCircularReferenceAsync(int categoryId, int newParentId, CancellationToken cancellationToken = default);

    /// <summary>
    ///     Reorders the display order of categories.
    /// </summary>
    /// <param name="categoryOrders">A dictionary of display orders by category ID.</param>
    /// <param name="cancellationToken">Cancellation token.</param>
    /// <returns>The number of categories updated.</returns>
    Task<int> ReorderCategoriesAsync(Dictionary<int, int> categoryOrders, CancellationToken cancellationToken = default);
}
