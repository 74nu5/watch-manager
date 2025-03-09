namespace Watch.Manager.Service.Analyse.Abstractions;

public interface IWebSiteService
{
    Task<(string Head, string Body)> GetWebSiteSource(string url, CancellationToken cancellationToken);
}
