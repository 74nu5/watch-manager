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
    private readonly IToastService toastService;
    private readonly AnalyzeService analyzeService;
    private readonly ArticleViewModel viewModel = new();
    private ExtractAnalyseModel? analyseResponse;
    private ArticleModel[] articles = [];
    private bool analyzeInProgress;

    private static readonly MarkdownPipeline MarkdownPipeline = new MarkdownPipelineBuilder()
                                                               .UseAdvancedExtensions()
                                                               .UseColorCode()
                                                               .Build();

    /// <summary>
    ///     Initializes a new instance of the <see cref="Home" /> class.
    /// </summary>
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

    private async Task SaveArticleHandlerAsync()
    {
        this.analyzeInProgress = true;
        var apiResult = await this.analyzeService.SaveArticleAsync(this.viewModel.Url).ConfigureAwait(true);

        if (apiResult.ApiResultErrorType == ApiResultErrorType.Conflict)
        {
            this.analyzeInProgress = false;
            this.toastService.ShowWarning("Url already save");
            return;
        }

        this.analyseResponse = apiResult.Result;
        await this.ReloadArticlesAsync().ConfigureAwait(true);
        this.analyzeInProgress = false;
    }

    private sealed class ArticleViewModel
    {
        public string Url { get; set; } = string.Empty;
    }
}
