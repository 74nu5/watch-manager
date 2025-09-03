namespace Watch.Manager.Service.Analyse.Services;

using System.Text.Json;

using Microsoft.Extensions.AI;
using Microsoft.Extensions.DependencyInjection;

using Watch.Manager.Service.Analyse.Models;

/// <summary>
///     Service for extracting and analyzing article data using an AI chat client.
/// </summary>
internal sealed class ExtractDataAI : IExtractDataAI
{
    private readonly IChatClient chatClient;
    private readonly ChatOptions chatOptions;
    private readonly IList<ChatMessage> messages;

    /// <summary>
    ///     Initializes a new instance of the <see cref="ExtractDataAI" /> class.
    /// </summary>
    /// <param name="serviceProvider">The service provider used to resolve dependencies.</param>
    public ExtractDataAI(IServiceProvider serviceProvider)
    {
        this.chatClient = serviceProvider.GetRequiredService<IChatClient>();
        this.chatOptions = new()
        {
            Tools =
            [
                //AIFunctionFactory.Create(this.GetTags),
            ],
            ResponseFormat = ChatResponseFormat.Json,
        };

        this.messages =
        [
            new(ChatRole.System,
                """
                Tu es l'assistant de gestion de veille technologique d'un responsable technique en développement logiciel en C#.
                Tu réponds de la façon la plus factuelle possible.
                Tes réponses doivent être sous format json avec ce format (TOUS les champs sont OBLIGATOIRE) : 
                {
                    "Title": "Titre de l'article",
                    "Tags": ["dotnet", "azure"],
                    "Summary": "Lorem ipsum...",
                    "Authors": ["John Doe"]
                }
                """),
            new(ChatRole.User,
                """
                Donne-moi les informations suivantes : 
                 - une liste de 6 tags pertinants au regard du contenu de l'article minimum.
                 - un résumé en Markdown à la troisième personne en mettant en évidence les éléments suivants : 
                 - les éléments de code
                 - le ou les auteurs pour ce site web.
                """),

            new(ChatRole.User, "Traduit le résumé en français sans traduire les notions techniques liés aux tags."),
        ];
    }

    /// <summary>
    ///     Extracts and analyzes data from the provided article content using the AI chat client.
    /// </summary>
    /// <param name="content">The article content to analyze.</param>
    /// <param name="cancellationToken">A token to monitor for cancellation requests.</param>
    /// <returns>
    ///     An <see cref="ExtractAnalyseModel" /> containing the extracted analysis data, or <see langword="null" /> if extraction fails.
    /// </returns>
    public async Task<ExtractAnalyseModel?> ExtractDatasAsync(string content, CancellationToken cancellationToken = default)
    {
        this.messages.Add(new(ChatRole.User, content));
        var response = await this.chatClient.GetResponseAsync(this.messages, this.chatOptions, cancellationToken).ConfigureAwait(false);

        var messageText = response.Messages.FirstOrDefault()?.Text;
        return messageText is not null ? JsonSerializer.Deserialize<ExtractAnalyseModel>(messageText) : null;
    }
}
