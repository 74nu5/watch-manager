namespace Watch.Manager.ApiService.Parameters.Categories;

using Microsoft.AspNetCore.Mvc;

using Watch.Manager.ApiService.ViewModels;
using Watch.Manager.Service.Database.Abstractions;

/// <summary>
/// Represents the parameters required to update an existing category.
/// </summary>
public record UpdateCategoryParameter
{
    /// <summary>
    /// Gets the ID of the category to update.
    /// </summary>
    [FromRoute]
    public int Id { get; init; }

    /// <summary>
    /// Gets the model containing updated data for the category.
    /// </summary>
    [FromBody]
    public required UpdateCategoryModel Model { get; init; }

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
