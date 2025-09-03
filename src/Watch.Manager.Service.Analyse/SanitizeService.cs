#pragma warning disable KMEXP00
namespace Watch.Manager.Service.Analyse;

using AngleSharp;
using AngleSharp.Dom;

using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Microsoft.KernelMemory.AI;

using Watch.Manager.Service.Analyse.Models;

/// <summary>
///     Provides methods to sanitize and extract relevant content from website sources,
///     including HTML strings and URIs. Removes unnecessary elements and limits content size for embedding.
/// </summary>
internal sealed class SanitizeService
{
    private const int MaxEmbeddingTokens = 8192;
    private const int MaxTextLengthforEmbedding = 39000;

    private readonly ITextTokenizer tokenizer;
    private readonly ILogger<SanitizeService> logger;

    /// <summary>
    ///     Initializes a new instance of the <see cref="SanitizeService" /> class.
    /// </summary>
    /// <param name="serviceProvider">The service provider used to resolve dependencies.</param>
    public SanitizeService(IServiceProvider serviceProvider)
    {
        this.tokenizer = serviceProvider.GetRequiredService<ITextTokenizer>();
        this.logger = serviceProvider.GetRequiredService<ILogger<SanitizeService>>();
    }

    /// <summary>
    ///     Sanitizes the website content from the specified URI and extracts the head, body, and thumbnail.
    /// </summary>
    /// <param name="source">The URI of the website to sanitize.</param>
    /// <param name="cancellationToken">A token to monitor for cancellation requests.</param>
    /// <returns>
    ///     An <see cref="ExtractedSite" /> containing the sanitized head, body, and thumbnail URI.
    /// </returns>
    public async Task<ExtractedSite> SanitizeWebSiteSource(Uri source, CancellationToken cancellationToken)
    {
        var config = Configuration.Default.WithDefaultLoader();
        var context = BrowsingContext.New(config);
        var document = await context.OpenAsync(new Url(source.ToString()), cancellationToken).ConfigureAwait(false);

        return this.SanitizeInternal(document);
    }

    /// <summary>
    ///     Internal method to sanitize the provided HTML document, removing scripts, styles, and links,
    ///     and extracting the head, body, and thumbnail.
    /// </summary>
    /// <param name="document">The HTML document to sanitize.</param>
    /// <returns>
    ///     An <see cref="ExtractedSite" /> containing the sanitized head, body, and thumbnail URI.
    /// </returns>
    private ExtractedSite SanitizeInternal(IDocument document)
    {
        var body = document.DocumentElement.GetElementsByTagName("body").FirstOrDefault();
        var head = document.DocumentElement.GetElementsByTagName("head").FirstOrDefault();

        if (head is null || body is null)
            return new(string.Empty, string.Empty, new(""));

        head.QuerySelectorAll("script").ToList().ForEach(x => x.Remove());
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
