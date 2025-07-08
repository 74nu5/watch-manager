namespace Watch.Manager.Service.Analyse;

using Watch.Manager.Service.Analyse.Abstractions;
using Watch.Manager.Service.Analyse.Models;

internal class WebSiteService(IHttpClientFactory factory, SanitizeService sanitizeService) : IWebSiteService
{
    public async Task<ExtractedSite> GetWebSiteSource(string url, CancellationToken cancellationToken)
    {
        var client = factory.CreateClient();
        var response = await client.GetAsync(url, cancellationToken).ConfigureAwait(false);

        if (!response.IsSuccessStatusCode)
            throw new HttpRequestException($"Failed to fetch the website source. Status code: {response.StatusCode}", null, response.StatusCode);

        var httpResult = await response.Content.ReadAsStringAsync(cancellationToken).ConfigureAwait(false);
        return await sanitizeService.SanitizeWebSiteSource(httpResult, cancellationToken).ConfigureAwait(false);
    }

}
