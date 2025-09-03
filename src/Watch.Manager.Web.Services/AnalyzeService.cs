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

    /// <summary>
    /// Classifie automatiquement un article et retourne les catégories suggérées.
    /// </summary>
    /// <param name="articleId">ID de l'article à classifier.</param>
    /// <param name="cancellationToken">Token d'annulation.</param>
    /// <returns>Résultat de la classification.</returns>
    public async Task<ApiResult<CategorySuggestionModel[]>> ClassifyArticleAsync(int articleId, CancellationToken cancellationToken = default)
    {
        var response = await this.client.PostAsync($"/api/classification/articles/{articleId}/classify", null, cancellationToken).ConfigureAwait(false);

        if (!response.IsSuccessStatusCode)
        {
            var errorContent = await response.Content.ReadAsStringAsync(cancellationToken).ConfigureAwait(false);
            return response.StatusCode switch
            {
                System.Net.HttpStatusCode.NotFound => ApiResult<CategorySuggestionModel[]>.Failure(ApiResultErrorType.NotFound),
                System.Net.HttpStatusCode.BadRequest => ApiResult<CategorySuggestionModel[]>.Failure(ApiResultErrorType.BadRequest),
                _ => ApiResult<CategorySuggestionModel[]>.Failure($"Échec de la classification: {errorContent}")
            };
        }

        var suggestions = await response.Content.ReadFromJsonAsync<CategorySuggestionModel[]>(cancellationToken).ConfigureAwait(false);
        if (suggestions == null)
        {
            return ApiResult<CategorySuggestionModel[]>.Failure("Échec de la lecture de la réponse");
        }

        return ApiResult<CategorySuggestionModel[]>.Success(suggestions);
    }

    /// <summary>
    /// Auto-assigne les catégories les plus pertinentes à un article basé sur un seuil de confiance.
    /// </summary>
    /// <param name="articleId">ID de l'article.</param>
    /// <param name="confidenceThreshold">Seuil de confiance minimal (0.0 à 1.0).</param>
    /// <param name="cancellationToken">Token d'annulation.</param>
    /// <returns>Résultat de l'auto-assignation.</returns>
    public async Task<ApiResult<CategoryAssignmentModel[]>> AutoAssignCategoriesAsync(int articleId, double? confidenceThreshold = null, CancellationToken cancellationToken = default)
    {
        var requestBody = new { ConfidenceThreshold = confidenceThreshold };
        var response = await this.client.PostAsJsonAsync($"/api/classification/articles/{articleId}/auto-assign", requestBody, cancellationToken).ConfigureAwait(false);

        if (!response.IsSuccessStatusCode)
        {
            var errorContent = await response.Content.ReadAsStringAsync(cancellationToken).ConfigureAwait(false);
            return response.StatusCode switch
            {
                System.Net.HttpStatusCode.NotFound => ApiResult<CategoryAssignmentModel[]>.Failure(ApiResultErrorType.NotFound),
                System.Net.HttpStatusCode.BadRequest => ApiResult<CategoryAssignmentModel[]>.Failure(ApiResultErrorType.BadRequest),
                _ => ApiResult<CategoryAssignmentModel[]>.Failure($"Échec de l'auto-assignation: {errorContent}")
            };
        }

        var assignments = await response.Content.ReadFromJsonAsync<CategoryAssignmentModel[]>(cancellationToken).ConfigureAwait(false);
        if (assignments == null)
        {
            return ApiResult<CategoryAssignmentModel[]>.Failure("Échec de la lecture de la réponse");
        }

        return ApiResult<CategoryAssignmentModel[]>.Success(assignments);
    }

    /// <summary>
    /// Suggère de nouvelles catégories basées sur un article qui ne correspond à aucune catégorie existante.
    /// </summary>
    /// <param name="articleId">ID de l'article.</param>
    /// <param name="maxSuggestions">Nombre maximum de suggestions.</param>
    /// <param name="cancellationToken">Token d'annulation.</param>
    /// <returns>Suggestions de nouvelles catégories.</returns>
    public async Task<ApiResult<NewCategorySuggestionModel[]>> SuggestNewCategoriesAsync(int articleId, int maxSuggestions = 3, CancellationToken cancellationToken = default)
    {
        var response = await this.client.GetAsync($"/api/classification/suggestions/new-categories?articleId={articleId}&maxSuggestions={maxSuggestions}", cancellationToken).ConfigureAwait(false);

        if (!response.IsSuccessStatusCode)
        {
            var errorContent = await response.Content.ReadAsStringAsync(cancellationToken).ConfigureAwait(false);
            return response.StatusCode switch
            {
                System.Net.HttpStatusCode.NotFound => ApiResult<NewCategorySuggestionModel[]>.Failure(ApiResultErrorType.NotFound),
                System.Net.HttpStatusCode.BadRequest => ApiResult<NewCategorySuggestionModel[]>.Failure(ApiResultErrorType.BadRequest),
                _ => ApiResult<NewCategorySuggestionModel[]>.Failure($"Échec de la récupération des suggestions: {errorContent}")
            };
        }

        var suggestions = await response.Content.ReadFromJsonAsync<NewCategorySuggestionModel[]>(cancellationToken).ConfigureAwait(false);
        if (suggestions == null)
        {
            return ApiResult<NewCategorySuggestionModel[]>.Failure("Échec de la lecture de la réponse");
        }

        return ApiResult<NewCategorySuggestionModel[]>.Success(suggestions);
    }

    /// <summary>
    /// Envoie un feedback sur une classification pour améliorer le modèle.
    /// </summary>
    /// <param name="articleId">ID de l'article.</param>
    /// <param name="categoryId">ID de la catégorie.</param>
    /// <param name="isCorrect">Indique si la classification était correcte.</param>
    /// <param name="actualCategoryId">ID de la catégorie réelle (si la classification était incorrecte).</param>
    /// <param name="cancellationToken">Token d'annulation.</param>
    /// <returns>Résultat de l'envoi du feedback.</returns>
    public async Task<ApiResult> SubmitClassificationFeedbackAsync(int articleId, int categoryId, bool isCorrect, int? actualCategoryId = null, CancellationToken cancellationToken = default)
    {
        var feedbackModel = new
        {
            ArticleId = articleId,
            CategoryId = categoryId,
            IsCorrect = isCorrect,
            ActualCategoryId = actualCategoryId
        };

        var response = await this.client.PostAsJsonAsync("/api/classification/feedback", feedbackModel, cancellationToken).ConfigureAwait(false);

        if (!response.IsSuccessStatusCode)
        {
            var errorContent = await response.Content.ReadAsStringAsync(cancellationToken).ConfigureAwait(false);
            return response.StatusCode switch
            {
                System.Net.HttpStatusCode.BadRequest => ApiResult.Failure(ApiResultErrorType.BadRequest),
                _ => ApiResult.Failure($"Échec de l'envoi du feedback: {errorContent}")
            };
        }

        return ApiResult.Success();
    }

    /// <summary>
    /// Suggère de nouvelles catégories basées sur tous les articles non classifiés.
    /// </summary>
    /// <param name="cancellationToken">Token d'annulation.</param>
    /// <returns>Suggestions de nouvelles catégories.</returns>
    public async Task<ApiResult<NewCategorySuggestion[]>> SuggestNewCategoriesAsync(CancellationToken cancellationToken = default)
    {
        var response = await this.client.GetAsync("/api/classification/suggestions", cancellationToken).ConfigureAwait(false);

        if (!response.IsSuccessStatusCode)
        {
            var errorContent = await response.Content.ReadAsStringAsync(cancellationToken).ConfigureAwait(false);
            return response.StatusCode switch
            {
                System.Net.HttpStatusCode.NotFound => ApiResult<NewCategorySuggestion[]>.Failure(ApiResultErrorType.NotFound),
                System.Net.HttpStatusCode.BadRequest => ApiResult<NewCategorySuggestion[]>.Failure(ApiResultErrorType.BadRequest),
                _ => ApiResult<NewCategorySuggestion[]>.Failure($"Échec de la récupération des suggestions: {errorContent}")
            };
        }

        var suggestions = await response.Content.ReadFromJsonAsync<Models.ApiSuggestion[]>(cancellationToken).ConfigureAwait(false);
        if (suggestions == null)
        {
            return ApiResult<NewCategorySuggestion[]>.Failure("Échec de la lecture de la réponse");
        }

        // Conversion des suggestions de l'API vers le modèle Web
        var convertedSuggestions = suggestions.Select(s => new NewCategorySuggestion
        {
            Name = s.SuggestedName,
            Description = s.SuggestedDescription,
            Keywords = s.SuggestedKeywords,
            ConfidenceScore = s.RelevanceScore,
            Reason = s.Justification,
            ParentId = s.SuggestedParentId,
            ParentName = s.SuggestedParentName,
            Color = s.SuggestedColor,
            Icon = s.SuggestedIcon
        }).ToArray();

        return ApiResult<NewCategorySuggestion[]>.Success(convertedSuggestions);
    }

    /// <summary>
    /// Classifie automatiquement tous les articles non classifiés par lot.
    /// </summary>
    /// <param name="cancellationToken">Token d'annulation.</param>
    /// <returns>Résultats de la classification par lot.</returns>
    public async Task<ApiResult<BatchClassificationResult[]>> BatchClassifyAsync(CancellationToken cancellationToken = default)
    {
        var response = await this.client.PostAsync("/api/classification/batch", null, cancellationToken).ConfigureAwait(false);

        if (!response.IsSuccessStatusCode)
        {
            var errorContent = await response.Content.ReadAsStringAsync(cancellationToken).ConfigureAwait(false);
            return response.StatusCode switch
            {
                System.Net.HttpStatusCode.BadRequest => ApiResult<BatchClassificationResult[]>.Failure(ApiResultErrorType.BadRequest),
                _ => ApiResult<BatchClassificationResult[]>.Failure($"Échec de la classification par lot: {errorContent}")
            };
        }

        var results = await response.Content.ReadFromJsonAsync<BatchClassificationResult[]>(cancellationToken).ConfigureAwait(false);
        if (results == null)
        {
            return ApiResult<BatchClassificationResult[]>.Failure("Échec de la lecture de la réponse");
        }

        return ApiResult<BatchClassificationResult[]>.Success(results);
    }
}
