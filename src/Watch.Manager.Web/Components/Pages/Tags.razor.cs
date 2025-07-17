namespace Watch.Manager.Web.Components.Pages;

using Microsoft.AspNetCore.Components;

using Watch.Manager.Web.Services;

public partial class Tags : ComponentBase, IDisposable, IAsyncDisposable
{
    private readonly AnalyzeService analyzeService;
    private string[] tags = [];
    private readonly CancellationTokenSource cts = new();

    /// <inheritdoc />
    public Tags(AnalyzeService analyzeService)
        => this.analyzeService = analyzeService;

    /// <inheritdoc />
    public void Dispose()
    {
        this.cts.Cancel();
        this.cts.Dispose();
    }

    /// <inheritdoc />
    public async ValueTask DisposeAsync()
    {
        await this.cts.CancelAsync().ConfigureAwait(false);
        this.cts.Dispose();
    }

    /// <inheritdoc />
    protected override async Task OnInitializedAsync()
        => this.tags = await this.analyzeService.GetTagsAsync(this.cts.Token).ConfigureAwait(true);
}
