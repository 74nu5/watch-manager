namespace Watch.Manager.Service.Analyse.Abstractions;

public interface IWebSiteService
{
    Task<string> GetWebSiteSource(string url, CancellationToken cancellationToken);
}
