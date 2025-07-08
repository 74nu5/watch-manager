#pragma warning disable KMEXP00
namespace Watch.Manager.Service.Analyse;

using AngleSharp;

using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Microsoft.KernelMemory.AI;

using Watch.Manager.Service.Analyse.Models;

internal sealed class SanitizeService
{
    private const int MaxEmbeddingTokens = 8192;

    private const int MaxTextLengthforEmbedding = 39000;

    private readonly ITextTokenizer tokenizer;

    private readonly ILogger<SanitizeService> logger;

    public SanitizeService(IServiceProvider serviceProvider)
    {
        this.tokenizer = serviceProvider.GetRequiredService<ITextTokenizer>();
        this.logger = serviceProvider.GetRequiredService<ILogger<SanitizeService>>();
    }

    public async Task<ExtractedSite> SanitizeWebSiteSource(string source, CancellationToken cancellationToken)
    {
        var config = Configuration.Default.WithDefaultLoader();
        var context = BrowsingContext.New(config);
        var document = await context.OpenAsync(req => req.Content(source), cancellationToken).ConfigureAwait(false);


        var body = document.DocumentElement.GetElementsByTagName("body").FirstOrDefault();
        var head = document.DocumentElement.GetElementsByTagName("head").FirstOrDefault();

        if (head is null || body is null)
            return new(string.Empty, string.Empty, new(""));

        head.QuerySelectorAll("script").ToList().ForEach(x => x.Remove());
        head.QuerySelectorAll("link").ToList().ForEach(x => x.Remove());
        head.QuerySelectorAll("style").ToList().ForEach(x => x.Remove());

        var thumbnail = document.QuerySelector("meta[property='og:image']")?.GetAttribute("content") is { } thumbnailUrl
                                ? new(thumbnailUrl)
                                : new Uri("https://www.example.com/default-thumbnail.png");

        var headText = head.TextContent
                           .Replace("\t", string.Empty)
                           .Replace("\n", string.Empty)
                           .Replace("\r", string.Empty);

        body.QuerySelectorAll("script").ToList().ForEach(x => x.Remove());
        body.QuerySelectorAll("link").ToList().ForEach(x => x.Remove());
        body.QuerySelectorAll("style").ToList().ForEach(x => x.Remove());

        var bodyContent = body.TextContent
                              .Replace("\t", string.Empty)
                              .Replace("\n", string.Empty)
                              .Replace("\r", string.Empty);

        this.logger.LogInformation("Head text length: {Length}", headText.Length);
        this.logger.LogInformation("Body content length: {Length}", bodyContent.Length);
        this.logger.LogInformation("Initial body content token count: {TokenCount}", this.tokenizer.CountTokens(bodyContent));

        if (this.tokenizer.CountTokens(bodyContent) > MaxEmbeddingTokens)
            bodyContent = bodyContent[..MaxTextLengthforEmbedding];

        while (this.tokenizer.CountTokens(bodyContent) > MaxEmbeddingTokens)
            bodyContent = bodyContent[..^1];

        this.logger.LogInformation("Sanitized body content token count: {TokenCount}", this.tokenizer.CountTokens(bodyContent));

        return new(headText, bodyContent, thumbnail);
    }
}
