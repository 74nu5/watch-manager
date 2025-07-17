namespace Watch.Manager.Web.Components.Pages;

using Markdig;

using Markdown.ColorCode;

using Microsoft.AspNetCore.Components;
using Microsoft.FluentUI.AspNetCore.Components;

using Watch.Manager.Common;
using Watch.Manager.Web.Services;
using Watch.Manager.Web.Services.Models;

/// <summary>
///     Home page.
/// </summary>
public partial class Home
{
    private static readonly MarkdownPipeline MarkdownPipeline = new MarkdownPipelineBuilder()
                                                               .UseAdvancedExtensions()
                                                               .UseColorCode()
                                                               .Build();

    private readonly IToastService toastService;
    private readonly AnalyzeService analyzeService;
    private readonly IConfiguration configuration;
    private readonly SearchArticleViewModel searchArticleViewModel = new();
    private readonly AddArticleViewModel addArticleViewModel = new();
    private ArticleModel[] articles = [];
    private bool analyzeInProgress;
    private bool searchInProgress;

    /// <summary>
    ///     Initializes a new instance of the <see cref="Home" /> class.
    /// </summary>
    /// <param name="toastService">The toast service.</param>
    /// <param name="analyzeService">The analyze service.</param>
    /// <param name="configuration">The configuration service, used to retrieve application settings such as the OpenAI API key.</param>
    public Home(IToastService toastService, AnalyzeService analyzeService, IConfiguration configuration)
    {
        this.toastService = toastService;
        this.analyzeService = analyzeService;
        this.configuration = configuration;
    }

    /// <summary>
    ///    Gets or sets the tag search term for articles.
    /// </summary>
    [SupplyParameterFromQuery(Name = "tag")]
    public string? TagSearch { get; set; }

    private string ApiUrl => this.configuration.GetValue<string>("services:apiservice:https:0", string.Empty);

    /// <inheritdoc />
    protected override async Task OnInitializedAsync()
        => await this.ReloadArticlesAsync().ConfigureAwait(true);

    private async Task ReloadArticlesAsync()
        => this.articles = await this.analyzeService.SearchArticleAsync(string.Empty, this.TagSearch).ConfigureAwait(true);

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
                break;
            default:
                throw new ArgumentOutOfRangeException();
        }

        await this.ReloadArticlesAsync().ConfigureAwait(true);
        this.addArticleViewModel.Url = string.Empty;
        this.analyzeInProgress = false;
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
