namespace Watch.Manager.Web.Services;

using System.Net.Http.Json;

using AngleSharp;

using Azure;
using Azure.AI.OpenAI;

using Microsoft.Extensions.Configuration;

using IConfiguration = Microsoft.Extensions.Configuration.IConfiguration;

public class AnalyzeService
{
    private readonly IConfiguration configuration;
    private readonly HttpClient client;

    public AnalyzeService(IConfiguration configuration, HttpClient client)
    {
        this.configuration = configuration;
        this.client = client;
    }

    public async Task AnalyzeArticleAsync(string url, CancellationToken cancellationToken = default)
    {
        var response = await this.client.PostAsJsonAsync("/analyze", new { url }, cancellationToken).ConfigureAwait(false);
        _ = response.EnsureSuccessStatusCode();
    }

    public async Task<string> GetAnalysisResultAsync(CancellationToken cancellationToken = default)
    {
        var response = await this.client.GetAsync("/analyze", cancellationToken).ConfigureAwait(false);
        _ = response.EnsureSuccessStatusCode();
        return await response.Content.ReadAsStringAsync(cancellationToken).ConfigureAwait(false);
    }

    public async Task<ChatResponseMessage> Analyse(string url, CancellationToken cancellationToken = default)
    {
        var contentSite = await this.ExtractTextFromUrlAsync(url, cancellationToken).ConfigureAwait(false);

        return await this.ExtractTagsAsync(contentSite, cancellationToken).ConfigureAwait(false);
    }

    private async Task<string> ExtractTextFromUrlAsync(string url, CancellationToken cancellationToken)
    {
        var httpResult = await this.client.GetStringAsync(url, cancellationToken).ConfigureAwait(false);
        var config = Configuration.Default.WithDefaultLoader();
        var context = BrowsingContext.New(config);
        var document = await context.OpenAsync(req => req.Content(httpResult), cancel: cancellationToken).ConfigureAwait(false);
        
        document.QuerySelectorAll("script").ToList().ForEach(x => x.Remove());
        document.QuerySelectorAll("link").ToList().ForEach(x => x.Remove());

        return document.DocumentElement.TextContent;
    }

    private async Task<ChatResponseMessage> ExtractTagsAsync(string content, CancellationToken cancellationToken = default)
    {
        var openApiKey = this.configuration.GetValue("az-open-ai:key", string.Empty);
        var openApiUrl = this.configuration.GetValue("az-open-ai:url", string.Empty);
        if (string.IsNullOrWhiteSpace(openApiKey))
            throw new InvalidOperationException("OpenAI key is missing");

        if (string.IsNullOrWhiteSpace(openApiUrl))
            throw new InvalidOperationException("OpenAI url is missing");

        var openAiClient = new OpenAIClient(new(openApiUrl), new AzureKeyCredential(openApiKey));
        var response = await openAiClient.GetChatCompletionsAsync(new()
        {
            DeploymentName = "TanusGpt35T16k",
            Messages =
            {
                new ChatRequestSystemMessage("""
                                             Tu es l'assistant de gestion de veille technologique d'un responsable technique en développement logiciel en C#.
                                             Tu réponds de la façon la plus factuelle possible.
                                             Tes réponses doivent être sous format json.
                                             """),
                new ChatRequestUserMessage("""
                                           Donne-moi les informations suivantes : 
                                            - une liste de 6 tags pertinants minimum
                                            - un résumé en Markdown à la troisième personne en mettant en évidence les éléments suivants : 
                                                - les éléments de code
                                                - le ou les auteurs pour ce site web.
                                            - le ou les auteurs pour ce site web.
                                           """),
                new ChatRequestUserMessage("Traduit le résumé en français sans traduire les notions techniques liés aux tags."),
                new ChatRequestUserMessage(content),
            },
        }, cancellationToken).ConfigureAwait(false);

        return response.Value.Choices[0].Message;
    }
}
