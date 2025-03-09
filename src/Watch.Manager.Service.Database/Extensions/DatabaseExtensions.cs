namespace Watch.Manager.Service.Database.Extensions;

using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;

using Watch.Manager.Service.Database.Abstractions;

public static class DatabaseExtensions
{
    public static void AddDatabaseServices(this IServiceCollection services)
    {
        // Add Azure Cosmos services
        //services.TryAddTransient<CosmosClient>(_ =>
        //{
        //    return new("AccountEndpoint=https://wm-cosmo-db.documents.azure.com:443/;AccountKey=Fmw3gj010DoTd4nNtpqSSR24N7qnFGx3VuiXpXxzAC5Glk39XTgdpEBt097nJwNAQwRI6X6SmRawACDbTROEMA==", new() { ApplicationName = "Watch.Manager" });
        //});
        services.TryAddTransient<CosmoManagementService>();
        services.TryAddTransient<IArticleAnalyseStore, ArticleAnalyseStore>();
    }
}
