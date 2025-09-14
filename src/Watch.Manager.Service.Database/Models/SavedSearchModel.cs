namespace Watch.Manager.Service.Database.Models;

using System.Text.Json;

/// <summary>
///     Model representing a saved search for a user.
/// </summary>
public sealed record SavedSearchModel
{
    /// <summary>
    ///     Gets the unique identifier of the saved search.
    /// </summary>
    public int Id { get; init; }

    /// <summary>
    ///     Gets the name given to the saved search.
    /// </summary>
    public required string Name { get; init; }

    /// <summary>
    ///     Gets the optional description of the saved search.
    /// </summary>
    public string? Description { get; init; }

    /// <summary>
    ///     Gets the search filters serialized as JSON.
    /// </summary>
    public required string FiltersJson { get; init; }

    /// <summary>
    ///     Gets the creation date of the saved search (UTC).
    /// </summary>
    public DateTime CreatedAt { get; init; }

    /// <summary>
    ///     Gets the last usage date of the saved search (UTC).
    /// </summary>
    public DateTime LastUsedAt { get; init; }

    /// <summary>
    ///     Gets the number of times this search has been used.
    /// </summary>
    public int UseCount { get; init; }

    /// <summary>
    ///     Gets a value indicating whether this search is marked as favorite.
    /// </summary>
    public bool IsFavorite { get; init; }

    /// <summary>
    ///     Creates a new <see cref="SavedSearchModel" /> instance from filters.
    /// </summary>
    /// <param name="name">The name of the saved search.</param>
    /// <param name="filters">The filters to save.</param>
    /// <param name="description">The optional description.</param>
    /// <returns>
    ///     A new instance of <see cref="SavedSearchModel" /> initialized with the provided filters and metadata.
    /// </returns>
    public static SavedSearchModel FromFilters(string name, ArticleSearchFilters filters, string? description = null)
    {
        var filtersJson = JsonSerializer.Serialize(filters);

        return new()
        {
            Name = name,
            Description = description,
            FiltersJson = filtersJson,
            CreatedAt = DateTime.UtcNow,
            LastUsedAt = DateTime.UtcNow,
            UseCount = 0,
            IsFavorite = false,
        };
    }

    /// <summary>
    ///     Gets the deserialized search filters.
    /// </summary>
    /// <returns>
    ///     The <see cref="ArticleSearchFilters" /> instance if deserialization succeeds; otherwise, <c>null</c>.
    /// </returns>
    public ArticleSearchFilters? GetFilters()
    {
        try
        {
            return JsonSerializer.Deserialize<ArticleSearchFilters>(this.FiltersJson);
        }
        catch
        {
            return null;
        }
    }
}
