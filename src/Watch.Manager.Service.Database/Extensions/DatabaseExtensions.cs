namespace Watch.Manager.Service.Database.Extensions;

using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection.Extensions;
using Microsoft.Extensions.Hosting;

using Watch.Manager.Service.Database.Abstractions;
using Watch.Manager.Service.Database.Context;

public static class DatabaseExtensions
{
    public static void AddDatabaseServices(this IHostApplicationBuilder builder)
    {
        builder.Services.TryAddTransient<IArticleAnalyseStore, ArticleAnalyseStore>();
        builder.AddNpgsqlDbContext<ArticlesContext>("articles-db", configureDbContextOptions: dbContextOptionsBuilder => dbContextOptionsBuilder.UseNpgsql(contextOptionsBuilder => contextOptionsBuilder.UseVector()));
    }
}
