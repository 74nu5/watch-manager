namespace Watch.Manager.Web.Components;

using Markdig;

using Markdown.ColorCode;

using Microsoft.AspNetCore.Components;

using Watch.Manager.Web.Services.Models;

public partial class Article : ComponentBase
{
    private static readonly MarkdownPipeline MarkdownPipeline = new MarkdownPipelineBuilder()
                                                               .UseAdvancedExtensions()
                                                               .UseColorCode()
                                                               .Build();

    private readonly IConfiguration configuration;
    private bool showMore;

    /// <inheritdoc />
    public Article(IConfiguration configuration)
        => this.configuration = configuration;

    [Parameter]
    public ArticleModel ArticleModel { get; set; }

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
}
