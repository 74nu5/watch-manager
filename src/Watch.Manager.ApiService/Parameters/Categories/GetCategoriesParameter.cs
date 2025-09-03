namespace Watch.Manager.ApiService.Parameters.Categories;

using Microsoft.AspNetCore.Mvc;

using Watch.Manager.Service.Database.Abstractions;

/// <summary>
/// Represents the parameters required to retrieve categories.
/// </summary>
public record GetCategoriesParameter
{
    /// <summary>
    /// Gets service for category data operations.
    /// </summary>
    [FromServices]
    public required ICategoryStore CategoryStore { get; init; }

    /// <summary>
    /// Gets a value indicating whether indicates whether to include inactive categories in the result.
    /// </summary>
    [FromQuery]
    public bool IncludeInactive { get; init; } = false;

    /// <summary>
    /// Gets token to cancel the operation if needed.
    /// </summary>
    public CancellationToken CancellationToken { get; init; }
}
