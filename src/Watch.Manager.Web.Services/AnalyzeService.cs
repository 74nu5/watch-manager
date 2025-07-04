namespace Watch.Manager.Web.Services;

using System.Net.Http.Json;

using Watch.Manager.Common;
using Watch.Manager.Web.Services.Models;

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

    //public async Task<ExtractAnalyseModel?> AnalyzeArticleAsync(string url, CancellationToken cancellationToken = default)
    //{
    //    var response = await this.client.PostAsJsonAsync("/analyze", new { url }, cancellationToken).ConfigureAwait(false);
    //    _ = response.EnsureSuccessStatusCode();
    //    return await response.Content.ReadFromJsonAsync<ExtractAnalyseModel>(cancellationToken).ConfigureAwait(false);
    //}

    public async Task<ApiResult<ExtractAnalyseModel>>SaveArticleAsync(string url, CancellationToken cancellationToken = default)
    {
        var response = await this.client.PostAsJsonAsync("/api/articles/save", new { UriToAnalyze = url }, cancellationToken).ConfigureAwait(false);

        if (!response.IsSuccessStatusCode)
        {
            if (response.StatusCode == System.Net.HttpStatusCode.Conflict)
                return ApiResult<ExtractAnalyseModel>.Failure(ApiResultErrorType.Conflict);

            if (response.StatusCode == System.Net.HttpStatusCode.NotFound)
                return ApiResult<ExtractAnalyseModel>.Failure(ApiResultErrorType.NotFound);

            return ApiResult<ExtractAnalyseModel>.Failure("Failed to save article");
        }

        var analyseModel = await response.Content.ReadFromJsonAsync<ExtractAnalyseModel>(cancellationToken).ConfigureAwait(false);

        if (analyseModel is null)
            return ApiResult<ExtractAnalyseModel>.Failure("Failed to parse response");

        return ApiResult<ExtractAnalyseModel>.Success(analyseModel);
    }

    public async Task<ArticleModel[]> SearchArticleAsync(string text, CancellationToken cancellationToken = default)
    {
        var response = await this.client.GetAsync($"/api/articles/search?text={text}", cancellationToken).ConfigureAwait(false);
        _ = response.EnsureSuccessStatusCode();
        return await response.Content.ReadFromJsonAsync<ArticleModel[]>(cancellationToken).ConfigureAwait(false) ?? [];
    }

    //public async Task<string> GetAnalysisResultAsync(CancellationToken cancellationToken = default)
    //{
    //    var response = await this.client.GetAsync("/analyze", cancellationToken).ConfigureAwait(false);
    //    _ = response.EnsureSuccessStatusCode();
    //    return await response.Content.ReadAsStringAsync(cancellationToken).ConfigureAwait(false);
    //}

    //public async Task Analyse(string url, CancellationToken cancellationToken = default)
    //{
    //    var contentSite = await this.ExtractTextFromUrlAsync(url, cancellationToken).ConfigureAwait(false);
    //}

    //private async Task<string> ExtractTextFromUrlAsync(string url, CancellationToken cancellationToken)
    //    => string.Empty;

    //private async Task ExtractTagsAsync(string content, CancellationToken cancellationToken = default)
    //{
    //}
}
