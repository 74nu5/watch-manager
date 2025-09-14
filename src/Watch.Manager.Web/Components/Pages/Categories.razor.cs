namespace Watch.Manager.Web.Components.Pages;

using System.ComponentModel.DataAnnotations;

using Microsoft.AspNetCore.Components;
using Microsoft.FluentUI.AspNetCore.Components;

using Watch.Manager.Common;
using Watch.Manager.Web.Services;
using Watch.Manager.Web.Services.Models;

/// <summary>
///     Page for managing categories, including creation, editing, deletion, and AI-based suggestions.
/// </summary>
/// <remarks>
///     Initializes a new instance of the <see cref="Categories" /> class.
/// </remarks>
/// <param name="analyzeService">The <see cref="AnalyzeService" /> used to interact with category data and AI functionalities.</param>
/// <param name="toastService">The <see cref="IToastService" /> used to display notifications to the user.</param>
public sealed partial class Categories(AnalyzeService analyzeService, IToastService toastService) : ComponentBase, IDisposable
{
    private readonly AnalyzeService analyzeService = analyzeService;
    private readonly IToastService toastService = toastService;
    private readonly CancellationTokenSource cts = new();

    private CategoryModel[] categories = [];
    private IEnumerable<CategoryModel> hierarchicalCategories = [];
    private bool loading = true;
    private bool showInactive;

    // Dialog states
    private bool hideDialog = true;
    private bool hideDeleteDialog = true;
    private bool hideBatchDialog = true;
    private bool saving;
    private bool deleting;

    // Editing
    private CategoryModel? editingCategory;
    private CategoryModel? categoryToDelete;
    private CategoryFormModel categoryForm = new();
    private string keywordsText = string.Empty;
    private int? selectedParent;

    // AI Classification
    private bool suggestingCategories;
    private NewCategorySuggestion[] newCategorySuggestions = [];
    private bool batchClassifying;
    private BatchClassificationResult[] batchResults = [];

    /// <summary>
    /// Gets or sets the selected parent as string, synchronized with selectedParent.
    /// </summary>
    private string SelectedParentString
    {
        get => this.selectedParent?.ToString() ?? string.Empty;
        set
        {
            if (string.IsNullOrEmpty(value))
                this.selectedParent = null;
            else if (int.TryParse(value, out var parentId))
                this.selectedParent = parentId;
            else
                this.selectedParent = null;
        }
    }

    /// <inheritdoc />
    public void Dispose()
    {
        this.cts.Cancel();
        this.cts.Dispose();
    }

    /// <inheritdoc />
    protected override async Task OnInitializedAsync()
        => await this.LoadCategoriesAsync().ConfigureAwait(true);

    /// <summary>
    ///     Loads the list of categories from the service.
    /// </summary>
    private async Task LoadCategoriesAsync()
    {
        this.loading = true;

        try
        {
            this.categories = await this.analyzeService.GetCategoriesAsync(this.showInactive, this.cts.Token).ConfigureAwait(true);

            // Load hierarchical categories for tree view
            var hierarchicalResult = await this.analyzeService.GetCategoriesAsTreeAsync(this.showInactive, this.cts.Token).ConfigureAwait(true);
            if (hierarchicalResult.ApiResultErrorType == null && string.IsNullOrEmpty(hierarchicalResult.Error))
                this.hierarchicalCategories = hierarchicalResult.Result ?? [];
            else
                this.hierarchicalCategories = this.categories; // Fallback to flat list if hierarchical loading fails
        }
        catch (Exception ex)
        {
            this.toastService.ShowError($"Erreur lors du chargement des catégories: {ex.Message}");
        }
        finally
        {
            this.loading = false;
        }
    }

    /// <summary>
    ///     Handles the change event for showing inactive categories.
    /// </summary>
    private async Task OnShowInactiveChangedAsync()
        => await this.LoadCategoriesAsync().ConfigureAwait(true);

    /// <summary>
    ///     Displays the dialog for creating a new category.
    /// </summary>
    private void ShowCreateDialog()
    {
        this.editingCategory = null;
        this.categoryForm = new();
        this.keywordsText = string.Empty;
        this.SelectedParentString = string.Empty;
        this.hideDialog = false;
    }

    /// <summary>
    ///     Opens the edit dialog for the specified category.
    /// </summary>
    /// <param name="category">The category to edit.</param>
    private void EditCategory(CategoryModel category)
    {
        this.editingCategory = category;
        this.categoryForm = new()
        {
            Name = category.Name,
            Description = category.Description,
            Color = category.Color,
            Icon = category.Icon,
            ConfidenceThreshold = category.ConfidenceThreshold,
        };

        this.keywordsText = string.Join(", ", category.Keywords);
        this.SelectedParentString = category.ParentId?.ToString() ?? string.Empty;
        this.hideDialog = false;
    }

    /// <summary>
    ///     Opens the delete confirmation dialog for the specified category.
    /// </summary>
    /// <param name="category">The category to delete.</param>
    private void DeleteCategory(CategoryModel category)
    {
        this.categoryToDelete = category;
        this.hideDeleteDialog = false;
    }

    /// <summary>
    ///     Saves the category (create or update) using the form data.
    /// </summary>
    private async Task SaveCategoryAsync()
    {
        if (this.categoryForm.Name == null)
        {
            this.toastService.ShowError("Le nom de la catégorie est obligatoire");
            return;
        }

        this.saving = true;

        try
        {
            var keywords = string.IsNullOrWhiteSpace(this.keywordsText)
                                   ? Array.Empty<string>()
                                   : [.. this.keywordsText.Split(',').Select(k => k.Trim()).Where(k => !string.IsNullOrEmpty(k))];

            ApiResult<CategoryModel> result;

            if (this.editingCategory == null)
            {
                // Creation
                result = await this.analyzeService.CreateCategoryAsync(
                             this.categoryForm.Name,
                             this.categoryForm.Description,
                             this.categoryForm.Color,
                             this.categoryForm.Icon,
                             keywords,
                             this.selectedParent,
                             this.categoryForm.ConfidenceThreshold,
                             this.cts.Token).ConfigureAwait(true);
            }
            else
            {
                // Update
                result = await this.analyzeService.UpdateCategoryAsync(
                             this.editingCategory.Id,
                             this.categoryForm.Name,
                             this.categoryForm.Description,
                             this.categoryForm.Color,
                             this.categoryForm.Icon,
                             keywords,
                             this.selectedParent,
                             null, // isActive not changed here
                             this.categoryForm.ConfidenceThreshold,
                             this.cts.Token).ConfigureAwait(true);
            }

            if (result.ApiResultErrorType != null || !string.IsNullOrEmpty(result.Error))
            {
                this.toastService.ShowError($"Erreur: {result.Error}");
                return;
            }

            this.toastService.ShowSuccess(this.editingCategory == null ? "Catégorie créée avec succès" : "Catégorie modifiée avec succès");
            this.hideDialog = true;
            await this.LoadCategoriesAsync().ConfigureAwait(true);
        }
        catch (Exception ex)
        {
            this.toastService.ShowError($"Erreur: {ex.Message}");
        }
        finally
        {
            this.saving = false;
        }
    }

    /// <summary>
    ///     Confirms and performs the deletion of the selected category.
    /// </summary>
    private async Task ConfirmDeleteAsync()
    {
        if (this.categoryToDelete == null)
            return;

        this.deleting = true;

        try
        {
            var result = await this.analyzeService.DeleteCategoryAsync(this.categoryToDelete.Id, this.cts.Token).ConfigureAwait(true);

            if (result.ApiResultErrorType != null || !string.IsNullOrEmpty(result.Error))
            {
                this.toastService.ShowError($"Erreur lors de la suppression: {result.Error}");
                return;
            }

            this.toastService.ShowSuccess("Catégorie supprimée avec succès");
            this.hideDeleteDialog = true;
            this.categoryToDelete = null;
            await this.LoadCategoriesAsync().ConfigureAwait(true);
        }
        catch (Exception ex)
        {
            this.toastService.ShowError($"Erreur: {ex.Message}");
        }
        finally
        {
            this.deleting = false;
        }
    }

    /// <summary>
    ///     Hides the create/edit dialog and resets the editing state.
    /// </summary>
    private void HideDialog()
    {
        this.hideDialog = true;
        this.editingCategory = null;
    }

    /// <summary>
    ///     Hides the delete confirmation dialog and resets the deletion state.
    /// </summary>
    private void HideDeleteDialog()
    {
        this.hideDeleteDialog = true;
        this.categoryToDelete = null;
    }

    /// <summary>
    ///     Gets the list of available parent categories, excluding the current category and its children to prevent circular references.
    /// </summary>
    /// <returns>Enumerable of available parent categories.</returns>
    private IEnumerable<CategoryModel> GetAvailableParentCategories()
    {
        // Exclude the category being edited and its children to avoid circular references
        if (this.editingCategory == null)
            return this.categories.Where(c => c.IsActive);

        return this.categories.Where(c => c.IsActive && c.Id != this.editingCategory.Id && !this.IsChildOf(c, this.editingCategory));
    }

    /// <summary>
    ///     Determines if a category is a child of a potential parent category.
    /// </summary>
    /// <param name="category">The category to check.</param>
    /// <param name="potentialParent">The potential parent category.</param>
    /// <returns>True if <paramref name="category" /> is a child of <paramref name="potentialParent" />; otherwise, false.</returns>
    private bool IsChildOf(CategoryModel category, CategoryModel potentialParent)
    {
        var current = category;

        while (current?.ParentId != null)
        {
            if (current.ParentId == potentialParent.Id)
                return true;

            current = this.categories.FirstOrDefault(c => c.Id == current.ParentId);
        }

        return false;
    }

    // AI Classification Methods

    /// <summary>
    ///     Requests new category suggestions from the AI service.
    /// </summary>
    private async Task SuggestNewCategoriesAsync()
    {
        this.suggestingCategories = true;

        try
        {
            var result = await this.analyzeService.SuggestNewCategoriesAsync(this.cts.Token).ConfigureAwait(true);

            if (result.ApiResultErrorType != null || !string.IsNullOrEmpty(result.Error))
            {
                this.toastService.ShowError($"Erreur lors de la suggestion de catégories: {result.Error}");
                return;
            }

            this.newCategorySuggestions = result.Result ?? [];
            if (this.newCategorySuggestions.Length == 0)
                this.toastService.ShowInfo("Aucune nouvelle catégorie suggérée. Vos catégories actuelles semblent couvrir tous les articles.");
            else
                this.toastService.ShowSuccess($"{this.newCategorySuggestions.Length} nouvelles catégories suggérées");
        }
        catch (Exception ex)
        {
            this.toastService.ShowError($"Erreur: {ex.Message}");
        }
        finally
        {
            this.suggestingCategories = false;
        }
    }

    /// <summary>
    ///     Shows the dialog for batch classification and loads the results.
    /// </summary>
    private async Task ShowBatchClassificationDialogAsync()
    {
        this.batchClassifying = true;
        this.batchResults = [];
        this.hideBatchDialog = false;

        try
        {
            var result = await this.analyzeService.BatchClassifyAsync(this.cts.Token).ConfigureAwait(true);

            if (result.ApiResultErrorType != null || !string.IsNullOrEmpty(result.Error))
            {
                this.toastService.ShowError($"Erreur lors de la classification par lot: {result.Error}");
                return;
            }

            this.batchResults = result.Result ?? [];
            this.toastService.ShowSuccess($"{this.batchResults.Length} articles classifiés");
        }
        catch (Exception ex)
        {
            this.toastService.ShowError($"Erreur: {ex.Message}");
        }
        finally
        {
            this.batchClassifying = false;
        }
    }

    /// <summary>
    ///     Creates a new category from an AI suggestion.
    /// </summary>
    /// <param name="suggestion">The suggestion to use for creating the category.</param>
    private async Task CreateCategoryFromSuggestionAsync(NewCategorySuggestion suggestion)
    {
        try
        {
            var result = await this.analyzeService.CreateCategoryAsync(
                             suggestion.Name,
                             suggestion.Description,
                             suggestion.Color,
                             suggestion.Icon,
                             suggestion.Keywords,
                             null, // no parent for suggestions
                             suggestion.ConfidenceThreshold,
                             this.cts.Token).ConfigureAwait(true);

            if (result.ApiResultErrorType != null || !string.IsNullOrEmpty(result.Error))
            {
                this.toastService.ShowError($"Erreur lors de la création: {result.Error}");
                return;
            }

            this.toastService.ShowSuccess($"Catégorie '{suggestion.Name}' créée avec succès");

            // Remove the suggestion from the list
            this.newCategorySuggestions = [.. this.newCategorySuggestions.Where(s => s != suggestion)];

            // Reload categories
            await this.LoadCategoriesAsync().ConfigureAwait(true);
        }
        catch (Exception ex)
        {
            this.toastService.ShowError($"Erreur: {ex.Message}");
        }
    }

    /// <summary>
    ///     Dismisses a category suggestion from the list.
    /// </summary>
    /// <param name="suggestion">The suggestion to dismiss.</param>
    private void DismissSuggestion(NewCategorySuggestion suggestion)
        => this.newCategorySuggestions = [.. this.newCategorySuggestions.Where(s => s != suggestion)];

    /// <summary>
    ///     Hides the batch classification dialog and clears the results.
    /// </summary>
    private void HideBatchDialog()
    {
        this.hideBatchDialog = true;
        this.batchResults = [];
    }

    /// <summary>
    ///     Form model for category creation and editing.
    /// </summary>
    private sealed class CategoryFormModel
    {
        /// <summary>
        ///     Gets or sets the name of the category.
        /// </summary>
        [Required(ErrorMessage = "Le nom est obligatoire")]
        [StringLength(200, ErrorMessage = "Le nom ne peut pas dépasser 200 caractères")]
        public string? Name { get; set; }

        /// <summary>
        ///     Gets or sets the description of the category.
        /// </summary>
        [StringLength(1000, ErrorMessage = "La description ne peut pas dépasser 1000 caractères")]
        public string? Description { get; set; }

        /// <summary>
        ///     Gets or sets the color of the category in hexadecimal format.
        /// </summary>
        [RegularExpression(@"^#[0-9A-Fa-f]{6}$", ErrorMessage = "La couleur doit être au format hexadécimal (#000000)")]
        public string? Color { get; set; }

        /// <summary>
        ///     Gets or sets the icon of the category.
        /// </summary>
        [StringLength(50, ErrorMessage = "L'icône ne peut pas dépasser 50 caractères")]
        public string? Icon { get; set; }

        /// <summary>
        ///     Gets or sets the confidence threshold for automatic classification.
        /// </summary>
        [Range(0.0, 1.0, ErrorMessage = "Le seuil de confiance doit être entre 0 et 1")]
        public double? ConfidenceThreshold { get; set; }
    }
}
