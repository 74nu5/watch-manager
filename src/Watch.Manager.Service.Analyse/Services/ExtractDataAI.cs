namespace Watch.Manager.Service.Analyse.Services;

using System.Text.Json;

using Microsoft.Extensions.AI;
using Microsoft.Extensions.DependencyInjection;

using Watch.Manager.Service.Analyse.Models;

internal class ExtractDataAI : IExtractDataAI
{
    private readonly IChatClient chatClient;

    private readonly ChatOptions chatOptions;

    private readonly IList<ChatMessage> messages;

    public ExtractDataAI(IServiceProvider serviceProvider)
    {
        this.chatClient = serviceProvider.GetRequiredService<IChatClient>();
        this.chatOptions = new()
        {
            Tools =
            [
                //AIFunctionFactory.Create(SearchArticles),
            ],
            ResponseFormat = ChatResponseFormat.Json,
        };

        this.messages =
        [
            new(ChatRole.System,
                """
                Tu es l'assistant de gestion de veille technologique d'un responsable technique en développement logiciel en C#.
                Tu réponds de la façon la plus factuelle possible.
                Tes réponses doivent être sous format json avec ce format : 
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
                 - une liste de 6 tags pertinants minimum
                 - un résumé en Markdown à la troisième personne en mettant en évidence les éléments suivants : 
                     - les éléments de code
                     - le ou les auteurs pour ce site web.
                 - le ou les auteurs pour ce site web.
                """),

            new(ChatRole.User, "Traduit le résumé en français sans traduire les notions techniques liés aux tags."),
        ];
    }

    public async Task<ExtractAnalyseModel?> ExtractDatasAsync(string content, CancellationToken cancellationToken = default)
    {
        this.messages.Add(new(ChatRole.User, content));
        var response = await this.chatClient.GetResponseAsync(this.messages, this.chatOptions, cancellationToken).ConfigureAwait(false);

        var messageText = response.Message.Text;
        if (messageText is not null)
            return JsonSerializer.Deserialize<ExtractAnalyseModel>(messageText);

        return null;
    }
}
