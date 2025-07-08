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

}
