namespace Watch.Manager.ApiService.Parameters.Categories;

using Microsoft.AspNetCore.Mvc;

using Watch.Manager.Service.Database.Abstractions;

/// <summary>
/// Represents the parameters required to assign a category to an article.
/// </summary>
public record AssignCategoryToArticleParameter
{
    /// <summary>
    /// Gets the ID of the category to assign.
    /// </summary>
    [FromRoute]
    public int CategoryId { get; init; }

    /// <summary>
    /// Gets the ID of the article to assign the category to.
    /// </summary>
    [FromRoute]
    public int ArticleId { get; init; }

    /// <summary>
    /// Gets service for category data operations.
    /// </summary>
    [FromServices]
    public required ICategoryStore CategoryStore { get; init; }

    /// <summary>
    /// Gets token to cancel the operation if needed.
    /// </summary>
    public CancellationToken CancellationToken { get; init; }
}
