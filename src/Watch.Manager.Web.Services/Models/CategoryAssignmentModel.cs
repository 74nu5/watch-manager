namespace Watch.Manager.Web.Services.Models;

/// <summary>
/// Modèle pour représenter l'assignation d'une catégorie à un article.
/// </summary>
public class CategoryAssignmentModel
{
    /// <summary>
    /// Identifiant de l'article.
    /// </summary>
    public int ArticleId { get; set; }

    /// <summary>
    /// Identifiant de la catégorie assignée.
    /// </summary>
    public int CategoryId { get; set; }

    /// <summary>
    /// Nom de la catégorie assignée.
    /// </summary>
    public required string CategoryName { get; set; }

    /// <summary>
    /// Score de confiance de l'assignation (0.0 à 1.0).
    /// </summary>
    public double ConfidenceScore { get; set; }

    /// <summary>
    /// Indique si l'assignation a été faite automatiquement.
    /// </summary>
    public bool IsAutomatic { get; set; }

    /// <summary>
    /// Date de l'assignation.
    /// </summary>
    public DateTime AssignedAt { get; set; }

    /// <summary>
    /// Raison de l'assignation.
    /// </summary>
    public string? Reason { get; set; }
}
