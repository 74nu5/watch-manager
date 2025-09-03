namespace Watch.Manager.ApiService.Parameters.Categories;

using Microsoft.AspNetCore.Mvc;

using Watch.Manager.Service.Database.Abstractions;

/// <summary>
/// Represents the parameters required to delete a category.
/// </summary>
public record DeleteCategoryParameter
{
    /// <summary>
    /// Gets the ID of the category to delete.
    /// </summary>
    [FromRoute]
    public int Id { get; init; }

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
