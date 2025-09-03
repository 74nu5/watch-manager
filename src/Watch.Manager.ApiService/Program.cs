using System.Globalization;
using System.Net;

using Microsoft.AspNetCore.Mvc;

using Watch.Manager.ApiService.Extensions;
using Watch.Manager.ApiService.Parameters;
using Watch.Manager.ApiService.ViewModels;
using Watch.Manager.Service.Analyse.Extensions;
using Watch.Manager.Service.Analyse.Models;
using Watch.Manager.Service.Analyse.Services;
using Watch.Manager.Service.Database.Abstractions;
using Watch.Manager.Service.Database.Entities;
using Watch.Manager.Service.Database.Extensions;
using Watch.Manager.ServiceDefaults;

var builder = WebApplication.CreateBuilder(args);

// Add service defaults & Aspire components.
builder.AddServiceDefaults();

// Add services to the container.
builder.Services.AddProblemDetails();

builder.Services.AddCors(options =>
{
    options.AddPolicy("AllowLocalOrigin",
        builderCors => builderCors.WithOrigins("https://localhost:7020")
                                  .AllowAnyHeader()
                                  .AllowAnyMethod());
    options.AddPolicy("AllowAzureOrigin",
        builderCors => builderCors.WithOrigins("https://app-watch-manager-web-fmc7d9f9cfekg6e6.francecentral-01.azurewebsites.net")
                                  .AllowAnyHeader()
                                  .AllowAnyMethod());
});


builder.Configuration.AddAnalyzeConfiguration();

var withApiVersioning = builder.Services.AddApiVersioning();

builder.AddDefaultOpenApi(withApiVersioning);
builder.AddAnalyzeServices();
builder.AddDatabaseServices();

var app = builder.Build();

app.UseCors("AllowLocalOrigin");
app.UseCors("AllowAzureOrigin");

// Configure the HTTP request pipeline.
app.UseExceptionHandler();
var vApi = app.NewVersionedApi("Articles");
var api = vApi.MapGroup("api/articles");
api.MapPost(
    "/save",
    async ([AsParameters] AnalyzeParameter analyzeParameter, CancellationToken cancellationToken) =>
    {
        try
        {
            var urlAlreadyExists = await analyzeParameter.ArticleAnalyseStore.IsArticleExistsAsync(analyzeParameter.AnalyzeModel.UriToAnalyze, cancellationToken).ConfigureAwait(false);
            if (urlAlreadyExists)
                return Results.Conflict("Url already exists");

            var webSiteSource = await analyzeParameter.WebSiteService.GetWebSiteSource(analyzeParameter.AnalyzeModel.UriToAnalyze, CancellationToken.None).ConfigureAwait(false);
            var embeddingsHead = await analyzeParameter.ExtractEmbeddingAi.GetEmbeddingAsync(webSiteSource.Head, cancellationToken).ConfigureAwait(false);
            var embeddingsBody = await analyzeParameter.ExtractEmbeddingAi.GetEmbeddingAsync(webSiteSource.Body, cancellationToken).ConfigureAwait(false);

            if (embeddingsHead is null || embeddingsBody is null)
                return Results.Problem("Embedding failed");

            var analyzeResult = await analyzeParameter.ExtractDataAi.ExtractDatasAsync(webSiteSource.Body, cancellationToken).ConfigureAwait(false);

            if (analyzeResult is null)
                return Results.Problem("Analyze failed");

            Article article = new()
            {
                Summary = analyzeResult.Summary,
                Authors = analyzeResult.Authors,
                Url = analyzeParameter.AnalyzeModel.UriToAnalyze,
                Thumbnail = webSiteSource.Thumbnail,
                ThumbnailBase64 = webSiteSource.ThumbnailBase64,
                Title = analyzeResult.Title,
                EmbeddingHead = embeddingsHead,
                EmbeddingBody = embeddingsBody,
                Tags = [.. analyzeResult.Tags],
                AnalyzeDate = DateTime.UtcNow,
            };

            await analyzeParameter.ArticleAnalyseStore.StoreArticleAnalyzeAsync(article, cancellationToken).ConfigureAwait(false);

            // Classification automatique après l'ajout de l'article
            if (analyzeParameter.ClassificationService.IsEnabled)
            {
                try
                {
                    // Récupérer les catégories disponibles pour la classification
                    var categories = await analyzeParameter.CategoryStore.GetAllCategoriesAsync(false, cancellationToken).ConfigureAwait(false);
                    var categoryForClassification = categories
                        .Where(c => c.IsActive)
                        .Select(c => new CategoryForClassification
                        {
                            Id = c.Id,
                            Name = c.Name,
                            Description = c.Description,
                            Keywords = c.Keywords ?? [],
                            AutoThreshold = 0.7, // Seuil par défaut pour l'auto-assignation
                            ManualThreshold = 0.5, // Seuil par défaut pour les suggestions
                            IsActive = c.IsActive
                        });

                    // Classification de l'article
                    var articleContent = $"{article.Title}\n\n{article.Summary}";
                    var suggestions = await analyzeParameter.ClassificationService.ClassifyArticleAsync(
                        articleContent,
                        categoryForClassification,
                        cancellationToken).ConfigureAwait(false);

                    // Auto-assigner les catégories qui dépassent le seuil automatique
                    var autoAssignedCategories = suggestions
                        .Where(s => s.ExceedsAutoThreshold)
                        .ToList();

                    foreach (var suggestion in autoAssignedCategories)
                    {
                        await analyzeParameter.CategoryStore.AssignCategoryToArticleAsync(
                            article.Id,
                            suggestion.CategoryId,
                            false, // Pas manuel, c'est automatique
                            suggestion.ConfidenceScore,
                            cancellationToken).ConfigureAwait(false);
                    }

                    analyzeParameter.Logger.LogInformation(
                        "Article {ArticleId} automatiquement classifié avec {Count} catégories",
                        article.Id,
                        autoAssignedCategories.Count);
                }
                catch (Exception classificationException)
                {
                    // Ne pas faire échouer l'ajout d'article si la classification échoue
                    analyzeParameter.Logger.LogWarning(classificationException,
                        "Échec de la classification automatique pour l'article {ArticleId}",
                        article.Id);
                }
            }

            return Results.Ok(analyzeResult);
        }
        catch (HttpRequestException httpRequestException) when(httpRequestException.StatusCode == HttpStatusCode.NotFound)
        {
            analyzeParameter.Logger.LogWarning(httpRequestException, "Url not found");
            return Results.NotFound("Url not found");
        }
        catch (HttpRequestException httpRequestException) when(httpRequestException.StatusCode == HttpStatusCode.Forbidden)
        {
            analyzeParameter.Logger.LogWarning(httpRequestException, "Url is forbidden");
            return Results.StatusCode(403);
        }
        catch (Exception e)
        {
            analyzeParameter.Logger.LogError(e, "Failed to save article");
            return Results.InternalServerError(e);
        }
    });

api.MapGet(
    "/search",
    Handler);

async IAsyncEnumerable<ArticleViewModel> Handler([FromQuery] string? text, [FromQuery] string? tag, [FromServices] IExtractEmbeddingAI extractEmbedding, [FromServices] IArticleAnalyseStore analyseStore, CancellationToken cancellationToken)
{
    await foreach (var article in analyseStore.SearchArticleAsync(text, tag, cancellationToken).ConfigureAwait(false))
    {
        yield return new()
        {
            Id = article.Id,
            Summary = article.Summary,
            Authors = article.Authors,
            Url = article.Url,
            Title = article.Title,
            Tags = article.Tags,
            AnalyzeDate = article.AnalyzeDate,
            Thumbnail = article.Thumbnail,
            Score = article.Score,
            Categories = article.Categories
        };
    }
}

api.MapGet(
    "/thumbnail/{id:int}.png",
    async (HttpContext httpContext, [FromRoute] int id, [FromServices] IArticleAnalyseStore analyseStore, CancellationToken cancellationToken) =>
    {
        var (memoryStream, fileName) = await analyseStore.GetThumbnailAsync(id, cancellationToken).ConfigureAwait(false);
        httpContext.Response.Headers.ContentDisposition = $"inline; filename=\"{fileName ?? "thumbnail.png"}\"";
        return Results.File(memoryStream, contentType: $"image/{Path.GetExtension(fileName)?.Replace(".", string.Empty).ToLower(CultureInfo.CurrentCulture) ?? "png"}");
    });

api.MapGet(
    "tags",
    async (IArticleAnalyseStore articleAnalyseStore, CancellationToken cancellationToken) =>
    {
        var tags = await articleAnalyseStore.GetAllTagsAsync(cancellationToken).ConfigureAwait(false);
        return Results.Ok(tags.Distinct(StringComparer.InvariantCultureIgnoreCase));
    });

// API pour les catégories
var categoriesApi = vApi.MapGroup("api/categories");

categoriesApi.MapGet(
    "/",
    async ([FromServices] ICategoryStore categoryStore, [FromQuery] bool includeInactive = false, CancellationToken cancellationToken = default) =>
    {
        var categories = await categoryStore.GetAllCategoriesAsync(includeInactive, cancellationToken).ConfigureAwait(false);
        var viewModels = new List<CategoryViewModel>();

        foreach (var c in categories)
        {
            var articleCount = await categoryStore.GetArticleCountInCategoryAsync(c.Id, false, cancellationToken).ConfigureAwait(false);
            var linkedArticles = await categoryStore.GetLinkedArticleTitlesAsync(c.Id, cancellationToken).ConfigureAwait(false);

            viewModels.Add(new CategoryViewModel
            {
                Id = c.Id,
                Name = c.Name,
                Description = c.Description,
                Color = c.Color,
                Icon = c.Icon,
                Keywords = c.Keywords,
                ParentId = c.ParentId,
                ParentName = c.Parent?.Name,
                Children = c.Children.Select(child => new CategoryViewModel
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
                    ConfidenceThreshold = child.ConfidenceThreshold
                }).ToList(),
                ArticleCount = articleCount,
                LinkedArticles = linkedArticles,
                CreatedAt = c.CreatedAt,
                UpdatedAt = c.UpdatedAt,
                IsActive = c.IsActive,
                ConfidenceThreshold = c.ConfidenceThreshold
            });
        }

        return Results.Ok(viewModels);
    })
    .WithName("GetAllCategories")
    .WithSummary("Récupère toutes les catégories");

categoriesApi.MapGet(
    "/{id:int}",
    async ([FromRoute] int id, [FromServices] ICategoryStore categoryStore, CancellationToken cancellationToken) =>
    {
        var category = await categoryStore.GetCategoryByIdAsync(id, cancellationToken).ConfigureAwait(false);
        if (category == null)
        {
            return Results.NotFound($"Catégorie avec l'ID {id} non trouvée");
        }

        var articleCount = await categoryStore.GetArticleCountInCategoryAsync(id, true, cancellationToken).ConfigureAwait(false);
        var linkedArticles = await categoryStore.GetLinkedArticleTitlesAsync(id, cancellationToken).ConfigureAwait(false);

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
            Children = category.Children.Select(child => new CategoryViewModel
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
                ConfidenceThreshold = child.ConfidenceThreshold
            }).ToList(),
            ArticleCount = articleCount,
            LinkedArticles = linkedArticles,
            CreatedAt = category.CreatedAt,
            UpdatedAt = category.UpdatedAt,
            IsActive = category.IsActive,
            ConfidenceThreshold = category.ConfidenceThreshold
        };
        return Results.Ok(viewModel);
    })
    .WithName("GetCategoryById")
    .WithSummary("Récupère une catégorie par son ID");

categoriesApi.MapPost(
    "/",
    async ([FromBody] CreateCategoryModel model, [FromServices] ICategoryStore categoryStore, CancellationToken cancellationToken) =>
    {
        // Validation du nom unique
        var nameExists = await categoryStore.CategoryNameExistsAsync(model.Name, cancellationToken: cancellationToken).ConfigureAwait(false);
        if (nameExists)
        {
            return Results.Conflict($"Une catégorie avec le nom '{model.Name}' existe déjà");
        }

        // Validation de la catégorie parente
        if (model.ParentId.HasValue)
        {
            var parentExists = await categoryStore.CategoryExistsAsync(model.ParentId.Value, cancellationToken).ConfigureAwait(false);
            if (!parentExists)
            {
                return Results.BadRequest($"La catégorie parente avec l'ID {model.ParentId.Value} n'existe pas");
            }
        }

        var category = new Category
        {
            Name = model.Name,
            Description = model.Description,
            Color = model.Color,
            Icon = model.Icon,
            Keywords = model.Keywords,
            ParentId = model.ParentId,
            ConfidenceThreshold = model.ConfidenceThreshold
        };

        var createdCategory = await categoryStore.CreateCategoryAsync(category, cancellationToken).ConfigureAwait(false);

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
            ConfidenceThreshold = createdCategory.ConfidenceThreshold
        };

        return Results.Created($"/api/categories/{createdCategory.Id}", viewModel);
    })
    .WithName("CreateCategory")
    .WithSummary("Crée une nouvelle catégorie");

categoriesApi.MapPut(
    "/{id:int}",
    async ([FromRoute] int id, [FromBody] UpdateCategoryModel model, [FromServices] ICategoryStore categoryStore, CancellationToken cancellationToken) =>
    {
        var category = await categoryStore.GetCategoryByIdAsync(id, cancellationToken).ConfigureAwait(false);
        if (category == null)
        {
            return Results.NotFound($"Catégorie avec l'ID {id} non trouvée");
        }

        // Validation du nom unique si modifié
        if (!string.IsNullOrEmpty(model.Name) && model.Name != category.Name)
        {
            var nameExists = await categoryStore.CategoryNameExistsAsync(model.Name, id, cancellationToken).ConfigureAwait(false);
            if (nameExists)
            {
                return Results.Conflict($"Une catégorie avec le nom '{model.Name}' existe déjà");
            }
            category.Name = model.Name;
        }

        // Validation de la catégorie parente si modifiée
        if (model.ParentId != category.ParentId)
        {
            if (model.ParentId.HasValue)
            {
                var parentExists = await categoryStore.CategoryExistsAsync(model.ParentId.Value, cancellationToken).ConfigureAwait(false);
                if (!parentExists)
                {
                    return Results.BadRequest($"La catégorie parente avec l'ID {model.ParentId.Value} n'existe pas");
                }

                // Vérifier qu'on ne crée pas une référence circulaire
                if (model.ParentId.Value == id)
                {
                    return Results.BadRequest("Une catégorie ne peut pas être sa propre parente");
                }
            }
            category.ParentId = model.ParentId;
        }

        // Mise à jour des autres propriétés
        if (model.Description != null) category.Description = model.Description;
        if (model.Color != null) category.Color = model.Color;
        if (model.Icon != null) category.Icon = model.Icon;
        if (model.Keywords != null) category.Keywords = model.Keywords;
        if (model.IsActive.HasValue) category.IsActive = model.IsActive.Value;
        if (model.ConfidenceThreshold.HasValue) category.ConfidenceThreshold = model.ConfidenceThreshold.Value;

        var updatedCategory = await categoryStore.UpdateCategoryAsync(category, cancellationToken).ConfigureAwait(false);

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
            ConfidenceThreshold = updatedCategory.ConfidenceThreshold
        };

        return Results.Ok(viewModel);
    })
    .WithName("UpdateCategory")
    .WithSummary("Met à jour une catégorie");

categoriesApi.MapDelete(
    "/{id:int}",
    async ([FromRoute] int id, [FromServices] ICategoryStore categoryStore, CancellationToken cancellationToken) =>
    {
        try
        {
            var deleted = await categoryStore.DeleteCategoryAsync(id, cancellationToken).ConfigureAwait(false);
            if (!deleted)
            {
                return Results.NotFound($"Catégorie avec l'ID {id} non trouvée");
            }
            return Results.NoContent();
        }
        catch (InvalidOperationException ex)
        {
            return Results.BadRequest(ex.Message);
        }
    })
    .WithName("DeleteCategory")
    .WithSummary("Supprime une catégorie");

categoriesApi.MapGet(
    "/roots",
    async ([FromServices] ICategoryStore categoryStore, [FromQuery] bool includeInactive = false, CancellationToken cancellationToken = default) =>
    {
        var categories = await categoryStore.GetRootCategoriesAsync(includeInactive, cancellationToken).ConfigureAwait(false);
        var viewModels = new List<CategoryViewModel>();

        foreach (var c in categories)
        {
            var articleCount = await categoryStore.GetArticleCountInCategoryAsync(c.Id, false, cancellationToken).ConfigureAwait(false);
            var linkedArticles = await categoryStore.GetLinkedArticleTitlesAsync(c.Id, cancellationToken).ConfigureAwait(false);

            viewModels.Add(new CategoryViewModel
            {
                Id = c.Id,
                Name = c.Name,
                Description = c.Description,
                Color = c.Color,
                Icon = c.Icon,
                Keywords = c.Keywords,
                ParentId = c.ParentId,
                Children = c.Children.Select(child => new CategoryViewModel
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
                    ConfidenceThreshold = child.ConfidenceThreshold
                }).ToList(),
                ArticleCount = articleCount,
                LinkedArticles = linkedArticles,
                CreatedAt = c.CreatedAt,
                UpdatedAt = c.UpdatedAt,
                IsActive = c.IsActive,
                ConfidenceThreshold = c.ConfidenceThreshold
            });
        }

        return Results.Ok(viewModels);
    })
    .WithName("GetRootCategories")
    .WithSummary("Récupère les catégories racines avec leurs enfants");

categoriesApi.MapPost(
    "/{categoryId:int}/articles/{articleId:int}",
    async ([FromRoute] int categoryId, [FromRoute] int articleId, [FromServices] ICategoryStore categoryStore, CancellationToken cancellationToken) =>
    {
        var success = await categoryStore.AssignCategoryToArticleAsync(articleId, categoryId, true, null, cancellationToken).ConfigureAwait(false);
        if (!success)
        {
            return Results.BadRequest("Impossible d'assigner la catégorie à l'article");
        }
        return Results.Ok();
    })
    .WithName("AssignCategoryToArticle")
    .WithSummary("Assigne une catégorie à un article");

categoriesApi.MapDelete(
    "/{categoryId:int}/articles/{articleId:int}",
    async ([FromRoute] int categoryId, [FromRoute] int articleId, [FromServices] ICategoryStore categoryStore, CancellationToken cancellationToken) =>
    {
        var success = await categoryStore.RemoveCategoryFromArticleAsync(articleId, categoryId, cancellationToken).ConfigureAwait(false);
        if (!success)
        {
            return Results.NotFound("Assignation non trouvée");
        }
        return Results.NoContent();
    })
    .WithName("RemoveCategoryFromArticle")
    .WithSummary("Retire une catégorie d'un article");

// Ajouter les endpoints de classification
app.MapClassificationEndpoints();

app.MapDefaultEndpoints();

app.UseDefaultOpenApi();

app.Run();
