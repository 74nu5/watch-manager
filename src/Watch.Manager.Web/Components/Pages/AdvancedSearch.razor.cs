namespace Watch.Manager.Web.Components.Pages;

using JetBrains.Annotations;

using Microsoft.AspNetCore.Components;

using Watch.Manager.Common.Enumerations;
using Watch.Manager.Web.Services;
using Watch.Manager.Web.Services.Models;
using Watch.Manager.Web.Services.Models.Search;

/// <summary>
///     Page de recherche avancée d'articles avec filtres multicritères.
/// </summary>
[UsedImplicitly]
public sealed partial class AdvancedSearch : ComponentBase
{
    private const int PageSize = 20;

    // État des filtres de recherche
    private readonly AdvancedSearchParameters searchFilters = new()
    {
        SortBy = ArticleSortBy.AnalyzeDate,
        SortOrder = SortOrder.Descending,
        IncludeFacets = true,
    };

    private readonly HashSet<int> selectedCategoryIds = [];
    private readonly List<string> selectedTags = [];
    private readonly List<string> selectedAuthors = [];

    // État de l'interface
    private AdvancedSearchResult? searchResult;
    private CategoryModel[]? availableCategories;
    private bool isLoading;

    [Inject]
    private AnalyzeService AnalyzeService { get; set; } = null!;

    /// <summary>
    ///     Propriété pour la gestion des FluentSelect du tri.
    /// </summary>
    private string SelectedSortBy
    {
        get => this.searchFilters.SortBy.ToString();
        set
        {
            if (!string.IsNullOrEmpty(value) && Enum.TryParse<ArticleSortBy>(value, out var sortBy))
                this.searchFilters.SortBy = sortBy;
        }
    }

    /// <summary>
    ///     Propriété pour la gestion des FluentSelect de l'ordre de tri.
    /// </summary>
    private string SelectedSortOrder
    {
        get => this.searchFilters.SortOrder.ToString();
        set
        {
            if (!string.IsNullOrEmpty(value) && Enum.TryParse<SortOrder>(value, out var sortOrder))
                this.searchFilters.SortOrder = sortOrder;
        }
    }

    /// <inheritdoc />
    protected override async Task OnInitializedAsync()
    {
        await this.LoadCategoriesAsync().ConfigureAwait(true);
        await this.ExecuteSearchAsync().ConfigureAwait(true);
    }

    /// <summary>
    ///     Charge la liste des catégories disponibles.
    /// </summary>
    private async Task LoadCategoriesAsync()
    {
        try
        {
            this.availableCategories = await this.AnalyzeService.GetCategoriesAsync().ConfigureAwait(true);
        }
        catch (Exception)
        {
            // Gestion d'erreur silencieuse pour ne pas bloquer l'interface
            this.availableCategories = [];
        }
    }

    /// <summary>
    ///     Exécute la recherche avec les filtres actuels.
    /// </summary>
    private async Task ExecuteSearchAsync()
    {
        this.isLoading = true;
        this.StateHasChanged();

        try
        {
            // Mise à jour des filtres avec les sélections actuelles
            this.searchFilters.CategoryIds = this.selectedCategoryIds.Count > 0 ? [.. this.selectedCategoryIds] : null;
            this.searchFilters.Tags = this.selectedTags.Count > 0 ? [.. this.selectedTags] : null;
            this.searchFilters.Authors = this.selectedAuthors.Count > 0 ? [.. this.selectedAuthors] : null;
            this.searchFilters.Limit = PageSize;

            this.searchResult = await this.AnalyzeService.AdvancedSearchArticleAsync(this.searchFilters).ConfigureAwait(true);
        }
        catch (Exception)
        {
            // En cas d'erreur, initialiser un résultat vide
            this.searchResult = new();
        }
        finally
        {
            this.isLoading = false;
            this.StateHasChanged();
        }
    }

    /// <summary>
    ///     Gère la saisie des termes de recherche au clavier.
    /// </summary>
    private async Task OnSearchTermsKeyPressAsync()
    {
        await this.OnFiltersChangedAsync().ConfigureAwait(true);
    }

    /// <summary>
    ///     Gère le changement de sélection d'une catégorie.
    /// </summary>
    private async Task OnCategorySelectionChangedAsync(int categoryId, bool isSelected)
    {
        _ = isSelected ? this.selectedCategoryIds.Add(categoryId) : this.selectedCategoryIds.Remove(categoryId);

        await this.OnFiltersChangedAsync().ConfigureAwait(true);
    }

    /// <summary>
    ///     Gère le toggle d'une catégorie.
    /// </summary>
    private async Task OnCategoryToggleAsync(int categoryId, bool isSelected)
    {
        if (isSelected)
            this.selectedCategoryIds.Add(categoryId);
        else
            this.selectedCategoryIds.Remove(categoryId);

        await this.OnFiltersChangedAsync().ConfigureAwait(true);
    }

    /// <summary>
    ///     Vérifie si une catégorie est sélectionnée.
    /// </summary>
    private bool IsSelectedCategory(int categoryId)
        => this.selectedCategoryIds.Contains(categoryId);

    /// <summary>
    ///     Active/désactive la sélection d'un tag.
    /// </summary>
    private async Task ToggleTagAsync(string tagName)
    {
        if (this.selectedTags.Contains(tagName))
            _ = this.selectedTags.Remove(tagName);
        else
            this.selectedTags.Add(tagName);

        await this.OnFiltersChangedAsync().ConfigureAwait(true);
    }

    /// <summary>
    ///     Vérifie si un tag est sélectionné.
    /// </summary>
    private bool IsSelectedTag(string tagName)
        => this.selectedTags.Contains(tagName);

    /// <summary>
    ///     Active/désactive la sélection d'un auteur.
    /// </summary>
    private async Task ToggleAuthorAsync(string authorName)
    {
        if (this.selectedAuthors.Contains(authorName))
            _ = this.selectedAuthors.Remove(authorName);
        else
            this.selectedAuthors.Add(authorName);

        await this.OnFiltersChangedAsync().ConfigureAwait(true);
    }

    /// <summary>
    ///     Vérifie si un auteur est sélectionné.
    /// </summary>
    private bool IsSelectedAuthor(string authorName)
        => this.selectedAuthors.Contains(authorName);

    /// <summary>
    ///     Gère le changement des filtres.
    /// </summary>
    private async Task OnFiltersChangedAsync()
    {
        // Réinitialiser la pagination lors du changement de filtres
        this.searchFilters.Offset = 0;
        await this.ExecuteSearchAsync().ConfigureAwait(true);
    }

    /// <summary>
    ///     Gère le changement du type de tri.
    /// </summary>
    private async Task OnSortByChangedAsync()
    {
        await this.OnFiltersChangedAsync().ConfigureAwait(true);
    }

    /// <summary>
    ///     Gère le changement de l'ordre de tri.
    /// </summary>
    private async Task OnSortOrderChangedAsync()
    {
        await this.OnFiltersChangedAsync().ConfigureAwait(true);
    }

    /// <summary>
    ///     Efface tous les filtres.
    /// </summary>
    private async Task ClearFiltersAsync()
    {
        this.searchFilters.SearchTerms = null;
        this.searchFilters.DateFrom = null;
        this.searchFilters.DateTo = null;
        this.searchFilters.MinScore = null;
        this.searchFilters.SortBy = ArticleSortBy.AnalyzeDate;
        this.searchFilters.SortOrder = SortOrder.Descending;
        this.searchFilters.Offset = 0;

        this.selectedCategoryIds.Clear();
        this.selectedTags.Clear();
        this.selectedAuthors.Clear();

        await this.ExecuteSearchAsync().ConfigureAwait(true);
    }

    /// <summary>
    ///     Change la page de résultats.
    /// </summary>
    private async Task ChangePageAsync(int pageNumber)
    {
        this.searchFilters.Offset = (pageNumber - 1) * PageSize;
        await this.ExecuteSearchAsync().ConfigureAwait(true);
    }

    // Méthodes helper pour FluentUI

    /// <summary>
    ///     Obtient la sélection d'une catégorie pour FluentCheckbox.
    /// </summary>
    private bool GetCategorySelection(int categoryId)
        => this.IsSelectedCategory(categoryId);

    /// <summary>
    ///     Obtient le label formaté pour une catégorie.
    /// </summary>
    private string GetCategoryLabel(CategoryModel category)
    {
        var facet = this.searchResult?.Facets?.Categories?.FirstOrDefault(f => f.CategoryId == category.Id);
        var count = facet?.Count ?? 0;
        var icon = !string.IsNullOrEmpty(category.Icon) ? $"{category.Icon} " : string.Empty;
        return $"{icon}{category.Name}" + (count > 0 ? $" ({count})" : string.Empty);
    }

    /// <summary>
    ///     Obtient les options de tri disponibles.
    /// </summary>
    private ArticleSortBy[] GetSortByOptions()
        => [ArticleSortBy.AnalyzeDate, ArticleSortBy.Title, ArticleSortBy.Score, ArticleSortBy.CategoryCount];

    /// <summary>
    ///     Obtient le texte d'affichage pour un type de tri.
    /// </summary>
    private string GetSortByText(ArticleSortBy sortBy)
        => sortBy switch
        {
            ArticleSortBy.AnalyzeDate => "Date d'analyse",
            ArticleSortBy.Title => "Titre",
            ArticleSortBy.Score => "Pertinence",
            ArticleSortBy.CategoryCount => "Nb. catégories",
            _ => sortBy.ToString()
        };

    /// <summary>
    ///     Obtient les options d'ordre de tri disponibles.
    /// </summary>
    private SortOrder[] GetSortOrderOptions()
        => [SortOrder.Descending, SortOrder.Ascending];

    /// <summary>
    ///     Obtient le texte d'affichage pour un ordre de tri.
    /// </summary>
    private string GetSortOrderText(SortOrder sortOrder)
        => sortOrder switch
        {
            SortOrder.Descending => "Décroissant",
            SortOrder.Ascending => "Croissant",
            _ => sortOrder.ToString()
        };
}
