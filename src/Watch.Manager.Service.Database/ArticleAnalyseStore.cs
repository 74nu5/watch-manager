namespace Watch.Manager.Service.Database;

using Microsoft.Azure.Cosmos;
using Microsoft.Extensions.Logging;
using Watch.Manager.Service.Database.Abstractions;
using Watch.Manager.Service.Database.Entities;

internal class ArticleAnalyseStore(ILogger<ArticleAnalyseStore> logger, CosmoManagementService cosmoManagementService) : IArticleAnalyseStore
{
    public async Task StoreArticleAnalyzeAsync(Analyse analyzeModel)
    {
        var database = await cosmoManagementService.CreateDatabaseAsync().ConfigureAwait(false);
        Container container = await cosmoManagementService.CreateAnalyzesContainerAsync(database).ConfigureAwait(false);

        try
        {
            var analyseResponse = await container.CreateItemAsync(analyzeModel, new PartitionKey(analyzeModel.PartitionKey)).ConfigureAwait(false);
        }
        catch (Exception ex)
        {
            logger.LogError(ex, "Error at cosmos insertion.");
        }
    }
}
