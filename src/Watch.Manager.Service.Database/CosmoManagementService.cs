namespace Watch.Manager.Service.Database;

using Microsoft.Azure.Cosmos;

internal class CosmoManagementService(CosmosClient cosmosClient)
{
    public const string DatabaseName = "WatchManager";
    public const string AnalysesContainerName = "wm-analyzes";

    public async Task<Database> CreateDatabaseAsync()
            // Create a new database
        => await cosmosClient.CreateDatabaseIfNotExistsAsync(DatabaseName).ConfigureAwait(false);

    public async Task<ContainerResponse> CreateAnalyzesContainerAsync(Database database)
            // Create a new container
        => await database.CreateContainerIfNotExistsAsync(AnalysesContainerName, "/tags").ConfigureAwait(false);
}
