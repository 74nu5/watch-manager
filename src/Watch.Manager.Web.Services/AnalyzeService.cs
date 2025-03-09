namespace Watch.Manager.Web.Services;

using System.Net.Http.Json;

using IConfiguration = Microsoft.Extensions.Configuration.IConfiguration;

public class AnalyzeService
{
    private readonly IConfiguration configuration;

    private readonly HttpClient client;

    public AnalyzeService(IConfiguration configuration, HttpClient client)
    {
        this.configuration = configuration;
        this.client = client;
    }

    public async Task AnalyzeArticleAsync(string url, CancellationToken cancellationToken = default)
    {
        var response = await this.client.PostAsJsonAsync("/analyze", new { url }, cancellationToken).ConfigureAwait(false);
        _ = response.EnsureSuccessStatusCode();
    }

    public async Task<string> GetAnalysisResultAsync(CancellationToken cancellationToken = default)
    {
        var response = await this.client.GetAsync("/analyze", cancellationToken).ConfigureAwait(false);
        _ = response.EnsureSuccessStatusCode();
        return await response.Content.ReadAsStringAsync(cancellationToken).ConfigureAwait(false);
    }

    public async Task Analyse(string url, CancellationToken cancellationToken = default)
    {
        var contentSite = await this.ExtractTextFromUrlAsync(url, cancellationToken).ConfigureAwait(false);
    }

    private async Task<string> ExtractTextFromUrlAsync(string url, CancellationToken cancellationToken)
        => string.Empty;

    private async Task ExtractTagsAsync(string content, CancellationToken cancellationToken = default)
    {
    }
}
