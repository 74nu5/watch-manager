namespace Watch.Manager.ApiService.Parameters.Categories;

using Microsoft.AspNetCore.Mvc;

using Watch.Manager.Service.Database.Abstractions;

/// <summary>
/// Paramètres pour récupérer les descendants d'une catégorie.
/// </summary>
public sealed record GetCategoryDescendantsParameter
{
    /// <summary>
    /// Identifiant de la catégorie.
    /// </summary>
    [FromRoute(Name = "id")]
    public required int Id { get; set; }

    /// <summary>
    /// Inclut les catégories inactives.
    /// </summary>
    [FromQuery]
    public bool IncludeInactive { get; set; } = false;

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
