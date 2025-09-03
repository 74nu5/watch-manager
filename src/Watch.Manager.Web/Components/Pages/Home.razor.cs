namespace Watch.Manager.Web.Components.Pages;

using Microsoft.AspNetCore.Components;
using Microsoft.FluentUI.AspNetCore.Components;

using Watch.Manager.Common;
using Watch.Manager.Web.Services;
using Watch.Manager.Web.Services.Models;

/// <summary>
///     Home page.
/// </summary>
public sealed partial class Home
{
    private readonly IToastService toastService;
    private readonly AnalyzeService analyzeService;
    private readonly SearchArticleViewModel searchArticleViewModel = new();
    private readonly AddArticleViewModel addArticleViewModel = new();
    private ArticleModel[] articles = [];
    private bool analyzeInProgress;
    private bool searchDisabled;
    private bool searchInProgress;
    private bool popoverIsVisible;

    /// <summary>
    ///     Initializes a new instance of the <see cref="Home" /> class.
    /// </summary>
    /// <param name="toastService">The toast service.</param>
    /// <param name="analyzeService">The analyze service.</param>
    public Home(IToastService toastService, AnalyzeService analyzeService)
    {
        this.toastService = toastService;
        this.analyzeService = analyzeService;
    }

    /// <summary>
    ///     Gets or sets the tag search term for articles.
    /// </summary>
    [SupplyParameterFromQuery(Name = "tag")]
    public string? TagSearch { get; set; }

    /// <summary>
    ///     Gets or sets the search query for articles.
    /// </summary>
    [SupplyParameterFromQuery(Name = "q")]
    public string? QuerySearch { get; set; }

    /// <inheritdoc />
    public override async Task SetParametersAsync(ParameterView parameters)
    {
        await base.SetParametersAsync(parameters).ConfigureAwait(true);

        if (!string.IsNullOrWhiteSpace(this.QuerySearch))
            this.searchArticleViewModel.Terms = this.QuerySearch;

        this.searchDisabled = !string.IsNullOrWhiteSpace(this.TagSearch);
    }

    /// <inheritdoc />
    protected override async Task OnParametersSetAsync()
        => await this.SearchArticleHandlerAsync().ConfigureAwait(true);

    private async Task ReloadArticlesAsync()
    {
        this.searchArticleViewModel.Terms = string.Empty;
        await this.SearchArticleHandlerAsync().ConfigureAwait(true);
        this.searchDisabled = false;
    }

    private async Task SearchArticleHandlerAsync()
    {
        this.searchInProgress = true;
        var apiResult = await this.analyzeService.SearchArticleAsync(this.searchArticleViewModel.Terms, this.TagSearch).ConfigureAwait(true);

        if (apiResult.Length == 0)
        {
            this.toastService.ShowWarning("Aucun résultat");
            this.articles = [];
            this.searchInProgress = false;
            return;
        }

        this.articles = apiResult;
        this.searchInProgress = false;
    }

    private async Task SaveArticleHandlerAsync()
    {
        this.analyzeInProgress = true;
        var apiResult = await this.analyzeService.SaveArticleAsync(this.addArticleViewModel.Url).ConfigureAwait(true);

        switch (apiResult.ApiResultErrorType)
        {
            case ApiResultErrorType.Conflict:
                this.analyzeInProgress = false;
                this.toastService.ShowWarning("Url déjà sauvegardée");
                return;
            case ApiResultErrorType.NotFound:
                this.analyzeInProgress = false;
                this.toastService.ShowError("Url non disponible");
                return;
            case ApiResultErrorType.None:
                // Article sauvé avec succès, afficher un message de succès avec info sur la classification
                this.toastService.ShowSuccess("Article sauvegardé avec succès ! 🤖 Classification automatique appliquée.");
                break;
            case ApiResultErrorType.BadRequest:
                this.analyzeInProgress = false;
                this.toastService.ShowError("Requête incorrecte");
                break;
            case ApiResultErrorType.Unauthorized:
                this.analyzeInProgress = false;
                this.toastService.ShowError("Non autorisé");
                break;
            case ApiResultErrorType.Forbidden:
                this.analyzeInProgress = false;
                this.toastService.ShowError("Url interdite");
                break;
            case ApiResultErrorType.InternalServerError:
                this.analyzeInProgress = false;
                this.toastService.ShowError("Erreur interne du serveur");
                break;
            case null:
                // Article sauvé avec succès, afficher un message de succès avec info sur la classification
                this.toastService.ShowSuccess("Article sauvegardé avec succès ! 🤖 Classification automatique appliquée.");
                break;
            default:
                throw new ArgumentOutOfRangeException();
        }

        await this.ReloadArticlesAsync().ConfigureAwait(true);
        this.addArticleViewModel.Url = string.Empty;
        this.analyzeInProgress = false;
        this.popoverIsVisible = false; // Fermer le popover après l'ajout
    }

    private async Task DismissTagAsync()
    {
        this.TagSearch = string.Empty;
        await this.ReloadArticlesAsync().ConfigureAwait(true);
    }

    private sealed class SearchArticleViewModel
    {
        public string Terms { get; set; } = string.Empty;
    }

    private sealed class AddArticleViewModel
    {
        public string Url { get; set; } = string.Empty;
    }
}
