namespace Watch.Manager.Service.Migrations;

using System.Diagnostics;

using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Infrastructure;
using Microsoft.EntityFrameworkCore.Storage;

using Watch.Manager.Service.Database.Context;

/// <summary>
///     Background service responsible for ensuring the database exists, running migrations, and seeding initial data.
/// </summary>
public sealed class Worker(IServiceProvider serviceProvider, IHostApplicationLifetime hostApplicationLifetime) : BackgroundService
{
    /// <summary>
    ///     The name of the activity source for diagnostics.
    /// </summary>
    internal const string ActivitySourceName = "Migrations";

    private static readonly ActivitySource ActivitySource = new(ActivitySourceName);

    /// <summary>
    ///     Executes the background service logic: ensures the database exists, applies migrations, and seeds data.
    /// </summary>
    /// <param name="cancellationToken">Token to signal cancellation.</param>
    /// <returns>A <see cref="Task" /> representing the asynchronous operation.</returns>
    protected override async Task ExecuteAsync(CancellationToken cancellationToken)
    {
        using var activity = ActivitySource.StartActivity("Migrating database", ActivityKind.Client, default(ActivityContext));

        try
        {
            using var scope = serviceProvider.CreateScope();
            var dbContext = scope.ServiceProvider.GetRequiredService<ArticlesContext>();

            //_ = await dbContext.Database.EnsureCreatedAsync(cancellationToken).ConfigureAwait(false);
            await EnsureDatabaseAsync(dbContext, cancellationToken).ConfigureAwait(false);
            await RunMigrationAsync(dbContext, cancellationToken).ConfigureAwait(false);

            // await SeedDataAsync(dbContext, cancellationToken).ConfigureAwait(false);
        }
        catch (Exception ex)
        {
            _ = activity?.AddException(ex);
            throw;
        }

        hostApplicationLifetime.StopApplication();
    }

    /// <summary>
    ///     Ensures the database exists by creating it if it does not.
    /// </summary>
    /// <param name="dbContext">The database context.</param>
    /// <param name="cancellationToken">Token to signal cancellation.</param>
    private static async Task EnsureDatabaseAsync(ArticlesContext dbContext, CancellationToken cancellationToken)
    {
        var dbCreator = dbContext.GetService<IRelationalDatabaseCreator>();

        var strategy = dbContext.Database.CreateExecutionStrategy();
        await strategy.ExecuteAsync(async () =>
        {
            // Create the database if it does not exist.
            // Do this first so there is then a database to start a transaction against.
            if (!await dbCreator.ExistsAsync(cancellationToken).ConfigureAwait(false))
                await dbCreator.CreateAsync(cancellationToken).ConfigureAwait(false);
        }).ConfigureAwait(false);
    }

    /// <summary>
    ///     Applies any pending migrations to the database.
    /// </summary>
    /// <param name="dbContext">The database context.</param>
    /// <param name="cancellationToken">Token to signal cancellation.</param>
    private static async Task RunMigrationAsync(ArticlesContext dbContext, CancellationToken cancellationToken)
    {
        var strategy = dbContext.Database.CreateExecutionStrategy();
        await strategy.ExecuteAsync(async () =>
                await dbContext.Database.MigrateAsync(cancellationToken).ConfigureAwait(false)
        ).ConfigureAwait(false);
    }
}
