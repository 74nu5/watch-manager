namespace Watch.Manager.Service.Database.Extensions;

using Microsoft.Extensions.AI;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;
using Microsoft.Extensions.Hosting;

using Watch.Manager.Service.Database.Abstractions;
using Watch.Manager.Service.Database.Context;
using Watch.Manager.Service.Database.Entities;

public static class DatabaseExtensions
{
    public static void AddDatabaseServices(this IHostApplicationBuilder builder)
    {
        builder.Services.TryAddTransient<IArticleAnalyseStore, ArticleAnalyseStore>();
        builder.AddSqlServerDbContext<ArticlesContext>("articles-db");
        _ = builder.Services.AddSqlServerVectorStore(
            connectionStringProvider: provider => provider.GetRequiredService<IConfiguration>().GetConnectionString("articles-db") ?? throw new InvalidOperationException("Connection string 'articles-db' is not configured."),
            optionsProvider: provider => new() { EmbeddingGenerator = provider.GetService<IEmbeddingGenerator>() });

        _ = builder.Services.AddSqlServerCollection<int, ArticleSearchEntity>("Articles", builder.Configuration.GetConnectionString("articles-db") ?? throw new InvalidOperationException("Connection string 'articles-db' is not configured."));
    }
}
