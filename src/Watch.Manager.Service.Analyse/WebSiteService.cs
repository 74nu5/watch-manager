namespace Watch.Manager.Service.Analyse;

using Watch.Manager.Service.Analyse.Abstractions;
using Watch.Manager.Service.Analyse.Models;

internal sealed class WebSiteService(SanitizeService sanitizeService, SanitizeYoutubeService sanitizeYoutubeService) : IWebSiteService
{
    public async Task<ExtractedSite> GetWebSiteSource(Uri url, CancellationToken cancellationToken)
    {
        var client = new HttpClient(new HttpClientHandler
        {
            ServerCertificateCustomValidationCallback = HttpClientHandler.DangerousAcceptAnyServerCertificateValidator,
        });

        ExtractedSite sanitizeWebSiteSource;

        if (url.Host.Contains("youtube", StringComparison.CurrentCultureIgnoreCase))
            sanitizeWebSiteSource = await sanitizeYoutubeService.SanitizeWebSiteSource(url, cancellationToken).ConfigureAwait(false);
        else
            sanitizeWebSiteSource = await sanitizeService.SanitizeWebSiteSource(url, cancellationToken).ConfigureAwait(false);

        var thumbnail = await client.GetByteArrayAsync(sanitizeWebSiteSource.Thumbnail, cancellationToken).ConfigureAwait(false);
        var thumbnailBase64 = Convert.ToBase64String(thumbnail);

        return sanitizeWebSiteSource with
        {
            ThumbnailBase64 = thumbnailBase64,
        };
    }
}
