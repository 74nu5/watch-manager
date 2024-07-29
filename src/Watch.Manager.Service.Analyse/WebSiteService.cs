namespace Watch.Manager.Service.Analyse;

using Watch.Manager.Service.Analyse.Abstractions;

internal class WebSiteService(IHttpClientFactory factory, SanitizeService sanitizeService) : IWebSiteService
{
    public async Task<string> GetWebSiteSource(string url, CancellationToken cancellationToken)
    {
        var client = factory.CreateClient();
        var httpResult = await client.GetStringAsync(url, cancellationToken).ConfigureAwait(false);
        return await sanitizeService.SanitizeWebSiteSource(httpResult, cancellationToken).ConfigureAwait(false);
    }
}
