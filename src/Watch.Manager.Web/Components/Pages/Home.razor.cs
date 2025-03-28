namespace Watch.Manager.Web.Components.Pages;

using Markdig;

using Markdown.ColorCode;

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
    public Home(IToastService toastService, AnalyzeService analyzeService)
    {
        this.toastService = toastService;
        this.analyzeService = analyzeService;
    }

    /// <inheritdoc />
    protected override async Task OnInitializedAsync()
        => await this.ReloadArticlesAsync().ConfigureAwait(true);

    private async Task ReloadArticlesAsync()
        => this.articles = await this.analyzeService.SearchArticleAsync(string.Empty).ConfigureAwait(true);

    private async Task SearchArticleHandlerAsync()
    {
        this.searchInProgress = true;
        var apiResult = await this.analyzeService.SearchArticleAsync(this.searchArticleViewModel.Terms).ConfigureAwait(true);

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

        if (apiResult.ApiResultErrorType == ApiResultErrorType.Conflict)
        {
            this.analyzeInProgress = false;
            this.toastService.ShowWarning("Url déjà sauvegardée");
            return;
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
