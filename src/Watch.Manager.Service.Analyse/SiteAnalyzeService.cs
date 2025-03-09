namespace Watch.Manager.Service.Analyse;

using Microsoft.Extensions.AI;

using Watch.Manager.Service.Analyse.Abstractions;
using Watch.Manager.Service.Analyse.Models;

internal class SiteAnalyzeService(IChatClient chatClient) : ISiteAnalyzeService
{
    public async Task<ExtractAnalyseModel?> ExtractTagsAsync(string content, CancellationToken cancellationToken = default)
            //var response = await openAiClient.GetChatCompletionsAsync(new()
            //                   {
            //                       DeploymentName = "TanusGpt35T16k",
            //                       Messages =
            //                       {
            //                           new ChatRequestSystemMessage("""
            //                                                        Tu es l'assistant de gestion de veille technologique d'un responsable technique en développement logiciel en C#.
            //                                                        Tu réponds de la façon la plus factuelle possible.
            //                                                        Tes réponses doivent être sous format json avec ce format : 
            //                                                        {
            //                                                            "tags": ["dotnet", "azure"],
            //                                                            "summary": "Lorem ipsum...",
            //                                                            "authors": ["John Doe"]
            //                                                        }
            //                                                        """),
            //                           new ChatRequestUserMessage("""
            //                                                      Donne-moi les informations suivantes : 
            //                                                       - une liste de 6 tags pertinants minimum
            //                                                       - un résumé en Markdown à la troisième personne en mettant en évidence les éléments suivants : 
            //                                                           - les éléments de code
            //                                                           - le ou les auteurs pour ce site web.
            //                                                       - le ou les auteurs pour ce site web.
            //                                                      """),
            //                           new ChatRequestUserMessage("Traduit le résumé en français sans traduire les notions techniques liés aux tags."),
            //                           new ChatRequestUserMessage(content),
            //                       },
            //                   },
            //                   cancellationToken).ConfigureAwait(false);
            //return JsonSerializer.Deserialize<ExtractAnalyseModel>(response.Value.Choices[0].Message.Content);
        => null;
}
