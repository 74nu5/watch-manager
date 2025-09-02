namespace Watch.Manager.Service.Database.Context;

using Microsoft.EntityFrameworkCore;

using Watch.Manager.Service.Database.Entities;

/// <summary>
/// Contexte Entity Framework pour la gestion des articles et catégories.
/// </summary>
public class ArticlesContext : DbContext
{
    /// <inheritdoc />
    public ArticlesContext(DbContextOptions options)
            : base(options)
    {
    }

    /// <summary>
    /// DbSet pour les articles.
    /// </summary>
    public DbSet<Article> Articles => this.Set<Article>();

    /// <summary>
    /// DbSet pour les catégories.
    /// </summary>
    public DbSet<Category> Categories => this.Set<Category>();

    /// <summary>
    /// DbSet pour les relations Article-Category.
    /// </summary>
    public DbSet<ArticleCategory> ArticleCategories => this.Set<ArticleCategory>();

    /// <inheritdoc />
    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);

        // Configuration de la relation Category hiérarchique
        modelBuilder.Entity<Category>()
            .HasOne(c => c.Parent)
            .WithMany(c => c.Children)
            .HasForeignKey(c => c.ParentId)
            .OnDelete(DeleteBehavior.Restrict);

        // Configuration de la relation many-to-many Article-Category
        modelBuilder.Entity<ArticleCategory>()
            .HasKey(ac => new { ac.ArticleId, ac.CategoryId });

        modelBuilder.Entity<ArticleCategory>()
            .HasOne(ac => ac.Article)
            .WithMany(a => a.ArticleCategories)
            .HasForeignKey(ac => ac.ArticleId)
            .OnDelete(DeleteBehavior.Cascade);

        modelBuilder.Entity<ArticleCategory>()
            .HasOne(ac => ac.Category)
            .WithMany(c => c.ArticleCategories)
            .HasForeignKey(ac => ac.CategoryId)
            .OnDelete(DeleteBehavior.Cascade);

        // Index pour améliorer les performances
        modelBuilder.Entity<Category>()
            .HasIndex(c => c.Name)
            .IsUnique();

        modelBuilder.Entity<Category>()
            .HasIndex(c => c.ParentId);

        modelBuilder.Entity<ArticleCategory>()
            .HasIndex(ac => ac.ConfidenceScore);
    }
}
