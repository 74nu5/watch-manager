namespace Watch.Manager.Web.Components.Pages;

using Microsoft.AspNetCore.Components;

using Watch.Manager.Web.Services;

public partial class Home
{
    private readonly ArticleModel model = new();
    private string chatResponse = string.Empty;

    [Inject]
    public AnalyzeService AnalyzeService { get; set; } = default!;

    private async Task AnalyseArticleHandler()
    {
        var responseMessage = await this.AnalyzeService.Analyse(this.model.Url).ConfigureAwait(false);
        this.chatResponse = $"[{responseMessage.Role.ToString().ToUpperInvariant()}]: {responseMessage.Content}";
    }

    private sealed class ArticleModel
    {
        public string Url { get; set; } = string.Empty;
    }
}
