namespace Watch.Manager.Web.Services;

using System.Net.Http.Json;

using Watch.Manager.Common;
using Watch.Manager.Web.Services.Models;

using IConfiguration = Microsoft.Extensions.Configuration.IConfiguration;

public class AnalyzeService
{
    private readonly IConfiguration configuration;

    private readonly HttpClient client;

    public AnalyzeService(IConfiguration configuration, HttpClient client)
    {
        this.configuration = configuration;
        this.client = client;
    }

    //public async Task<ExtractAnalyseModel?> AnalyzeArticleAsync(string url, CancellationToken cancellationToken = default)
    //{
    //    var response = await this.client.PostAsJsonAsync("/analyze", new { url }, cancellationToken).ConfigureAwait(false);
    //    _ = response.EnsureSuccessStatusCode();
    //    return await response.Content.ReadFromJsonAsync<ExtractAnalyseModel>(cancellationToken).ConfigureAwait(false);
    //}

    public async Task<ApiResult<ExtractAnalyseModel>>SaveArticleAsync(string url, CancellationToken cancellationToken = default)
    {
        var response = await this.client.PostAsJsonAsync("/api/articles/save", new { UriToAnalyze = url }, cancellationToken).ConfigureAwait(false);

        if (!response.IsSuccessStatusCode)
        {
            return response.StatusCode switch
            {
                System.Net.HttpStatusCode.Conflict => ApiResult<ExtractAnalyseModel>.Failure(ApiResultErrorType.Conflict),
                System.Net.HttpStatusCode.NotFound => ApiResult<ExtractAnalyseModel>.Failure(ApiResultErrorType.NotFound),
                System.Net.HttpStatusCode.Forbidden => ApiResult<ExtractAnalyseModel>.Failure(ApiResultErrorType.Forbidden),
                _ => ApiResult<ExtractAnalyseModel>.Failure("Failed to save article")
            };
        }

        var analyseModel = await response.Content.ReadFromJsonAsync<ExtractAnalyseModel>(cancellationToken).ConfigureAwait(false);

        if (analyseModel is null)
            return ApiResult<ExtractAnalyseModel>.Failure("Failed to parse response");

        return ApiResult<ExtractAnalyseModel>.Success(analyseModel);
    }

    public async Task<ArticleModel[]> SearchArticleAsync(string text, string? tag, CancellationToken cancellationToken = default)
    {
        var response = await this.client.GetAsync($"/api/articles/search?text={text}&tag={tag}", cancellationToken).ConfigureAwait(false);
        _ = response.EnsureSuccessStatusCode();
        return await response.Content.ReadFromJsonAsync<ArticleModel[]>(cancellationToken).ConfigureAwait(false) ?? [];
    }

    //public async Task<string> GetAnalysisResultAsync(CancellationToken cancellationToken = default)
    //{
    //    var response = await this.client.GetAsync("/analyze", cancellationToken).ConfigureAwait(false);
    //    _ = response.EnsureSuccessStatusCode();
    //    return await response.Content.ReadAsStringAsync(cancellationToken).ConfigureAwait(false);
    //}

    //public async Task Analyse(string url, CancellationToken cancellationToken = default)
    //{
    //    var contentSite = await this.ExtractTextFromUrlAsync(url, cancellationToken).ConfigureAwait(false);
    //}

    //private async Task<string> ExtractTextFromUrlAsync(string url, CancellationToken cancellationToken)
    //    => string.Empty;

    //private async Task ExtractTagsAsync(string content, CancellationToken cancellationToken = default)
    //{
    //}
    public async Task<string[]> GetTagsAsync(CancellationToken cancellationToken)
    {
        var response = await this.client.GetAsync($"/api/articles/tags", cancellationToken).ConfigureAwait(false);
        _ = response.EnsureSuccessStatusCode();
        return await response.Content.ReadFromJsonAsync<string[]>(cancellationToken).ConfigureAwait(false) ?? [];
    }

    /// <summary>
    /// Récupère toutes les catégories.
    /// </summary>
    /// <param name="includeInactive">Inclut les catégories inactives.</param>
    /// <param name="cancellationToken">Token d'annulation.</param>
    /// <returns>Liste des catégories.</returns>
    public async Task<CategoryModel[]> GetCategoriesAsync(bool includeInactive = false, CancellationToken cancellationToken = default)
    {
        var response = await this.client.GetAsync($"/api/categories?includeInactive={includeInactive}", cancellationToken).ConfigureAwait(false);
        _ = response.EnsureSuccessStatusCode();
        return await response.Content.ReadFromJsonAsync<CategoryModel[]>(cancellationToken).ConfigureAwait(false) ?? [];
    }

    /// <summary>
    /// Récupère une catégorie par son ID.
    /// </summary>
    /// <param name="id">Identifiant de la catégorie.</param>
    /// <param name="cancellationToken">Token d'annulation.</param>
    /// <returns>La catégorie ou null si non trouvée.</returns>
    public async Task<CategoryModel?> GetCategoryByIdAsync(int id, CancellationToken cancellationToken = default)
    {
        var response = await this.client.GetAsync($"/api/categories/{id}", cancellationToken).ConfigureAwait(false);
        if (!response.IsSuccessStatusCode)
        {
            return null;
        }
        return await response.Content.ReadFromJsonAsync<CategoryModel>(cancellationToken).ConfigureAwait(false);
    }

    /// <summary>
    /// Crée une nouvelle catégorie.
    /// </summary>
    /// <param name="name">Nom de la catégorie.</param>
    /// <param name="description">Description de la catégorie.</param>
    /// <param name="color">Couleur de la catégorie.</param>
    /// <param name="icon">Icône de la catégorie.</param>
    /// <param name="keywords">Mots-clés de la catégorie.</param>
    /// <param name="parentId">ID de la catégorie parente.</param>
    /// <param name="confidenceThreshold">Seuil de confiance.</param>
    /// <param name="cancellationToken">Token d'annulation.</param>
    /// <returns>Résultat de l'opération.</returns>
    public async Task<ApiResult<CategoryModel>> CreateCategoryAsync(
        string name,
        string? description = null,
        string? color = null,
        string? icon = null,
        string[]? keywords = null,
        int? parentId = null,
        double? confidenceThreshold = null,
        CancellationToken cancellationToken = default)
    {
        var createModel = new
        {
            Name = name,
            Description = description,
            Color = color,
            Icon = icon,
            Keywords = keywords ?? [],
            ParentId = parentId,
            ConfidenceThreshold = confidenceThreshold
        };

        var response = await this.client.PostAsJsonAsync("/api/categories", createModel, cancellationToken).ConfigureAwait(false);

        if (!response.IsSuccessStatusCode)
        {
            var errorContent = await response.Content.ReadAsStringAsync(cancellationToken).ConfigureAwait(false);
            return response.StatusCode switch
            {
                System.Net.HttpStatusCode.Conflict => ApiResult<CategoryModel>.Failure(ApiResultErrorType.Conflict),
                System.Net.HttpStatusCode.BadRequest => ApiResult<CategoryModel>.Failure(ApiResultErrorType.BadRequest),
                _ => ApiResult<CategoryModel>.Failure($"Échec de la création de la catégorie: {errorContent}")
            };
        }

        var category = await response.Content.ReadFromJsonAsync<CategoryModel>(cancellationToken).ConfigureAwait(false);
        if (category == null)
        {
            return ApiResult<CategoryModel>.Failure("Échec de la lecture de la réponse");
        }

        return ApiResult<CategoryModel>.Success(category);
    }

    /// <summary>
    /// Met à jour une catégorie.
    /// </summary>
    /// <param name="id">ID de la catégorie.</param>
    /// <param name="name">Nom de la catégorie.</param>
    /// <param name="description">Description de la catégorie.</param>
    /// <param name="color">Couleur de la catégorie.</param>
    /// <param name="icon">Icône de la catégorie.</param>
    /// <param name="keywords">Mots-clés de la catégorie.</param>
    /// <param name="parentId">ID de la catégorie parente.</param>
    /// <param name="isActive">Statut actif de la catégorie.</param>
    /// <param name="confidenceThreshold">Seuil de confiance.</param>
    /// <param name="cancellationToken">Token d'annulation.</param>
    /// <returns>Résultat de l'opération.</returns>
    public async Task<ApiResult<CategoryModel>> UpdateCategoryAsync(
        int id,
        string? name = null,
        string? description = null,
        string? color = null,
        string? icon = null,
        string[]? keywords = null,
        int? parentId = null,
        bool? isActive = null,
        double? confidenceThreshold = null,
        CancellationToken cancellationToken = default)
    {
        var updateModel = new
        {
            Name = name,
            Description = description,
            Color = color,
            Icon = icon,
            Keywords = keywords,
            ParentId = parentId,
            IsActive = isActive,
            ConfidenceThreshold = confidenceThreshold
        };

        var response = await this.client.PutAsJsonAsync($"/api/categories/{id}", updateModel, cancellationToken).ConfigureAwait(false);

        if (!response.IsSuccessStatusCode)
        {
            var errorContent = await response.Content.ReadAsStringAsync(cancellationToken).ConfigureAwait(false);
            return response.StatusCode switch
            {
                System.Net.HttpStatusCode.NotFound => ApiResult<CategoryModel>.Failure(ApiResultErrorType.NotFound),
                System.Net.HttpStatusCode.Conflict => ApiResult<CategoryModel>.Failure(ApiResultErrorType.Conflict),
                System.Net.HttpStatusCode.BadRequest => ApiResult<CategoryModel>.Failure(ApiResultErrorType.BadRequest),
                _ => ApiResult<CategoryModel>.Failure($"Échec de la mise à jour de la catégorie: {errorContent}")
            };
        }

        var category = await response.Content.ReadFromJsonAsync<CategoryModel>(cancellationToken).ConfigureAwait(false);
        if (category == null)
        {
            return ApiResult<CategoryModel>.Failure("Échec de la lecture de la réponse");
        }

        return ApiResult<CategoryModel>.Success(category);
    }

    /// <summary>
    /// Supprime une catégorie.
    /// </summary>
    /// <param name="id">ID de la catégorie à supprimer.</param>
    /// <param name="cancellationToken">Token d'annulation.</param>
    /// <returns>Résultat de l'opération.</returns>
    public async Task<ApiResult> DeleteCategoryAsync(int id, CancellationToken cancellationToken = default)
    {
        var response = await this.client.DeleteAsync($"/api/categories/{id}", cancellationToken).ConfigureAwait(false);

        if (!response.IsSuccessStatusCode)
        {
            var errorContent = await response.Content.ReadAsStringAsync(cancellationToken).ConfigureAwait(false);
            return response.StatusCode switch
            {
                System.Net.HttpStatusCode.NotFound => ApiResult.Failure(ApiResultErrorType.NotFound),
                System.Net.HttpStatusCode.BadRequest => ApiResult.Failure(ApiResultErrorType.BadRequest),
                _ => ApiResult.Failure($"Échec de la suppression de la catégorie: {errorContent}")
            };
        }

        return ApiResult.Success();
    }

    /// <summary>
    /// Récupère les catégories racines avec leurs enfants.
    /// </summary>
    /// <param name="includeInactive">Inclut les catégories inactives.</param>
    /// <param name="cancellationToken">Token d'annulation.</param>
    /// <returns>Liste des catégories racines.</returns>
    public async Task<CategoryModel[]> GetRootCategoriesAsync(bool includeInactive = false, CancellationToken cancellationToken = default)
    {
        var response = await this.client.GetAsync($"/api/categories/roots?includeInactive={includeInactive}", cancellationToken).ConfigureAwait(false);
        _ = response.EnsureSuccessStatusCode();
        return await response.Content.ReadFromJsonAsync<CategoryModel[]>(cancellationToken).ConfigureAwait(false) ?? [];
    }

    /// <summary>
    /// Assigne une catégorie à un article.
    /// </summary>
    /// <param name="categoryId">ID de la catégorie.</param>
    /// <param name="articleId">ID de l'article.</param>
    /// <param name="cancellationToken">Token d'annulation.</param>
    /// <returns>Résultat de l'opération.</returns>
    public async Task<ApiResult> AssignCategoryToArticleAsync(int categoryId, int articleId, CancellationToken cancellationToken = default)
    {
        var response = await this.client.PostAsync($"/api/categories/{categoryId}/articles/{articleId}", null, cancellationToken).ConfigureAwait(false);

        if (!response.IsSuccessStatusCode)
        {
            var errorContent = await response.Content.ReadAsStringAsync(cancellationToken).ConfigureAwait(false);
            return ApiResult.Failure($"Échec de l'assignation de la catégorie: {errorContent}");
        }

        return ApiResult.Success();
    }

    /// <summary>
    /// Retire une catégorie d'un article.
    /// </summary>
    /// <param name="categoryId">ID de la catégorie.</param>
    /// <param name="articleId">ID de l'article.</param>
    /// <param name="cancellationToken">Token d'annulation.</param>
    /// <returns>Résultat de l'opération.</returns>
    public async Task<ApiResult> RemoveCategoryFromArticleAsync(int categoryId, int articleId, CancellationToken cancellationToken = default)
    {
        var response = await this.client.DeleteAsync($"/api/categories/{categoryId}/articles/{articleId}", cancellationToken).ConfigureAwait(false);

        if (!response.IsSuccessStatusCode)
        {
            var errorContent = await response.Content.ReadAsStringAsync(cancellationToken).ConfigureAwait(false);
            return response.StatusCode switch
            {
                System.Net.HttpStatusCode.NotFound => ApiResult.Failure(ApiResultErrorType.NotFound),
                _ => ApiResult.Failure($"Échec de la suppression de l'assignation: {errorContent}")
            };
        }

        return ApiResult.Success();
    }
}
