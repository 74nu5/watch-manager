namespace Watch.Manager.Web.Services;

using System.Net;
using System.Net.Http.Json;

using Watch.Manager.Common;
using Watch.Manager.Web.Services.Models;

/// <summary>
///     Service for analyzing articles, managing categories, and handling classification operations via API.
/// </summary>
public sealed class AnalyzeService
{
    private readonly HttpClient client;

    /// <summary>
    ///     Initializes a new instance of the <see cref="AnalyzeService" /> class.
    /// </summary>
    /// <param name="client">HTTP client for API requests.</param>
    public AnalyzeService(HttpClient client)
        => this.client = client;

    /// <summary>
    ///     Saves an article by analyzing its content from the specified URL.
    /// </summary>
    /// <param name="url">The URL of the article to analyze and save.</param>
    /// <param name="cancellationToken">Cancellation token.</param>
    /// <returns>API result containing the extracted analysis model.</returns>
    public async Task<ApiResult<ExtractAnalyseModel>> SaveArticleAsync(string url, CancellationToken cancellationToken = default)
    {
        var response = await this.client.PostAsJsonAsync("/api/articles/save", new { UriToAnalyze = url }, cancellationToken).ConfigureAwait(false);

        if (!response.IsSuccessStatusCode)
        {
            return response.StatusCode switch
            {
                HttpStatusCode.Conflict => ApiResult<ExtractAnalyseModel>.Failure(ApiResultErrorType.Conflict),
                HttpStatusCode.NotFound => ApiResult<ExtractAnalyseModel>.Failure(ApiResultErrorType.NotFound),
                HttpStatusCode.Forbidden => ApiResult<ExtractAnalyseModel>.Failure(ApiResultErrorType.Forbidden),
                _ => ApiResult<ExtractAnalyseModel>.Failure("Failed to save article"),
            };
        }

        var analyseModel = await response.Content.ReadFromJsonAsync<ExtractAnalyseModel>(cancellationToken).ConfigureAwait(false);

        return analyseModel switch
        {
            null => ApiResult<ExtractAnalyseModel>.Failure("Failed to parse response"),
            _ => ApiResult<ExtractAnalyseModel>.Success(analyseModel),
        };
    }

    /// <summary>
    ///     Searches for articles by text and optional tag.
    /// </summary>
    /// <param name="text">Text to search for.</param>
    /// <param name="tag">Optional tag to filter articles.</param>
    /// <param name="cancellationToken">Cancellation token.</param>
    /// <returns>Array of matching articles.</returns>
    public async Task<ArticleModel[]> SearchArticleAsync(string text, string? tag, CancellationToken cancellationToken = default)
    {
        var response = await this.client.GetAsync($"/api/articles/search?text={text}&tag={tag}", cancellationToken).ConfigureAwait(false);
        _ = response.EnsureSuccessStatusCode();
        return await response.Content.ReadFromJsonAsync<ArticleModel[]>(cancellationToken).ConfigureAwait(false) ?? [];
    }

    /// <summary>
    ///     Gets all available tags.
    /// </summary>
    /// <param name="cancellationToken">Cancellation token.</param>
    /// <returns>Array of tags.</returns>
    public async Task<string[]> GetTagsAsync(CancellationToken cancellationToken)
    {
        var response = await this.client.GetAsync("/api/articles/tags", cancellationToken).ConfigureAwait(false);
        _ = response.EnsureSuccessStatusCode();
        return await response.Content.ReadFromJsonAsync<string[]>(cancellationToken).ConfigureAwait(false) ?? [];
    }

    /// <summary>
    ///     Gets all categories.
    /// </summary>
    /// <param name="includeInactive">Whether to include inactive categories.</param>
    /// <param name="cancellationToken">Cancellation token.</param>
    /// <returns>Array of categories.</returns>
    public async Task<CategoryModel[]> GetCategoriesAsync(bool includeInactive = false, CancellationToken cancellationToken = default)
    {
        var response = await this.client.GetAsync($"/api/categories?includeInactive={includeInactive}", cancellationToken).ConfigureAwait(false);
        _ = response.EnsureSuccessStatusCode();
        return await response.Content.ReadFromJsonAsync<CategoryModel[]>(cancellationToken).ConfigureAwait(false) ?? [];
    }

    /// <summary>
    ///     Gets categories organized as a hierarchical tree.
    /// </summary>
    /// <param name="includeInactive">Whether to include inactive categories.</param>
    /// <param name="cancellationToken">Cancellation token.</param>
    /// <returns>API result containing the hierarchical tree of categories.</returns>
    public async Task<ApiResult<CategoryModel[]>> GetCategoriesAsTreeAsync(bool includeInactive = false, CancellationToken cancellationToken = default)
    {
        try
        {
            var response = await this.client.GetAsync($"/api/categories/tree?includeInactive={includeInactive}", cancellationToken).ConfigureAwait(false);

            if (!response.IsSuccessStatusCode)
            {
                return response.StatusCode switch
                {
                    HttpStatusCode.NotFound => ApiResult<CategoryModel[]>.Failure(ApiResultErrorType.NotFound),
                    HttpStatusCode.BadRequest => ApiResult<CategoryModel[]>.Failure(ApiResultErrorType.BadRequest),
                    _ => ApiResult<CategoryModel[]>.Failure("Failed to load categories tree"),
                };
            }

            var categories = await response.Content.ReadFromJsonAsync<CategoryModel[]>(cancellationToken).ConfigureAwait(false);
            return ApiResult<CategoryModel[]>.Success(categories ?? []);
        }
        catch (Exception ex)
        {
            return ApiResult<CategoryModel[]>.Failure($"Error loading categories tree: {ex.Message}");
        }
    }

    /// <summary>
    ///     Gets a category by its identifier.
    /// </summary>
    /// <param name="id">Category identifier.</param>
    /// <param name="cancellationToken">Cancellation token.</param>
    /// <returns>The category if found; otherwise, null.</returns>
    public async Task<CategoryModel?> GetCategoryByIdAsync(int id, CancellationToken cancellationToken = default)
    {
        var response = await this.client.GetAsync($"/api/categories/{id}", cancellationToken).ConfigureAwait(false);
        if (!response.IsSuccessStatusCode)
            return null;

        return await response.Content.ReadFromJsonAsync<CategoryModel>(cancellationToken).ConfigureAwait(false);
    }

    /// <summary>
    ///     Creates a new category.
    /// </summary>
    /// <param name="name">Category name.</param>
    /// <param name="description">Category description.</param>
    /// <param name="color">Category color.</param>
    /// <param name="icon">Category icon.</param>
    /// <param name="keywords">Category keywords.</param>
    /// <param name="parentId">Parent category identifier.</param>
    /// <param name="confidenceThreshold">Confidence threshold for classification.</param>
    /// <param name="cancellationToken">Cancellation token.</param>
    /// <returns>API result containing the created category.</returns>
    public async Task<ApiResult<CategoryModel>> CreateCategoryAsync(string name,
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
            ConfidenceThreshold = confidenceThreshold,
        };

        var response = await this.client.PostAsJsonAsync("/api/categories", createModel, cancellationToken).ConfigureAwait(false);

        if (!response.IsSuccessStatusCode)
        {
            var errorContent = await response.Content.ReadAsStringAsync(cancellationToken).ConfigureAwait(false);
            return response.StatusCode switch
            {
                HttpStatusCode.Conflict => ApiResult<CategoryModel>.Failure(ApiResultErrorType.Conflict),
                HttpStatusCode.BadRequest => ApiResult<CategoryModel>.Failure(ApiResultErrorType.BadRequest),
                _ => ApiResult<CategoryModel>.Failure($"Failed to create category: {errorContent}"),
            };
        }

        var category = await response.Content.ReadFromJsonAsync<CategoryModel>(cancellationToken).ConfigureAwait(false);
        return category switch
        {
            null => ApiResult<CategoryModel>.Failure("Failed to read response"),
            _ => ApiResult<CategoryModel>.Success(category),
        };
    }

    /// <summary>
    ///     Updates an existing category.
    /// </summary>
    /// <param name="id">Category identifier.</param>
    /// <param name="name">Category name.</param>
    /// <param name="description">Category description.</param>
    /// <param name="color">Category color.</param>
    /// <param name="icon">Category icon.</param>
    /// <param name="keywords">Category keywords.</param>
    /// <param name="parentId">Parent category identifier.</param>
    /// <param name="isActive">Whether the category is active.</param>
    /// <param name="confidenceThreshold">Confidence threshold for classification.</param>
    /// <param name="cancellationToken">Cancellation token.</param>
    /// <returns>API result containing the updated category.</returns>
    public async Task<ApiResult<CategoryModel>> UpdateCategoryAsync(int id,
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
            ConfidenceThreshold = confidenceThreshold,
        };

        var response = await this.client.PutAsJsonAsync($"/api/categories/{id}", updateModel, cancellationToken).ConfigureAwait(false);

        if (!response.IsSuccessStatusCode)
        {
            var errorContent = await response.Content.ReadAsStringAsync(cancellationToken).ConfigureAwait(false);
            return response.StatusCode switch
            {
                HttpStatusCode.NotFound => ApiResult<CategoryModel>.Failure(ApiResultErrorType.NotFound),
                HttpStatusCode.Conflict => ApiResult<CategoryModel>.Failure(ApiResultErrorType.Conflict),
                HttpStatusCode.BadRequest => ApiResult<CategoryModel>.Failure(ApiResultErrorType.BadRequest),
                _ => ApiResult<CategoryModel>.Failure($"Failed to update category: {errorContent}"),
            };
        }

        var category = await response.Content.ReadFromJsonAsync<CategoryModel>(cancellationToken).ConfigureAwait(false);
        return category switch
        {
            null => ApiResult<CategoryModel>.Failure("Failed to read response"),
            _ => ApiResult<CategoryModel>.Success(category),
        };
    }

    /// <summary>
    ///     Deletes a category by its identifier.
    /// </summary>
    /// <param name="id">Category identifier.</param>
    /// <param name="cancellationToken">Cancellation token.</param>
    /// <returns>API result indicating success or failure.</returns>
    public async Task<ApiResult> DeleteCategoryAsync(int id, CancellationToken cancellationToken = default)
    {
        var response = await this.client.DeleteAsync($"/api/categories/{id}", cancellationToken).ConfigureAwait(false);

        if (!response.IsSuccessStatusCode)
        {
            var errorContent = await response.Content.ReadAsStringAsync(cancellationToken).ConfigureAwait(false);
            return response.StatusCode switch
            {
                HttpStatusCode.NotFound => ApiResult.Failure(ApiResultErrorType.NotFound),
                HttpStatusCode.BadRequest => ApiResult.Failure(ApiResultErrorType.BadRequest),
                _ => ApiResult.Failure($"Failed to delete category: {errorContent}"),
            };
        }

        return ApiResult.Success();
    }

    /// <summary>
    ///     Gets root categories with their children.
    /// </summary>
    /// <param name="includeInactive">Whether to include inactive categories.</param>
    /// <param name="cancellationToken">Cancellation token.</param>
    /// <returns>Array of root categories.</returns>
    public async Task<CategoryModel[]> GetRootCategoriesAsync(bool includeInactive = false, CancellationToken cancellationToken = default)
    {
        var response = await this.client.GetAsync($"/api/categories/roots?includeInactive={includeInactive}", cancellationToken).ConfigureAwait(false);
        _ = response.EnsureSuccessStatusCode();
        return await response.Content.ReadFromJsonAsync<CategoryModel[]>(cancellationToken).ConfigureAwait(false) ?? [];
    }

    /// <summary>
    ///     Assigns a category to an article.
    /// </summary>
    /// <param name="categoryId">Category identifier.</param>
    /// <param name="articleId">Article identifier.</param>
    /// <param name="cancellationToken">Cancellation token.</param>
    /// <returns>API result indicating success or failure.</returns>
    public async Task<ApiResult> AssignCategoryToArticleAsync(int categoryId, int articleId, CancellationToken cancellationToken = default)
    {
        var response = await this.client.PostAsync($"/api/categories/{categoryId}/articles/{articleId}", null, cancellationToken).ConfigureAwait(false);

        if (!response.IsSuccessStatusCode)
        {
            var errorContent = await response.Content.ReadAsStringAsync(cancellationToken).ConfigureAwait(false);
            return ApiResult.Failure($"Failed to assign category: {errorContent}");
        }

        return ApiResult.Success();
    }

    /// <summary>
    ///     Removes a category assignment from an article.
    /// </summary>
    /// <param name="categoryId">Category identifier.</param>
    /// <param name="articleId">Article identifier.</param>
    /// <param name="cancellationToken">Cancellation token.</param>
    /// <returns>API result indicating success or failure.</returns>
    public async Task<ApiResult> RemoveCategoryFromArticleAsync(int categoryId, int articleId, CancellationToken cancellationToken = default)
    {
        var response = await this.client.DeleteAsync($"/api/categories/{categoryId}/articles/{articleId}", cancellationToken).ConfigureAwait(false);

        if (!response.IsSuccessStatusCode)
        {
            var errorContent = await response.Content.ReadAsStringAsync(cancellationToken).ConfigureAwait(false);
            return response.StatusCode switch
            {
                HttpStatusCode.NotFound => ApiResult.Failure(ApiResultErrorType.NotFound),
                _ => ApiResult.Failure($"Failed to remove category assignment: {errorContent}"),
            };
        }

        return ApiResult.Success();
    }

    /// <summary>
    ///     Automatically classifies an article and returns suggested categories.
    /// </summary>
    /// <param name="articleId">Article identifier.</param>
    /// <param name="cancellationToken">Cancellation token.</param>
    /// <returns>API result containing suggested categories.</returns>
    public async Task<ApiResult<CategorySuggestionModel[]>> ClassifyArticleAsync(int articleId, CancellationToken cancellationToken = default)
    {
        var response = await this.client.PostAsync($"/api/classification/articles/{articleId}/classify", null, cancellationToken).ConfigureAwait(false);

        if (!response.IsSuccessStatusCode)
        {
            var errorContent = await response.Content.ReadAsStringAsync(cancellationToken).ConfigureAwait(false);
            return response.StatusCode switch
            {
                HttpStatusCode.NotFound => ApiResult<CategorySuggestionModel[]>.Failure(ApiResultErrorType.NotFound),
                HttpStatusCode.BadRequest => ApiResult<CategorySuggestionModel[]>.Failure(ApiResultErrorType.BadRequest),
                _ => ApiResult<CategorySuggestionModel[]>.Failure($"Failed to classify article: {errorContent}"),
            };
        }

        var suggestions = await response.Content.ReadFromJsonAsync<CategorySuggestionModel[]>(cancellationToken).ConfigureAwait(false);
        return suggestions switch
        {
            null => ApiResult<CategorySuggestionModel[]>.Failure("Failed to read response"),
            _ => ApiResult<CategorySuggestionModel[]>.Success(suggestions),
        };
    }

    /// <summary>
    ///     Automatically assigns the most relevant categories to an article based on a confidence threshold.
    /// </summary>
    /// <param name="articleId">Article identifier.</param>
    /// <param name="confidenceThreshold">Minimum confidence threshold (0.0 to 1.0).</param>
    /// <param name="cancellationToken">Cancellation token.</param>
    /// <returns>API result containing assigned categories.</returns>
    public async Task<ApiResult<CategoryAssignmentModel[]>> AutoAssignCategoriesAsync(int articleId, double? confidenceThreshold = null, CancellationToken cancellationToken = default)
    {
        var requestBody = new { ConfidenceThreshold = confidenceThreshold };
        var response = await this.client.PostAsJsonAsync($"/api/classification/articles/{articleId}/auto-assign", requestBody, cancellationToken).ConfigureAwait(false);

        if (!response.IsSuccessStatusCode)
        {
            var errorContent = await response.Content.ReadAsStringAsync(cancellationToken).ConfigureAwait(false);
            return response.StatusCode switch
            {
                HttpStatusCode.NotFound => ApiResult<CategoryAssignmentModel[]>.Failure(ApiResultErrorType.NotFound),
                HttpStatusCode.BadRequest => ApiResult<CategoryAssignmentModel[]>.Failure(ApiResultErrorType.BadRequest),
                _ => ApiResult<CategoryAssignmentModel[]>.Failure($"Failed to auto-assign categories: {errorContent}"),
            };
        }

        var assignments = await response.Content.ReadFromJsonAsync<CategoryAssignmentModel[]>(cancellationToken).ConfigureAwait(false);
        return assignments switch
        {
            null => ApiResult<CategoryAssignmentModel[]>.Failure("Failed to read response"),
            _ => ApiResult<CategoryAssignmentModel[]>.Success(assignments),
        };
    }

    /// <summary>
    ///     Suggests new categories based on an article that does not match any existing category.
    /// </summary>
    /// <param name="articleId">Article identifier.</param>
    /// <param name="maxSuggestions">Maximum number of suggestions.</param>
    /// <param name="cancellationToken">Cancellation token.</param>
    /// <returns>API result containing new category suggestions.</returns>
    public async Task<ApiResult<NewCategorySuggestionModel[]>> SuggestNewCategoriesAsync(int articleId, int maxSuggestions = 3, CancellationToken cancellationToken = default)
    {
        var response = await this.client.GetAsync($"/api/classification/suggestions/new-categories?articleId={articleId}&maxSuggestions={maxSuggestions}", cancellationToken).ConfigureAwait(false);

        if (!response.IsSuccessStatusCode)
        {
            var errorContent = await response.Content.ReadAsStringAsync(cancellationToken).ConfigureAwait(false);
            return response.StatusCode switch
            {
                HttpStatusCode.NotFound => ApiResult<NewCategorySuggestionModel[]>.Failure(ApiResultErrorType.NotFound),
                HttpStatusCode.BadRequest => ApiResult<NewCategorySuggestionModel[]>.Failure(ApiResultErrorType.BadRequest),
                _ => ApiResult<NewCategorySuggestionModel[]>.Failure($"Failed to get suggestions: {errorContent}"),
            };
        }

        var suggestions = await response.Content.ReadFromJsonAsync<NewCategorySuggestionModel[]>(cancellationToken).ConfigureAwait(false);
        return suggestions switch
        {
            null => ApiResult<NewCategorySuggestionModel[]>.Failure("Failed to read response"),
            _ => ApiResult<NewCategorySuggestionModel[]>.Success(suggestions),
        };
    }

    /// <summary>
    ///     Submits feedback on a classification to improve the model.
    /// </summary>
    /// <param name="articleId">Article identifier.</param>
    /// <param name="categoryId">Category identifier.</param>
    /// <param name="isCorrect">Indicates if the classification was correct.</param>
    /// <param name="actualCategoryId">Actual category identifier if the classification was incorrect.</param>
    /// <param name="cancellationToken">Cancellation token.</param>
    /// <returns>API result indicating success or failure.</returns>
    public async Task<ApiResult> SubmitClassificationFeedbackAsync(int articleId, int categoryId, bool isCorrect, int? actualCategoryId = null, CancellationToken cancellationToken = default)
    {
        var feedbackModel = new
        {
            ArticleId = articleId,
            CategoryId = categoryId,
            IsCorrect = isCorrect,
            ActualCategoryId = actualCategoryId,
        };

        var response = await this.client.PostAsJsonAsync("/api/classification/feedback", feedbackModel, cancellationToken).ConfigureAwait(false);

        if (!response.IsSuccessStatusCode)
        {
            var errorContent = await response.Content.ReadAsStringAsync(cancellationToken).ConfigureAwait(false);
            return response.StatusCode switch
            {
                HttpStatusCode.BadRequest => ApiResult.Failure(ApiResultErrorType.BadRequest),
                _ => ApiResult.Failure($"Failed to submit feedback: {errorContent}"),
            };
        }

        return ApiResult.Success();
    }

    /// <summary>
    ///     Suggests new categories based on all unclassified articles.
    /// </summary>
    /// <param name="cancellationToken">Cancellation token.</param>
    /// <returns>API result containing new category suggestions.</returns>
    public async Task<ApiResult<NewCategorySuggestion[]>> SuggestNewCategoriesAsync(CancellationToken cancellationToken = default)
    {
        var response = await this.client.GetAsync("/api/classification/suggestions", cancellationToken).ConfigureAwait(false);

        if (!response.IsSuccessStatusCode)
        {
            var errorContent = await response.Content.ReadAsStringAsync(cancellationToken).ConfigureAwait(false);
            return response.StatusCode switch
            {
                HttpStatusCode.NotFound => ApiResult<NewCategorySuggestion[]>.Failure(ApiResultErrorType.NotFound),
                HttpStatusCode.BadRequest => ApiResult<NewCategorySuggestion[]>.Failure(ApiResultErrorType.BadRequest),
                _ => ApiResult<NewCategorySuggestion[]>.Failure($"Failed to get suggestions: {errorContent}"),
            };
        }

        var suggestions = await response.Content.ReadFromJsonAsync<ApiSuggestion[]>(cancellationToken).ConfigureAwait(false);
        if (suggestions == null)
            return ApiResult<NewCategorySuggestion[]>.Failure("Failed to read response");

        // Convert API suggestions to web model
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
            Icon = s.SuggestedIcon,
        }).ToArray();

        return ApiResult<NewCategorySuggestion[]>.Success(convertedSuggestions);
    }

    /// <summary>
    ///     Automatically classifies all unclassified articles in batch.
    /// </summary>
    /// <param name="cancellationToken">Cancellation token.</param>
    /// <returns>API result containing batch classification results.</returns>
    public async Task<ApiResult<BatchClassificationResult[]>> BatchClassifyAsync(CancellationToken cancellationToken = default)
    {
        var response = await this.client.PostAsync("/api/classification/batch", null, cancellationToken).ConfigureAwait(false);

        if (!response.IsSuccessStatusCode)
        {
            var errorContent = await response.Content.ReadAsStringAsync(cancellationToken).ConfigureAwait(false);
            return response.StatusCode switch
            {
                HttpStatusCode.BadRequest => ApiResult<BatchClassificationResult[]>.Failure(ApiResultErrorType.BadRequest),
                _ => ApiResult<BatchClassificationResult[]>.Failure($"Failed batch classification: {errorContent}"),
            };
        }

        var results = await response.Content.ReadFromJsonAsync<BatchClassificationResult[]>(cancellationToken).ConfigureAwait(false);
        return results switch
        {
            null => ApiResult<BatchClassificationResult[]>.Failure("Failed to read response"),
            _ => ApiResult<BatchClassificationResult[]>.Success(results),
        };
    }
}
