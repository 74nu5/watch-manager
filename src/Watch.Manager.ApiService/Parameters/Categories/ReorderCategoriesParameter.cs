namespace Watch.Manager.ApiService.Parameters.Categories;

using Microsoft.AspNetCore.Mvc;

using Watch.Manager.Service.Database.Abstractions;

/// <summary>
/// Paramètres pour réorganiser l'ordre d'affichage des catégories.
/// </summary>
public sealed record ReorderCategoriesParameter
{
    /// <summary>
    /// Dictionnaire des ordres d'affichage par ID de catégorie.
    /// </summary>
    [FromBody]
    public required Dictionary<int, int> CategoryOrders { get; set; }

    /// <summary>
    /// Service de gestion des catégories.
    /// </summary>
    [FromServices]
    public required ICategoryStore CategoryStore { get; set; }

    /// <summary>
    /// Token d'annulation.
    /// </summary>
    public CancellationToken CancellationToken { get; set; } = CancellationToken.None;
}
