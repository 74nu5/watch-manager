namespace Watch.Manager.ApiService.Parameters.Categories;

using Microsoft.AspNetCore.Mvc;

using Watch.Manager.ApiService.ViewModels;
using Watch.Manager.Service.Database.Abstractions;

/// <summary>
/// Represents the parameters required to create a new category.
/// </summary>
public record CreateCategoryParameter
{
    /// <summary>
    /// Gets the model containing data for the new category.
    /// </summary>
    [FromBody]
    public required CreateCategoryModel Model { get; init; }

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
