namespace Watch.Manager.Service.Analyse;

using AngleSharp;

internal class SanitizeService
{
    public async Task<string> SanitizeWebSiteSource(string source, CancellationToken cancellationToken)
    {
        var config = Configuration.Default.WithDefaultLoader();
        var context = BrowsingContext.New(config);
        var document = await context.OpenAsync(req => req.Content(source), cancellationToken).ConfigureAwait(false);

        document.QuerySelectorAll("script").ToList().ForEach(x => x.Remove());
        document.QuerySelectorAll("link").ToList().ForEach(x => x.Remove());

        return document.DocumentElement.TextContent;
    }
}
