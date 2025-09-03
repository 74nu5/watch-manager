namespace Watch.Manager.Service.Database.Extensions;

using Microsoft.Extensions.AI;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;
using Microsoft.Extensions.Hosting;

using Watch.Manager.Service.Database.Abstractions;
using Watch.Manager.Service.Database.Context;
using Watch.Manager.Service.Database.Entities;

/// <summary>
///     Provides extension methods for registering database-related services in the application.
/// </summary>
public static class DatabaseExtensions
{
    /// <summary>
    ///     Registers database services, DbContext, and vector store components with the application's dependency injection container.
    /// </summary>
    /// <param name="builder">The <see cref="IHostApplicationBuilder" /> used to configure the application.</param>
    /// <exception cref="InvalidOperationException">
    ///     Thrown if the required connection string 'articlesdb' is not configured.
    /// </exception>
    public static void AddDatabaseServices(this IHostApplicationBuilder builder)
    {
        builder.Services.TryAddTransient<IArticleAnalyseStore, ArticleAnalyseStore>();
        builder.Services.TryAddTransient<ICategoryStore, CategoryStore>();
        builder.AddSqlServerDbContext<ArticlesContext>("articlesdb");
        _ = builder.Services.AddSqlServerVectorStore(
            provider => provider.GetRequiredService<IConfiguration>().GetConnectionString("articlesdb") ?? throw new InvalidOperationException("Connection string 'articlesdb' is not configured."),
            provider => new() { EmbeddingGenerator = provider.GetService<IEmbeddingGenerator>() });

        _ = builder.Services.AddSqlServerCollection<int, ArticleSearchEntity>("Articles", builder.Configuration.GetConnectionString("articlesdb") ?? throw new InvalidOperationException("Connection string 'articlesdb' is not configured."));
    }
}
