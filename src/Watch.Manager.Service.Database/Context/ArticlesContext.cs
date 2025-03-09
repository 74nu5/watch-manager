namespace Watch.Manager.Service.Database.Context;

using Microsoft.EntityFrameworkCore;

using Watch.Manager.Service.Database.Entities;

public class ArticlesContext : DbContext
{
    /// <inheritdoc />
    public ArticlesContext(DbContextOptions options)
            : base(options)
    {
    }

    public DbSet<Article> Articles => this.Set<Article>();

    /// <inheritdoc />
    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.HasPostgresExtension("vector");
        modelBuilder.Entity<Article>().Property(p => p.EmbeddingHead).HasColumnType("vector(1536)");
        modelBuilder.Entity<Article>().Property(p => p.EmbeddingBody).HasColumnType("vector(1536)");
    }
}
