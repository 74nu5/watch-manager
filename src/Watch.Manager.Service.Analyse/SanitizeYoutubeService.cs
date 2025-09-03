namespace Watch.Manager.Service.Analyse;

using AngleSharp;
using AngleSharp.Dom;

using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;

using Watch.Manager.Service.Analyse.Models;

/// <summary>
///     Provides methods to sanitize and extract relevant content from YouTube website sources,
///     including HTML strings and URIs. Removes unnecessary elements and extracts metadata for embedding.
/// </summary>
internal sealed class SanitizeYoutubeService
{
    private readonly ILogger<SanitizeService> logger;
    private readonly HttpClient client;

    /// <summary>
    ///     Initializes a new instance of the <see cref="SanitizeYoutubeService" /> class.
    /// </summary>
    /// <param name="serviceProvider">The service provider used to resolve dependencies.</param>
    public SanitizeYoutubeService(IServiceProvider serviceProvider)
    {
        this.client = serviceProvider.GetRequiredService<IHttpClientFactory>().CreateClient();
        this.logger = serviceProvider.GetRequiredService<ILogger<SanitizeService>>();
    }

    /// <summary>
    ///     Sanitizes the YouTube website content from the specified URI and extracts the head, body, and thumbnail.
    /// </summary>
    /// <param name="source">The URI of the YouTube website to sanitize.</param>
    /// <param name="cancellationToken">A token to monitor for cancellation requests.</param>
    /// <returns>
    ///     An <see cref="ExtractedSite" /> containing the sanitized head, body, and thumbnail URI.
    /// </returns>
    public async Task<ExtractedSite> SanitizeWebSiteSource(Uri source, CancellationToken cancellationToken)
    {
        var config = Configuration.Default.WithDefaultLoader();
        var context = BrowsingContext.New(config);
        var document = await context.OpenAsync(new Url(source.ToString()), cancellationToken).ConfigureAwait(false);

        return await this.SanitizeInternal(document).ConfigureAwait(false);
    }

    /// <summary>
    ///     Internal method to sanitize the provided HTML document, removing scripts, styles, and links,
    ///     and extracting the head, body, and thumbnail.
    /// </summary>
    /// <param name="document">The HTML document to sanitize.</param>
    /// <returns>
    ///     An <see cref="ExtractedSite" /> containing the sanitized head, body, and thumbnail URI.
    /// </returns>
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
