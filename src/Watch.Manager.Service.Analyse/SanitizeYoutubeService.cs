#pragma warning disable KMEXP00
namespace Watch.Manager.Service.Analyse;

using AngleSharp;
using AngleSharp.Dom;

using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Microsoft.KernelMemory.AI;

using Watch.Manager.Service.Analyse.Models;

internal class SanitizeYoutubeService
{
    private const int MaxEmbeddingTokens = 8192;

    private const int MaxTextLengthforEmbedding = 39000;

    private readonly ITextTokenizer tokenizer;

    private readonly ILogger<SanitizeService> logger;
    private readonly HttpClient client;

    public SanitizeYoutubeService(IServiceProvider serviceProvider)
    {
        this.tokenizer = serviceProvider.GetRequiredService<ITextTokenizer>();
        this.client = serviceProvider.GetRequiredService<IHttpClientFactory>().CreateClient();
        this.logger = serviceProvider.GetRequiredService<ILogger<SanitizeService>>();
    }

    public async Task<ExtractedSite> SanitizeWebSiteSource(string source, CancellationToken cancellationToken)
    {
        var config = Configuration.Default.WithDefaultLoader();
        var context = BrowsingContext.New(config);
        var document = await context.OpenAsync(req => req.Content(source), cancellationToken).ConfigureAwait(false);


        return await this.SanitizeInternal(document).ConfigureAwait(false);
    }

    public async Task<ExtractedSite> SanitizeWebSiteSource(Uri source, CancellationToken cancellationToken)
    {
        var config = Configuration.Default.WithDefaultLoader();
        var context = BrowsingContext.New(config);
        var document = await context.OpenAsync(new Url(source.ToString()), cancellationToken).ConfigureAwait(false);

        return await this.SanitizeInternal(document).ConfigureAwait(false);
    }

    private async Task<ExtractedSite> SanitizeInternal(IDocument document)
    {
        var head = document.DocumentElement.GetElementsByTagName("head").FirstOrDefault();

        if (head is null)
            return new(string.Empty, string.Empty, new(""));

        head.QuerySelectorAll("script").ToList().ForEach(x => x.Remove());

        var alternate = string.Empty;
        var linkAlternateJsonSelector = head.QuerySelector("link[rel='alternate'][type^='application/json']");

        if (linkAlternateJsonSelector is not null)
        {
            var jsonContent = linkAlternateJsonSelector.GetAttribute("href");
            if (!string.IsNullOrEmpty(jsonContent))
                alternate = await this.client.GetStringAsync(new Uri(jsonContent)).ConfigureAwait(false);
        }

        head.QuerySelectorAll("link").ToList().ForEach(x => x.Remove());
        head.QuerySelectorAll("style").ToList().ForEach(x => x.Remove());

        var thumbnailSelector = document.QuerySelector("meta[property='og:image']");
        var thumbnail = thumbnailSelector?.GetAttribute("content") is { } thumbnailUrl
                                ? new(thumbnailUrl)
                                : new Uri("https://www.example.com/default-thumbnail.png");

        var headText = head.TextContent
                           .Replace("\t", string.Empty)
                           .Replace("\n", string.Empty)
                           .Replace("\r", string.Empty);

        this.logger.LogInformation("Head text length: {Length}", headText.Length);

        var descriptionSelector = document.QuerySelector("meta[name='description']");
        var description = descriptionSelector?.GetAttribute("content") ?? string.Empty;

        return new(headText, string.IsNullOrEmpty(alternate) ? description : alternate, thumbnail);
    }
}
