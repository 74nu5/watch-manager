namespace Watch.Manager.ApiService.Extensions;

using Watch.Manager.ApiService.Parameters.Categories;
using Watch.Manager.ApiService.ViewModels;
using Watch.Manager.Service.Database.Entities;

/// <summary>
/// Provides extension methods for mapping category-related endpoints to a <see cref="WebApplication" />.
/// </summary>
/// <remarks>
/// This class defines a set of endpoints for managing categories, including operations such as
/// retrieving, creating, updating, and deleting categories, as well as assigning and removing categories from articles.
/// The endpoints are grouped under the "api/categories" route and support additional operations like fetching root
/// categories and their children.
/// </remarks>
public static class CategoryEndpoints
{
    /// <summary>
    /// Configures the application's endpoints for managing categories, including operations for retrieving, creating,
    /// updating, and deleting categories, as well as assigning and removing categories from articles.
    /// </summary>
    /// <param name="app">The <see cref="WebApplication" /> instance to which the category endpoints will be added.</param>
    internal static void MapCategoryEndpoints(this WebApplication app)
    {
        var vApi = app.NewVersionedApi("Categories");
        var categoriesApi = vApi.MapGroup("api/categories");

        _ = categoriesApi.MapGet("/", GetCategoriesAsync)
                         .WithName("GetAllCategories")
                         .WithSummary("Récupère toutes les catégories");

        _ = categoriesApi.MapGet("/{id:int}", GetCategoryAsync)
                         .WithName("GetCategoryById")
                         .WithSummary("Récupère une catégorie par son ID");

        _ = categoriesApi.MapPost("/", CreateCategoryAsync)
                         .WithName("CreateCategory")
                         .WithSummary("Crée une nouvelle catégorie");

        _ = categoriesApi.MapPut("/{id:int}", UpdateCategoryAsync)
                         .WithName("UpdateCategory")
                         .WithSummary("Met à jour une catégorie");

        _ = categoriesApi.MapDelete("/{id:int}", DeleteCategoryAsync)
                         .WithName("DeleteCategory")
                         .WithSummary("Supprime une catégorie");

        _ = categoriesApi.MapGet("/roots", GetRootsCategoryAsync)
                         .WithName("GetRootCategories")
                         .WithSummary("Récupère les catégories racines avec leurs enfants");

        _ = categoriesApi.MapPost("/{categoryId:int}/articles/{articleId:int}", AssignCategoryToArticleAsync)
                         .WithName("AssignCategoryToArticle")
                         .WithSummary("Assigne une catégorie à un article");

        _ = categoriesApi.MapDelete("/{categoryId:int}/articles/{articleId:int}", RemoveCategoryFromArticleAsync)
                         .WithName("RemoveCategoryFromArticle")
                         .WithSummary("Retire une catégorie d'un article");
    }

    private static async Task<IResult> GetCategoriesAsync([AsParameters] GetCategoriesParameter p)
    {
        var categories = await p.CategoryStore.GetAllCategoriesAsync(p.IncludeInactive, p.CancellationToken).ConfigureAwait(false);
        var viewModels = new List<CategoryViewModel>();

        foreach (var c in categories)
        {
            var articleCount = await p.CategoryStore.GetArticleCountInCategoryAsync(c.Id, false, p.CancellationToken).ConfigureAwait(false);
            var linkedArticles = await p.CategoryStore.GetLinkedArticleTitlesAsync(c.Id, p.CancellationToken).ConfigureAwait(false);
            viewModels.Add(new()
            {
                Id = c.Id,
                Name = c.Name,
                Description = c.Description,
                Color = c.Color,
                Icon = c.Icon,
                Keywords = c.Keywords,
                ParentId = c.ParentId,
                ParentName = c.Parent?.Name,
                Children =
                [
                    .. c.Children.Select(child => new CategoryViewModel
                    {
                        Id = child.Id,
                        Name = child.Name,
                        Description = child.Description,
                        Color = child.Color,
                        Icon = child.Icon,
                        Keywords = child.Keywords,
                        ParentId = child.ParentId,
                        CreatedAt = child.CreatedAt,
                        UpdatedAt = child.UpdatedAt,
                        IsActive = child.IsActive,
                        ConfidenceThreshold = child.ConfidenceThreshold,
                    }),
                ],
                ArticleCount = articleCount,
                LinkedArticles = linkedArticles,
                CreatedAt = c.CreatedAt,
                UpdatedAt = c.UpdatedAt,
                IsActive = c.IsActive,
                ConfidenceThreshold = c.ConfidenceThreshold,
            });
        }

        return Results.Ok(viewModels);
    }

    private static async Task<IResult> CreateCategoryAsync([AsParameters] CreateCategoryParameter p)
    {
        var nameExists = await p.CategoryStore.CategoryNameExistsAsync(p.Model.Name, cancellationToken: p.CancellationToken).ConfigureAwait(false);
        if (nameExists)
            return Results.Conflict($"Une catégorie avec le nom '{p.Model.Name}' existe déjà");

        if (p.Model.ParentId.HasValue)
        {
            var parentExists = await p.CategoryStore.CategoryExistsAsync(p.Model.ParentId.Value, p.CancellationToken).ConfigureAwait(false);
            if (!parentExists)
                return Results.BadRequest($"La catégorie parente avec l'ID {p.Model.ParentId.Value} n'existe pas");
        }

        var category = new Category
        {
            Name = p.Model.Name,
            Description = p.Model.Description,
            Color = p.Model.Color,
            Icon = p.Model.Icon,
            Keywords = p.Model.Keywords,
            ParentId = p.Model.ParentId,
            ConfidenceThreshold = p.Model.ConfidenceThreshold,
        };

        var createdCategory = await p.CategoryStore.CreateCategoryAsync(category, p.CancellationToken).ConfigureAwait(false);
        var viewModel = new CategoryViewModel
        {
            Id = createdCategory.Id,
            Name = createdCategory.Name,
            Description = createdCategory.Description,
            Color = createdCategory.Color,
            Icon = createdCategory.Icon,
            Keywords = createdCategory.Keywords,
            ParentId = createdCategory.ParentId,
            CreatedAt = createdCategory.CreatedAt,
            UpdatedAt = createdCategory.UpdatedAt,
            IsActive = createdCategory.IsActive,
            ConfidenceThreshold = createdCategory.ConfidenceThreshold,
        };

        return Results.Created($"/api/categories/{createdCategory.Id}", viewModel);
    }

    private static async Task<IResult> UpdateCategoryAsync([AsParameters] UpdateCategoryParameter p)
    {
        var category = await p.CategoryStore.GetCategoryByIdAsync(p.Id, p.CancellationToken).ConfigureAwait(false);
        if (category == null)
            return Results.NotFound($"Catégorie avec l'ID {p.Id} non trouvée");

        if (!string.IsNullOrEmpty(p.Model.Name) && p.Model.Name != category.Name)
        {
            var nameExists = await p.CategoryStore.CategoryNameExistsAsync(p.Model.Name, p.Id, p.CancellationToken).ConfigureAwait(false);
            if (nameExists)
                return Results.Conflict($"Une catégorie avec le nom '{p.Model.Name}' existe déjà");

            category.Name = p.Model.Name;
        }

        if (p.Model.ParentId != category.ParentId)
        {
            if (p.Model.ParentId.HasValue)
            {
                var parentExists = await p.CategoryStore.CategoryExistsAsync(p.Model.ParentId.Value, p.CancellationToken).ConfigureAwait(false);
                if (!parentExists)
                    return Results.BadRequest($"La catégorie parente avec l'ID {p.Model.ParentId.Value} n'existe pas");

                if (p.Model.ParentId.Value == p.Id)
                    return Results.BadRequest("Une catégorie ne peut pas être sa propre parente");
            }

            category.ParentId = p.Model.ParentId;
        }

        if (p.Model.Description != null)
            category.Description = p.Model.Description;

        if (p.Model.Color != null)
            category.Color = p.Model.Color;

        if (p.Model.Icon != null)
            category.Icon = p.Model.Icon;

        if (p.Model.Keywords != null)
            category.Keywords = p.Model.Keywords;

        if (p.Model.IsActive.HasValue)
            category.IsActive = p.Model.IsActive.Value;

        if (p.Model.ConfidenceThreshold.HasValue)
            category.ConfidenceThreshold = p.Model.ConfidenceThreshold.Value;

        var updatedCategory = await p.CategoryStore.UpdateCategoryAsync(category, p.CancellationToken).ConfigureAwait(false);
        var viewModel = new CategoryViewModel
        {
            Id = updatedCategory.Id,
            Name = updatedCategory.Name,
            Description = updatedCategory.Description,
            Color = updatedCategory.Color,
            Icon = updatedCategory.Icon,
            Keywords = updatedCategory.Keywords,
            ParentId = updatedCategory.ParentId,
            CreatedAt = updatedCategory.CreatedAt,
            UpdatedAt = updatedCategory.UpdatedAt,
            IsActive = updatedCategory.IsActive,
            ConfidenceThreshold = updatedCategory.ConfidenceThreshold,
        };

        return Results.Ok(viewModel);
    }

    private static async Task<IResult> DeleteCategoryAsync([AsParameters] DeleteCategoryParameter p)
    {
        try
        {
            var deleted = await p.CategoryStore.DeleteCategoryAsync(p.Id, p.CancellationToken).ConfigureAwait(false);
            if (!deleted)
                return Results.NotFound($"Catégorie avec l'ID {p.Id} non trouvée");

            return Results.NoContent();
        }
        catch (InvalidOperationException ex)
        {
            return Results.BadRequest(ex.Message);
        }
    }

    private static async Task<IResult> GetCategoryAsync([AsParameters] GetCategoryParameter p)
    {
        var category = await p.CategoryStore.GetCategoryByIdAsync(p.Id, p.CancellationToken).ConfigureAwait(false);
        if (category == null)
            return Results.NotFound($"Catégorie avec l'ID {p.Id} non trouvée");

        var articleCount = await p.CategoryStore.GetArticleCountInCategoryAsync(p.Id, true, p.CancellationToken).ConfigureAwait(false);
        var linkedArticles = await p.CategoryStore.GetLinkedArticleTitlesAsync(p.Id, p.CancellationToken).ConfigureAwait(false);
        var viewModel = new CategoryViewModel
        {
            Id = category.Id,
            Name = category.Name,
            Description = category.Description,
            Color = category.Color,
            Icon = category.Icon,
            Keywords = category.Keywords,
            ParentId = category.ParentId,
            ParentName = category.Parent?.Name,
            Children =
            [
                .. category.Children.Select(child => new CategoryViewModel
                {
                    Id = child.Id,
                    Name = child.Name,
                    Description = child.Description,
                    Color = child.Color,
                    Icon = child.Icon,
                    Keywords = child.Keywords,
                    ParentId = child.ParentId,
                    CreatedAt = child.CreatedAt,
                    UpdatedAt = child.UpdatedAt,
                    IsActive = child.IsActive,
                    ConfidenceThreshold = child.ConfidenceThreshold,
                }),
            ],
            ArticleCount = articleCount,
            LinkedArticles = linkedArticles,
            CreatedAt = category.CreatedAt,
            UpdatedAt = category.UpdatedAt,
            IsActive = category.IsActive,
            ConfidenceThreshold = category.ConfidenceThreshold,
        };

        return Results.Ok(viewModel);
    }

    private static async Task<IResult> GetRootsCategoryAsync([AsParameters] GetRootsCategoryParameter p)
    {
        var categories = await p.CategoryStore.GetRootCategoriesAsync(p.IncludeInactive, p.CancellationToken).ConfigureAwait(false);
        var viewModels = new List<CategoryViewModel>();

        foreach (var c in categories)
        {
            var articleCount = await p.CategoryStore.GetArticleCountInCategoryAsync(c.Id, false, p.CancellationToken).ConfigureAwait(false);
            var linkedArticles = await p.CategoryStore.GetLinkedArticleTitlesAsync(c.Id, p.CancellationToken).ConfigureAwait(false);
            viewModels.Add(new()
            {
                Id = c.Id,
                Name = c.Name,
                Description = c.Description,
                Color = c.Color,
                Icon = c.Icon,
                Keywords = c.Keywords,
                ParentId = c.ParentId,
                Children =
                [
                    .. c.Children.Select(child => new CategoryViewModel
                    {
                        Id = child.Id,
                        Name = child.Name,
                        Description = child.Description,
                        Color = child.Color,
                        Icon = child.Icon,
                        Keywords = child.Keywords,
                        ParentId = child.ParentId,
                        CreatedAt = child.CreatedAt,
                        UpdatedAt = child.UpdatedAt,
                        IsActive = child.IsActive,
                        ConfidenceThreshold = child.ConfidenceThreshold,
                    }),
                ],
                ArticleCount = articleCount,
                LinkedArticles = linkedArticles,
                CreatedAt = c.CreatedAt,
                UpdatedAt = c.UpdatedAt,
                IsActive = c.IsActive,
                ConfidenceThreshold = c.ConfidenceThreshold,
            });
        }

        return Results.Ok(viewModels);
    }

    private static async Task<IResult> AssignCategoryToArticleAsync([AsParameters] AssignCategoryToArticleParameter p)
    {
        var success = await p.CategoryStore.AssignCategoryToArticleAsync(p.ArticleId, p.CategoryId, true, null, p.CancellationToken).ConfigureAwait(false);
        if (!success)
            return Results.BadRequest("Impossible d'assigner la catégorie à l'article");

        return Results.Ok();
    }

    private static async Task<IResult> RemoveCategoryFromArticleAsync([AsParameters] RemoveCategoryFromArticleParameter p)
    {
        var success = await p.CategoryStore.RemoveCategoryFromArticleAsync(p.ArticleId, p.CategoryId, p.CancellationToken).ConfigureAwait(false);
        if (!success)
            return Results.NotFound("Assignation non trouvée");

        return Results.NoContent();
    }
}
