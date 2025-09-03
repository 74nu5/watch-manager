namespace Watch.Manager.Web.Components;

using Markdig;

using Markdown.ColorCode;

using Microsoft.AspNetCore.Components;

using Watch.Manager.Web.Services.Models;

/// <summary>
/// Composant pour afficher un article avec fonctionnalités de classification IA.
/// </summary>
public sealed partial class Article : ComponentBase
{
    private static readonly MarkdownPipeline MarkdownPipeline = new MarkdownPipelineBuilder()
                                                               .UseAdvancedExtensions()
                                                               .UseColorCode()
                                                               .Build();

    private readonly IConfiguration configuration;
    private bool showMore;
    private bool showClassification;

    /// <summary>
    /// Initialise une nouvelle instance de la classe <see cref="Article"/>.
    /// </summary>
    /// <param name="configuration">Configuration de l'application.</param>
    public Article(IConfiguration configuration)
        => this.configuration = configuration;

    /// <summary>
    /// Modèle de l'article à afficher.
    /// </summary>
    [Parameter]
    public required ArticleModel ArticleModel { get; set; }

    /// <summary>
    /// Callback déclenché quand l'article est mis à jour.
    /// </summary>
    [Parameter]
    public EventCallback OnArticleUpdated { get; set; }

    private string ApiUrl => this.configuration.GetValue("services:apiservice:https:0", string.Empty);

    private string GetFullMarkdownContent(string content)
        => Markdown.ToHtml(content, MarkdownPipeline);

    private string GetShortMarkdownContent(string content)
    {
        const int Length = 300;

        if (string.IsNullOrEmpty(content) || content.Length <= Length)
            return content;

        return Markdown.ToHtml(content[..Length] + "...", MarkdownPipeline);
    }

    private void ToggleShowMore()
        => this.showMore = !this.showMore;

    private async Task OnCategoryAssignedAsync()
    {
        // Notifier le parent que l'article a été mis à jour (nouvelles catégories assignées)
        await this.OnArticleUpdated.InvokeAsync().ConfigureAwait(false);
    }
}
