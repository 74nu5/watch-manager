namespace Watch.Manager.Service.Database;

using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;

using Watch.Manager.Service.Database.Abstractions;
using Watch.Manager.Service.Database.Context;
using Watch.Manager.Service.Database.Entities;
using Watch.Manager.Service.Database.Extensions;

/// <summary>
///     Implementation of the category management service.
/// </summary>
internal sealed class CategoryStore(ILogger<CategoryStore> logger, ArticlesContext context) : ICategoryStore
{
    /// <inheritdoc />
    public async Task<Category[]> GetAllCategoriesAsync(bool includeInactive = false, CancellationToken cancellationToken = default)
    {
        logger.LogInformation("Récupération de toutes les catégories. IncludeInactive: {IncludeInactive}", includeInactive);

        var query = context.Categories
                           .Include(c => c.Parent)
                           .Include(c => c.Children)
                           .AsQueryable();

        if (!includeInactive)
            query = query.Where(c => c.IsActive);

        return await query
                    .OrderBy(c => c.Name)
                    .ToArrayAsync(cancellationToken)
                    .ConfigureAwait(false);
    }

    /// <inheritdoc />
    public async Task<Category?> GetCategoryByIdAsync(int id, CancellationToken cancellationToken = default)
    {
        logger.LogInformation("Récupération de la catégorie avec l'ID: {CategoryId}", id);

        return await context.Categories
                            .Include(c => c.Parent)
                            .Include(c => c.Children)
                            .FirstOrDefaultAsync(c => c.Id == id, cancellationToken)
                            .ConfigureAwait(false);
    }

    /// <inheritdoc />
    public async Task<Category> CreateCategoryAsync(Category category, CancellationToken cancellationToken = default)
    {
        logger.LogInformation("Création d'une nouvelle catégorie: {CategoryName}", category.Name);

        category.CreatedAt = DateTime.UtcNow;
        category.UpdatedAt = DateTime.UtcNow;

        // Calculate hierarchy path and level
        if (category.ParentId.HasValue)
        {
            var allCategories = await context.Categories.ToArrayAsync(cancellationToken).ConfigureAwait(false);
            category.HierarchyPath = category.CalculateHierarchyPath(allCategories);
            category.HierarchyLevel = category.CalculateHierarchyLevel(allCategories);
        }
        else
        {
            category.HierarchyPath = category.Name;
            category.HierarchyLevel = 0;
        }

        _ = context.Categories.Add(category);
        _ = await context.SaveChangesAsync(cancellationToken).ConfigureAwait(false);

        logger.LogInformation("Catégorie créée avec l'ID: {CategoryId}", category.Id);
        return category;
    }

    /// <inheritdoc />
    public async Task<Category> UpdateCategoryAsync(Category category, CancellationToken cancellationToken = default)
    {
        logger.LogInformation("Mise à jour de la catégorie: {CategoryId}", category.Id);

        var oldCategory = await context.Categories.AsNoTracking().FirstOrDefaultAsync(c => c.Id == category.Id, cancellationToken).ConfigureAwait(false);
        var hierarchyChanged = oldCategory != null && (oldCategory.ParentId != category.ParentId || oldCategory.Name != category.Name);

        category.UpdatedAt = DateTime.UtcNow;

        // Recalculate hierarchy if needed
        if (hierarchyChanged)
        {
            var allCategories = await context.Categories.Where(c => c.Id != category.Id).ToListAsync(cancellationToken).ConfigureAwait(false);
            allCategories.Add(category); // Add the updated category

            category.HierarchyPath = category.CalculateHierarchyPath(allCategories);
            category.HierarchyLevel = category.CalculateHierarchyLevel(allCategories);

            // Update descendants if name or parent changed
            category.UpdateDescendantsHierarchyPaths(allCategories);
        }

        _ = context.Categories.Update(category);
        _ = await context.SaveChangesAsync(cancellationToken).ConfigureAwait(false);

        logger.LogInformation("Catégorie mise à jour: {CategoryId}", category.Id);
        return category;
    }

    /// <inheritdoc />
    public async Task<bool> DeleteCategoryAsync(int id, CancellationToken cancellationToken = default)
    {
        logger.LogInformation("Suppression de la catégorie: {CategoryId}", id);

        var category = await context.Categories
                                    .Include(c => c.Children)
                                    .FirstOrDefaultAsync(c => c.Id == id, cancellationToken)
                                    .ConfigureAwait(false);

        if (category == null)
        {
            logger.LogWarning("Catégorie non trouvée pour suppression: {CategoryId}", id);
            return false;
        }

        // Check for subcategories
        if (category.Children.Count != 0)
        {
            logger.LogWarning("Impossible de supprimer la catégorie {CategoryId}: elle contient des sous-catégories", id);
            throw new InvalidOperationException("Impossible de supprimer une catégorie qui contient des sous-catégories.");
        }

        // Remove all article-category relations
        var articleCategories = await context.ArticleCategories
                                             .Where(ac => ac.CategoryId == id)
                                             .ToListAsync(cancellationToken)
                                             .ConfigureAwait(false);

        context.ArticleCategories.RemoveRange(articleCategories);

        // Remove the category
        _ = context.Categories.Remove(category);
        _ = await context.SaveChangesAsync(cancellationToken).ConfigureAwait(false);

        logger.LogInformation("Catégorie supprimée: {CategoryId}", id);
        return true;
    }

    /// <inheritdoc />
    public async Task<IEnumerable<Category>> GetRootCategoriesAsync(bool includeInactive = false, CancellationToken cancellationToken = default)
    {
        logger.LogInformation("Récupération des catégories racines. IncludeInactive: {IncludeInactive}", includeInactive);

        var query = context.Categories
                           .Where(c => c.ParentId == null)
                           .Include(c => c.Children.Where(child => includeInactive || child.IsActive))
                           .AsQueryable();

        if (!includeInactive)
            query = query.Where(c => c.IsActive);

        return await query
                    .OrderBy(c => c.Name)
                    .ToListAsync(cancellationToken)
                    .ConfigureAwait(false);
    }

    /// <inheritdoc />
    public async Task<bool> CategoryExistsAsync(int id, CancellationToken cancellationToken = default)
        => await context.Categories
                        .AnyAsync(c => c.Id == id, cancellationToken)
                        .ConfigureAwait(false);

    /// <inheritdoc />
    public async Task<bool> CategoryNameExistsAsync(string name, int? excludeId = null, CancellationToken cancellationToken = default)
    {
        var query = context.Categories.Where(c => c.Name == name);

        if (excludeId.HasValue)
            query = query.Where(c => c.Id != excludeId.Value);

        return await query
                    .AnyAsync(cancellationToken)
                    .ConfigureAwait(false);
    }

    /// <inheritdoc />
    public async Task<bool> AssignCategoryToArticleAsync(int articleId, int categoryId, bool isManual = true, double? confidenceScore = null, CancellationToken cancellationToken = default)
    {
        logger.LogInformation("Assignation de la catégorie {CategoryId} à l'article {ArticleId}", categoryId, articleId);

        // Check if article exists
        var articleExists = await context.Articles
                                         .AnyAsync(a => a.Id == articleId, cancellationToken)
                                         .ConfigureAwait(false);

        if (!articleExists)
        {
            logger.LogWarning("Article non trouvé: {ArticleId}", articleId);
            return false;
        }

        // Check if category exists
        var categoryExists = await this.CategoryExistsAsync(categoryId, cancellationToken).ConfigureAwait(false);

        if (!categoryExists)
        {
            logger.LogWarning("Catégorie non trouvée: {CategoryId}", categoryId);
            return false;
        }

        // Check if assignment already exists
        var existingAssignment = await context.ArticleCategories
                                              .FirstOrDefaultAsync(ac => ac.ArticleId == articleId && ac.CategoryId == categoryId, cancellationToken)
                                              .ConfigureAwait(false);

        if (existingAssignment != null)
        {
            // Update existing assignment
            existingAssignment.IsManual = isManual;
            existingAssignment.ConfidenceScore = confidenceScore;
            existingAssignment.AssignedAt = DateTime.UtcNow;
        }
        else
        {
            // Create new assignment
            var articleCategory = new ArticleCategory
            {
                ArticleId = articleId,
                CategoryId = categoryId,
                IsManual = isManual,
                ConfidenceScore = confidenceScore,
                AssignedAt = DateTime.UtcNow,
            };

            _ = context.ArticleCategories.Add(articleCategory);
        }

        _ = await context.SaveChangesAsync(cancellationToken).ConfigureAwait(false);
        logger.LogInformation("Assignation réussie: Article {ArticleId} -> Catégorie {CategoryId}", articleId, categoryId);
        return true;
    }

    /// <inheritdoc />
    public async Task<bool> RemoveCategoryFromArticleAsync(int articleId, int categoryId, CancellationToken cancellationToken = default)
    {
        logger.LogInformation("Suppression de l'assignation: Article {ArticleId} -> Catégorie {CategoryId}", articleId, categoryId);

        var articleCategory = await context.ArticleCategories
                                           .FirstOrDefaultAsync(ac => ac.ArticleId == articleId && ac.CategoryId == categoryId, cancellationToken)
                                           .ConfigureAwait(false);

        if (articleCategory == null)
        {
            logger.LogWarning("Assignation non trouvée: Article {ArticleId} -> Catégorie {CategoryId}", articleId, categoryId);
            return false;
        }

        _ = context.ArticleCategories.Remove(articleCategory);
        _ = await context.SaveChangesAsync(cancellationToken).ConfigureAwait(false);

        logger.LogInformation("Assignation supprimée: Article {ArticleId} -> Catégorie {CategoryId}", articleId, categoryId);
        return true;
    }

    /// <inheritdoc />
    public async Task<IEnumerable<Category>> GetArticleCategoriesAsync(int articleId, CancellationToken cancellationToken = default)
    {
        logger.LogInformation("Récupération des catégories de l'article: {ArticleId}", articleId);

        return (await context.ArticleCategories
                             .Where(ac => ac.ArticleId == articleId)
                             .Include(ac => ac.Category)
                             .Select(ac => ac.Category)
                             .Where(c => c != null)
                             .ToListAsync(cancellationToken)
                             .ConfigureAwait(false))!;
    }

    /// <inheritdoc />
    public async Task<int> GetArticleCountInCategoryAsync(int categoryId, bool includeChildren = false, CancellationToken cancellationToken = default)
    {
        logger.LogInformation("Comptage des articles dans la catégorie: {CategoryId}, IncludeChildren: {IncludeChildren}", categoryId, includeChildren);

        if (!includeChildren)
        {
            return await context.ArticleCategories
                                .CountAsync(ac => ac.CategoryId == categoryId, cancellationToken)
                                .ConfigureAwait(false);
        }

        // Get all child category IDs
        var categoryIds = await this.GetCategoryAndChildrenIdsAsync(categoryId, cancellationToken).ConfigureAwait(false);

        return await context.ArticleCategories
                            .CountAsync(ac => categoryIds.Contains(ac.CategoryId), cancellationToken)
                            .ConfigureAwait(false);
    }

    /// <inheritdoc />
    public async Task<string[]> GetLinkedArticleTitlesAsync(int categoryId, CancellationToken cancellationToken = default)
    {
        logger.LogInformation("Récupération des titres d'articles liés à la catégorie: {CategoryId}", categoryId);

        return await context.ArticleCategories
                            .Where(ac => ac.CategoryId == categoryId)
                            .Include(ac => ac.Article)
                            .Select(ac => ac.Article.Title)
                            .ToArrayAsync(cancellationToken)
                            .ConfigureAwait(false);
    }

    /// <inheritdoc />
    public async Task<int> UpdateAllHierarchyPathsAsync(CancellationToken cancellationToken = default)
    {
        logger.LogInformation("Mise à jour de tous les chemins hiérarchiques");

        var allCategories = await context.Categories.ToListAsync(cancellationToken).ConfigureAwait(false);
        var updateCount = 0;

        foreach (var category in allCategories)
        {
            var newPath = category.CalculateHierarchyPath(allCategories);
            var newLevel = category.CalculateHierarchyLevel(allCategories);

            if (category.HierarchyPath != newPath || category.HierarchyLevel != newLevel)
            {
                category.HierarchyPath = newPath;
                category.HierarchyLevel = newLevel;
                category.UpdatedAt = DateTime.UtcNow;
                updateCount++;
            }
        }

        if (updateCount > 0)
        {
            _ = await context.SaveChangesAsync(cancellationToken).ConfigureAwait(false);
            logger.LogInformation("Chemins hiérarchiques mis à jour pour {UpdateCount} catégories", updateCount);
        }

        return updateCount;
    }

    /// <inheritdoc />
    public async Task<Category[]> GetCategoriesAsTreeAsync(bool includeInactive = false, CancellationToken cancellationToken = default)
    {
        logger.LogInformation("Récupération des catégories en arbre hiérarchique. IncludeInactive: {IncludeInactive}", includeInactive);

        var query = context.Categories.AsQueryable()
                           .Include(c => c.Children)
                           .Where(c => c.ParentId == null);

        if (!includeInactive)
            query = query.Where(c => c.IsActive);

        var categories = await query.ToArrayAsync(cancellationToken).ConfigureAwait(false);
        return categories;
    }

    /// <inheritdoc />
    public async Task<Category[]> GetCategoryDescendantsAsync(int categoryId, bool includeInactive = false, CancellationToken cancellationToken = default)
    {
        logger.LogInformation("Récupération des descendants de la catégorie: {CategoryId}", categoryId);

        var allCategories = await this.GetAllCategoriesAsync(includeInactive, cancellationToken).ConfigureAwait(false);
        var category = allCategories.FirstOrDefault(c => c.Id == categoryId);

        if (category == null)
            return [];

        return [.. category.GetAllDescendants(allCategories)];
    }

    /// <inheritdoc />
    public async Task<Category[]> GetCategoryAncestorsAsync(int categoryId, CancellationToken cancellationToken = default)
    {
        logger.LogInformation("Récupération des ancêtres de la catégorie: {CategoryId}", categoryId);

        var allCategories = await this.GetAllCategoriesAsync(true, cancellationToken).ConfigureAwait(false);
        var category = allCategories.FirstOrDefault(c => c.Id == categoryId);

        if (category == null)
            return [];

        return [.. category.GetAncestors(allCategories)];
    }

    /// <inheritdoc />
    public async Task<bool> WouldCreateCircularReferenceAsync(int categoryId, int newParentId, CancellationToken cancellationToken = default)
    {
        logger.LogInformation("Vérification de référence circulaire: catégorie {CategoryId} vers parent {ParentId}", categoryId, newParentId);

        if (categoryId == newParentId)
            return true;

        var allCategories = await this.GetAllCategoriesAsync(true, cancellationToken).ConfigureAwait(false);
        var category = allCategories.FirstOrDefault(c => c.Id == categoryId);
        var newParent = allCategories.FirstOrDefault(c => c.Id == newParentId);

        if (category == null || newParent == null)
            return false;

        // Check if the new parent is a descendant of the current category
        return category.IsAncestorOf(newParent, allCategories);
    }

    /// <inheritdoc />
    public async Task<int> ReorderCategoriesAsync(Dictionary<int, int> categoryOrders, CancellationToken cancellationToken = default)
    {
        logger.LogInformation("Réorganisation de {Count} catégories", categoryOrders.Count);

        var updateCount = 0;

        foreach (var (categoryId, order) in categoryOrders)
        {
            var category = await context.Categories.FindAsync([categoryId], cancellationToken).ConfigureAwait(false);

            if (category != null && category.DisplayOrder != order)
            {
                category.DisplayOrder = order;
                category.UpdatedAt = DateTime.UtcNow;
                updateCount++;
            }
        }

        if (updateCount > 0)
        {
            _ = await context.SaveChangesAsync(cancellationToken).ConfigureAwait(false);
            logger.LogInformation("Ordre mis à jour pour {UpdateCount} catégories", updateCount);
        }

        return updateCount;
    }

    /// <summary>
    ///     Retrieves the ID of a category and all its child category IDs recursively.
    /// </summary>
    /// <param name="categoryId">The parent category ID.</param>
    /// <param name="cancellationToken">Cancellation token.</param>
    /// <returns>List of category IDs.</returns>
    private async Task<List<int>> GetCategoryAndChildrenIdsAsync(int categoryId, CancellationToken cancellationToken)
    {
        var categoryIds = new List<int> { categoryId };
        var children = await context.Categories
                                    .Where(c => c.ParentId == categoryId)
                                    .Select(c => c.Id)
                                    .ToListAsync(cancellationToken)
                                    .ConfigureAwait(false);

        foreach (var childId in children)
        {
            var childIds = await this.GetCategoryAndChildrenIdsAsync(childId, cancellationToken).ConfigureAwait(false);
            categoryIds.AddRange(childIds);
        }

        return categoryIds;
    }
}
