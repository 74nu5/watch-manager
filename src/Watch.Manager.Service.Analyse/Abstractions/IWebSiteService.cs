namespace Watch.Manager.Service.Analyse.Abstractions;

using Watch.Manager.Service.Analyse.Models;

public interface IWebSiteService
{
    Task<ExtractedSite> GetWebSiteSource(Uri url, CancellationToken cancellationToken);
}
