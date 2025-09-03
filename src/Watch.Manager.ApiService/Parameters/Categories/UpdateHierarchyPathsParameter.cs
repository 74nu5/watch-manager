namespace Watch.Manager.ApiService.Parameters.Categories;

using Microsoft.AspNetCore.Mvc;

using Watch.Manager.Service.Database.Abstractions;

/// <summary>
/// Paramètres pour mettre à jour les chemins hiérarchiques.
/// </summary>
public sealed record UpdateHierarchyPathsParameter
{
    /// <summary>
    /// Service de gestion des catégories.
    /// </summary>
    [FromServices]
    public required ICategoryStore CategoryStore { get; set; }

    /// <summary>
    /// Token d'annulation.
    /// </summary>
    public CancellationToken CancellationToken { get; set; } = default;
}
