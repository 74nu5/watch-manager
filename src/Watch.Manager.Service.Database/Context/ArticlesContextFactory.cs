namespace Watch.Manager.Service.Database.Context;

using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Design;

/// <summary>
/// Factory pour créer le contexte ArticlesContext lors du design-time (migrations).
/// </summary>
public class ArticlesContextFactory : IDesignTimeDbContextFactory<ArticlesContext>
{
    /// <summary>
    /// Crée un contexte ArticlesContext pour le design-time.
    /// </summary>
    /// <param name="args">Arguments de ligne de commande.</param>
    /// <returns>Instance du contexte.</returns>
    public ArticlesContext CreateDbContext(string[] args)
    {
        var optionsBuilder = new DbContextOptionsBuilder<ArticlesContext>();

        // Utiliser une connection string par défaut pour les migrations
        optionsBuilder.UseSqlServer("Server=(localdb)\\MSSQLLocalDB;Database=WatchManagerDb;Trusted_Connection=true;MultipleActiveResultSets=true");

        return new ArticlesContext(optionsBuilder.Options);
    }
}
