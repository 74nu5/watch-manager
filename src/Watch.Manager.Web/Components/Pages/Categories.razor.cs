using Microsoft.AspNetCore.Components;
using Microsoft.FluentUI.AspNetCore.Components;
using System.ComponentModel.DataAnnotations;
using Watch.Manager.Web.Services;
using Watch.Manager.Web.Services.Models;
using Watch.Manager.Common;

namespace Watch.Manager.Web.Components.Pages;

/// <summary>
/// Page de gestion des catégories.
/// </summary>
public partial class Categories : ComponentBase, IDisposable
{
    private readonly AnalyzeService analyzeService;
    private readonly IToastService toastService;
    private readonly CancellationTokenSource cts = new();

    private CategoryModel[] categories = [];
    private bool loading = true;
    private bool showInactive = false;

    // Dialog states
    private bool hideDialog = true;
    private bool hideDeleteDialog = true;
    private bool hideBatchDialog = true;
    private bool saving = false;
    private bool deleting = false;

    // Editing
    private CategoryModel? editingCategory = null;
    private CategoryModel? categoryToDelete = null;
    private CategoryFormModel categoryForm = new();
    private string keywordsText = string.Empty;
    private int? selectedParent = null;

    // AI Classification
    private bool suggestingCategories = false;
    private NewCategorySuggestion[] newCategorySuggestions = [];
    private bool batchClassifying = false;
    private BatchClassificationResult[] batchResults = [];

    /// <summary>
    /// Constructeur.
    /// </summary>
    public Categories(AnalyzeService analyzeService, IToastService toastService)
    {
        this.analyzeService = analyzeService;
        this.toastService = toastService;
    }

    /// <inheritdoc />
    public void Dispose()
    {
        cts.Cancel();
        cts.Dispose();
    }

    /// <inheritdoc />
    protected override async Task OnInitializedAsync()
    {
        await LoadCategoriesAsync();
    }

    private async Task LoadCategoriesAsync()
    {
        loading = true;
        try
        {
            categories = await analyzeService.GetCategoriesAsync(showInactive, cts.Token);
        }
        catch (Exception ex)
        {
            toastService.ShowError($"Erreur lors du chargement des catégories: {ex.Message}");
        }
        finally
        {
            loading = false;
        }
    }

    private async Task OnShowInactiveChanged()
    {
        await LoadCategoriesAsync();
    }

    private void ShowCreateDialog()
    {
        editingCategory = null;
        categoryForm = new CategoryFormModel();
        keywordsText = string.Empty;
        selectedParent = null;
        hideDialog = false;
    }

    private void EditCategory(CategoryModel category)
    {
        editingCategory = category;
        categoryForm = new CategoryFormModel
        {
            Name = category.Name,
            Description = category.Description,
            Color = category.Color,
            Icon = category.Icon,
            ConfidenceThreshold = category.ConfidenceThreshold
        };
        keywordsText = string.Join(", ", category.Keywords);
        selectedParent = category.ParentId;
        hideDialog = false;
    }

    private void DeleteCategory(CategoryModel category)
    {
        categoryToDelete = category;
        hideDeleteDialog = false;
    }

    private async Task ToggleActiveCategory(CategoryModel category)
    {
        try
        {
            var result = await analyzeService.UpdateCategoryAsync(
                category.Id,
                isActive: !category.IsActive,
                cancellationToken: cts.Token);

            if (result.ApiResultErrorType != null || !string.IsNullOrEmpty(result.Error))
            {
                toastService.ShowError($"Erreur lors de la modification: {result.Error}");
                return;
            }

            toastService.ShowSuccess($"Catégorie {(category.IsActive ? "désactivée" : "activée")} avec succès");
            await LoadCategoriesAsync();
        }
        catch (Exception ex)
        {
            toastService.ShowError($"Erreur: {ex.Message}");
        }
    }

    private async Task SaveCategory()
    {
        if (categoryForm.Name == null)
        {
            toastService.ShowError("Le nom de la catégorie est obligatoire");
            return;
        }

        saving = true;
        try
        {
            var keywords = string.IsNullOrWhiteSpace(keywordsText)
                ? Array.Empty<string>()
                : keywordsText.Split(',').Select(k => k.Trim()).Where(k => !string.IsNullOrEmpty(k)).ToArray();

            ApiResult<CategoryModel> result;

            if (editingCategory == null)
            {
                // Création
                result = await analyzeService.CreateCategoryAsync(
                    categoryForm.Name,
                    categoryForm.Description,
                    categoryForm.Color,
                    categoryForm.Icon,
                    keywords,
                    selectedParent,
                    categoryForm.ConfidenceThreshold,
                    cts.Token);
            }
            else
            {
                // Modification
                result = await analyzeService.UpdateCategoryAsync(
                    editingCategory.Id,
                    categoryForm.Name,
                    categoryForm.Description,
                    categoryForm.Color,
                    categoryForm.Icon,
                    keywords,
                    selectedParent,
                    null, // isActive not changed here
                    categoryForm.ConfidenceThreshold,
                    cts.Token);
            }

            if (result.ApiResultErrorType != null || !string.IsNullOrEmpty(result.Error))
            {
                toastService.ShowError($"Erreur: {result.Error}");
                return;
            }

            toastService.ShowSuccess(editingCategory == null ? "Catégorie créée avec succès" : "Catégorie modifiée avec succès");
            hideDialog = true;
            await LoadCategoriesAsync();
        }
        catch (Exception ex)
        {
            toastService.ShowError($"Erreur: {ex.Message}");
        }
        finally
        {
            saving = false;
        }
    }

    private async Task ConfirmDelete()
    {
        if (categoryToDelete == null) return;

        deleting = true;
        try
        {
            var result = await analyzeService.DeleteCategoryAsync(categoryToDelete.Id, cts.Token);

            if (result.ApiResultErrorType != null || !string.IsNullOrEmpty(result.Error))
            {
                toastService.ShowError($"Erreur lors de la suppression: {result.Error}");
                return;
            }

            toastService.ShowSuccess("Catégorie supprimée avec succès");
            hideDeleteDialog = true;
            categoryToDelete = null;
            await LoadCategoriesAsync();
        }
        catch (Exception ex)
        {
            toastService.ShowError($"Erreur: {ex.Message}");
        }
        finally
        {
            deleting = false;
        }
    }

    private void HideDialog()
    {
        hideDialog = true;
        editingCategory = null;
    }

    private void HideDeleteDialog()
    {
        hideDeleteDialog = true;
        categoryToDelete = null;
    }

    private IEnumerable<CategoryModel> GetAvailableParentCategories()
    {
        // Exclure la catégorie en cours d'édition et ses enfants pour éviter les références circulaires
        if (editingCategory == null)
        {
            return categories.Where(c => c.IsActive);
        }

        return categories.Where(c => c.IsActive && c.Id != editingCategory.Id && !IsChildOf(c, editingCategory));
    }

    private bool IsChildOf(CategoryModel category, CategoryModel potentialParent)
    {
        var current = category;
        while (current?.ParentId != null)
        {
            if (current.ParentId == potentialParent.Id)
                return true;
            current = categories.FirstOrDefault(c => c.Id == current.ParentId);
        }
        return false;
    }

    // AI Classification Methods
    private async Task SuggestNewCategories()
    {
        suggestingCategories = true;
        try
        {
            var result = await analyzeService.SuggestNewCategoriesAsync(cts.Token);

            if (result.ApiResultErrorType != null || !string.IsNullOrEmpty(result.Error))
            {
                toastService.ShowError($"Erreur lors de la suggestion de catégories: {result.Error}");
                return;
            }

            newCategorySuggestions = result.Result ?? [];
            if (newCategorySuggestions.Length == 0)
            {
                toastService.ShowInfo("Aucune nouvelle catégorie suggérée. Vos catégories actuelles semblent couvrir tous les articles.");
            }
            else
            {
                toastService.ShowSuccess($"{newCategorySuggestions.Length} nouvelles catégories suggérées");
            }
        }
        catch (Exception ex)
        {
            toastService.ShowError($"Erreur: {ex.Message}");
        }
        finally
        {
            suggestingCategories = false;
        }
    }

    private async Task ShowBatchClassificationDialog()
    {
        batchClassifying = true;
        batchResults = [];
        hideBatchDialog = false;

        try
        {
            var result = await analyzeService.BatchClassifyAsync(cts.Token);

            if (result.ApiResultErrorType != null || !string.IsNullOrEmpty(result.Error))
            {
                toastService.ShowError($"Erreur lors de la classification par lot: {result.Error}");
                return;
            }

            batchResults = result.Result ?? [];
            toastService.ShowSuccess($"{batchResults.Length} articles classifiés");
        }
        catch (Exception ex)
        {
            toastService.ShowError($"Erreur: {ex.Message}");
        }
        finally
        {
            batchClassifying = false;
        }
    }

    private async Task CreateCategoryFromSuggestion(NewCategorySuggestion suggestion)
    {
        try
        {
            var result = await analyzeService.CreateCategoryAsync(
                suggestion.Name,
                suggestion.Description,
                suggestion.Color,
                suggestion.Icon,
                suggestion.Keywords,
                null, // pas de parent pour les suggestions
                suggestion.ConfidenceThreshold,
                cts.Token);

            if (result.ApiResultErrorType != null || !string.IsNullOrEmpty(result.Error))
            {
                toastService.ShowError($"Erreur lors de la création: {result.Error}");
                return;
            }

            toastService.ShowSuccess($"Catégorie '{suggestion.Name}' créée avec succès");

            // Retirer la suggestion de la liste
            newCategorySuggestions = newCategorySuggestions.Where(s => s != suggestion).ToArray();

            // Recharger les catégories
            await LoadCategoriesAsync();
        }
        catch (Exception ex)
        {
            toastService.ShowError($"Erreur: {ex.Message}");
        }
    }

    private void DismissSuggestion(NewCategorySuggestion suggestion)
    {
        newCategorySuggestions = newCategorySuggestions.Where(s => s != suggestion).ToArray();
    }

    private void HideBatchDialog()
    {
        hideBatchDialog = true;
        batchResults = [];
    }

    /// <summary>
    /// Modèle de formulaire pour les catégories.
    /// </summary>
    private class CategoryFormModel
    {
        [Required(ErrorMessage = "Le nom est obligatoire")]
        [StringLength(200, ErrorMessage = "Le nom ne peut pas dépasser 200 caractères")]
        public string? Name { get; set; }

        [StringLength(1000, ErrorMessage = "La description ne peut pas dépasser 1000 caractères")]
        public string? Description { get; set; }

        [RegularExpression(@"^#[0-9A-Fa-f]{6}$", ErrorMessage = "La couleur doit être au format hexadécimal (#000000)")]
        public string? Color { get; set; }

        [StringLength(50, ErrorMessage = "L'icône ne peut pas dépasser 50 caractères")]
        public string? Icon { get; set; }

        [Range(0.0, 1.0, ErrorMessage = "Le seuil de confiance doit être entre 0 et 1")]
        public double? ConfidenceThreshold { get; set; }
    }
}
