namespace Watch.Manager.Service.Database.Entities;

using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

/// <summary>
///     Entity representing a saved search in the database.
/// </summary>
[Table("SavedSearches")]
public sealed class SavedSearch
{
    /// <summary>
    ///     Gets or sets identifiant unique de la recherche sauvegardée.
    /// </summary>
    [Key]
    public int Id { get; set; }

    /// <summary>
    ///     Gets or sets name given to the saved search.
    /// </summary>
    [Required]
    [StringLength(200)]
    public required string Name { get; set; }

    /// <summary>
    ///     Gets or sets optional description of the search.
    /// </summary>
    [StringLength(1000)]
    public string? Description { get; set; }

    /// <summary>
    ///     Gets or sets search filters serialized in JSON format.
    /// </summary>
    [Required]
    [Column(TypeName = "nvarchar(max)")]
    // ReSharper disable once EntityFramework.ModelValidation.UnlimitedStringLength
    public required string FiltersJson { get; set; }

    /// <summary>
    ///     Gets or sets creation date of the saved search (UTC).
    /// </summary>
    [Required]
    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

    /// <summary>
    ///     Gets or sets last usage date of the saved search (UTC).
    /// </summary>
    [Required]
    public DateTime LastUsedAt { get; set; } = DateTime.UtcNow;

    /// <summary>
    ///     Gets or sets number of times this search has been used.
    /// </summary>
    public int UseCount { get; set; }

    /// <summary>
    ///     Gets or sets a value indicating whether indicates whether this search is marked as favorite.
    /// </summary>
    public bool IsFavorite { get; set; }
}
