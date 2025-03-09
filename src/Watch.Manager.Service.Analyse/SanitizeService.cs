#pragma warning disable KMEXP00
namespace Watch.Manager.Service.Analyse;

using AngleSharp;

using Microsoft.Extensions.DependencyInjection;
using Microsoft.KernelMemory.AI;

internal class SanitizeService
{
    private const int MaxEmbeddingTokens = 8192;
    private const int MaxTextLengthforEmbedding = 39000;

    private readonly ITextTokenizer tokenizer;

    public SanitizeService(IServiceProvider serviceProvider)
    {
        this.tokenizer = serviceProvider.GetRequiredService<ITextTokenizer>();
    }

    public async Task<(string Head, string Body)> SanitizeWebSiteSource(string source, CancellationToken cancellationToken)
    {
        var config = Configuration.Default.WithDefaultLoader();
        var context = BrowsingContext.New(config);
        var document = await context.OpenAsync(req => req.Content(source), cancellationToken).ConfigureAwait(false);


        var body = document.DocumentElement.GetElementsByTagName("body").FirstOrDefault();
        var head = document.DocumentElement.GetElementsByTagName("head").FirstOrDefault();
        
        if (head is null || body is null)
            return (string.Empty, string.Empty);

        head.QuerySelectorAll("script").ToList().ForEach(x => x.Remove());
        head.QuerySelectorAll("link").ToList().ForEach(x => x.Remove());
        head.QuerySelectorAll("style").ToList().ForEach(x => x.Remove());
        
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

        if (this.tokenizer.CountTokens(bodyContent) > MaxEmbeddingTokens)
            bodyContent = bodyContent[..MaxTextLengthforEmbedding];

        return (headText, bodyContent);
    }
}
