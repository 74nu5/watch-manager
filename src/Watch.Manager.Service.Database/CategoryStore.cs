namespace Watch.Manager.Service.Database;

using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;

using Watch.Manager.Service.Database.Abstractions;
using Watch.Manager.Service.Database.Context;
using Watch.Manager.Service.Database.Entities;

/// <summary>
/// Implémentation du service de gestion des catégories.
/// </summary>
internal sealed class CategoryStore(ILogger<CategoryStore> logger, ArticlesContext context) : ICategoryStore
{
    /// <inheritdoc />
    public async Task<IEnumerable<Category>> GetAllCategoriesAsync(bool includeInactive = false, CancellationToken cancellationToken = default)
    {
        logger.LogInformation("Récupération de toutes les catégories. IncludeInactive: {IncludeInactive}", includeInactive);

        var query = context.Categories
            .Include(c => c.Parent)
            .Include(c => c.Children)
            .AsQueryable();

        if (!includeInactive)
        {
            query = query.Where(c => c.IsActive);
        }

        return await query
            .OrderBy(c => c.Name)
            .ToListAsync(cancellationToken)
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

        context.Categories.Add(category);
        await context.SaveChangesAsync(cancellationToken).ConfigureAwait(false);

        logger.LogInformation("Catégorie créée avec l'ID: {CategoryId}", category.Id);
        return category;
    }

    /// <inheritdoc />
    public async Task<Category> UpdateCategoryAsync(Category category, CancellationToken cancellationToken = default)
    {
        logger.LogInformation("Mise à jour de la catégorie: {CategoryId}", category.Id);

        category.UpdatedAt = DateTime.UtcNow;
        context.Categories.Update(category);
        await context.SaveChangesAsync(cancellationToken).ConfigureAwait(false);

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

        // Vérifier s'il y a des sous-catégories
        if (category.Children.Any())
        {
            logger.LogWarning("Impossible de supprimer la catégorie {CategoryId}: elle contient des sous-catégories", id);
            throw new InvalidOperationException("Impossible de supprimer une catégorie qui contient des sous-catégories.");
        }

        // Supprimer toutes les relations avec les articles
        var articleCategories = await context.ArticleCategories
            .Where(ac => ac.CategoryId == id)
            .ToListAsync(cancellationToken)
            .ConfigureAwait(false);

        context.ArticleCategories.RemoveRange(articleCategories);

        // Supprimer la catégorie
        context.Categories.Remove(category);
        await context.SaveChangesAsync(cancellationToken).ConfigureAwait(false);

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
        {
            query = query.Where(c => c.IsActive);
        }

        return await query
            .OrderBy(c => c.Name)
            .ToListAsync(cancellationToken)
            .ConfigureAwait(false);
    }

    /// <inheritdoc />
    public async Task<bool> CategoryExistsAsync(int id, CancellationToken cancellationToken = default)
    {
        return await context.Categories
            .AnyAsync(c => c.Id == id, cancellationToken)
            .ConfigureAwait(false);
    }

    /// <inheritdoc />
    public async Task<bool> CategoryNameExistsAsync(string name, int? excludeId = null, CancellationToken cancellationToken = default)
    {
        var query = context.Categories.Where(c => c.Name == name);

        if (excludeId.HasValue)
        {
            query = query.Where(c => c.Id != excludeId.Value);
        }

        return await query
            .AnyAsync(cancellationToken)
            .ConfigureAwait(false);
    }

    /// <inheritdoc />
    public async Task<bool> AssignCategoryToArticleAsync(int articleId, int categoryId, bool isManual = true, double? confidenceScore = null, CancellationToken cancellationToken = default)
    {
        logger.LogInformation("Assignation de la catégorie {CategoryId} à l'article {ArticleId}", categoryId, articleId);

        // Vérifier si l'article existe
        var articleExists = await context.Articles
            .AnyAsync(a => a.Id == articleId, cancellationToken)
            .ConfigureAwait(false);

        if (!articleExists)
        {
            logger.LogWarning("Article non trouvé: {ArticleId}", articleId);
            return false;
        }

        // Vérifier si la catégorie existe
        var categoryExists = await CategoryExistsAsync(categoryId, cancellationToken).ConfigureAwait(false);
        if (!categoryExists)
        {
            logger.LogWarning("Catégorie non trouvée: {CategoryId}", categoryId);
            return false;
        }

        // Vérifier si l'assignation existe déjà
        var existingAssignment = await context.ArticleCategories
            .FirstOrDefaultAsync(ac => ac.ArticleId == articleId && ac.CategoryId == categoryId, cancellationToken)
            .ConfigureAwait(false);

        if (existingAssignment != null)
        {
            // Mettre à jour l'assignation existante
            existingAssignment.IsManual = isManual;
            existingAssignment.ConfidenceScore = confidenceScore;
            existingAssignment.AssignedAt = DateTime.UtcNow;
        }
        else
        {
            // Créer une nouvelle assignation
            var articleCategory = new ArticleCategory
            {
                ArticleId = articleId,
                CategoryId = categoryId,
                IsManual = isManual,
                ConfidenceScore = confidenceScore,
                AssignedAt = DateTime.UtcNow
            };

            context.ArticleCategories.Add(articleCategory);
        }

        await context.SaveChangesAsync(cancellationToken).ConfigureAwait(false);
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

        context.ArticleCategories.Remove(articleCategory);
        await context.SaveChangesAsync(cancellationToken).ConfigureAwait(false);

        logger.LogInformation("Assignation supprimée: Article {ArticleId} -> Catégorie {CategoryId}", articleId, categoryId);
        return true;
    }

    /// <inheritdoc />
    public async Task<IEnumerable<Category>> GetArticleCategoriesAsync(int articleId, CancellationToken cancellationToken = default)
    {
        logger.LogInformation("Récupération des catégories de l'article: {ArticleId}", articleId);

        return await context.ArticleCategories
            .Where(ac => ac.ArticleId == articleId)
            .Include(ac => ac.Category)
            .Select(ac => ac.Category)
            .ToListAsync(cancellationToken)
            .ConfigureAwait(false);
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

        // Récupérer tous les IDs des catégories enfants
        var categoryIds = await GetCategoryAndChildrenIdsAsync(categoryId, cancellationToken).ConfigureAwait(false);

        return await context.ArticleCategories
            .CountAsync(ac => categoryIds.Contains(ac.CategoryId), cancellationToken)
            .ConfigureAwait(false);
    }

    /// <summary>
    /// Récupère l'ID d'une catégorie et tous les IDs de ses sous-catégories de manière récursive.
    /// </summary>
    /// <param name="categoryId">ID de la catégorie parent.</param>
    /// <param name="cancellationToken">Token d'annulation.</param>
    /// <returns>Liste des IDs de catégories.</returns>
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
            var childIds = await GetCategoryAndChildrenIdsAsync(childId, cancellationToken).ConfigureAwait(false);
            categoryIds.AddRange(childIds);
        }

        return categoryIds;
    }
}
