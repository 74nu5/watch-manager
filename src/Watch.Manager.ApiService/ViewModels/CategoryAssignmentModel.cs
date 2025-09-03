namespace Watch.Manager.ApiService.ViewModels;

/// <summary>
/// Modèle pour représenter l'assignation d'une catégorie à un article.
/// </summary>
public class CategoryAssignmentModel
{
    /// <summary>
    /// Identifiant de la catégorie.
    /// </summary>
    public required int CategoryId { get; set; }

    /// <summary>
    /// Nom de la catégorie.
    /// </summary>
    public required string CategoryName { get; set; }

    /// <summary>
    /// Score de confiance de l'assignation.
    /// </summary>
    public double ConfidenceScore { get; set; }

    /// <summary>
    /// Indique si l'assignation a été faite automatiquement ou manuellement.
    /// </summary>
    public bool IsAutomatic { get; set; }

    /// <summary>
    /// Date de l'assignation.
    /// </summary>
    public DateTime AssignedAt { get; set; } = DateTime.UtcNow;
}
