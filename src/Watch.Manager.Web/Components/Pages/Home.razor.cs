namespace Watch.Manager.Web.Components.Pages;

using Microsoft.AspNetCore.Components;

using Watch.Manager.Web.Services;

public partial class Home
{
    private string chatResponse = string.Empty;

    private readonly ArticleModel model = new();

    [Inject]
    public AnalyzeService AnalyzeService { get; set; } = default!;
    private sealed class ArticleModel
    {
        public string Url { get; set; } = string.Empty;
    }

    private async Task AnalyseArticleHandler()
    {
        var responseMessage = await this.AnalyzeService.Analyse(this.model.Url);
        this.chatResponse = $"[{responseMessage.Role.ToString().ToUpperInvariant()}]: {responseMessage.Content}";

    }
}
