namespace Watch.Manager.Service.Database.Context;

using Microsoft.EntityFrameworkCore;

using Watch.Manager.Service.Database.Entities;

/// <summary>
///     Represents the Entity Framework Core database context for managing articles, categories, and saved searches.
/// </summary>
/// <remarks>This context exposes <see cref="DbSet{TEntity}" /> properties for each entity in the model. </remarks>
/// <remarks>Initializes a new instance of the <see cref="ArticlesContext" /> class using the specified options.</remarks>
/// <param name="options">The options to be used by a <see cref="DbContext" />.</param>
public class ArticlesContext(DbContextOptions<ArticlesContext> options) : DbContext(options)
{
    /// <summary>
    ///     Gets the <see cref="DbSet{TEntity}" /> of <see cref="Article" /> entities.
    /// </summary>
    public DbSet<Article> Articles => this.Set<Article>();

    /// <summary>
    ///     Gets the <see cref="DbSet{TEntity}" /> of <see cref="Category" /> entities.
    /// </summary>
    public DbSet<Category> Categories => this.Set<Category>();

    /// <summary>
    ///     Gets the <see cref="DbSet{TEntity}" /> of <see cref="ArticleCategory" /> entities.
    /// </summary>
    public DbSet<ArticleCategory> ArticleCategories => this.Set<ArticleCategory>();

    /// <summary>
    ///     Gets the <see cref="DbSet{TEntity}" /> of <see cref="SavedSearch" /> entities.
    /// </summary>
    public DbSet<SavedSearch> SavedSearches => this.Set<SavedSearch>();

    /// <summary>
    ///     Configures the schema needed for the context.
    /// </summary>
    /// <param name="modelBuilder">The builder being used to construct the model for this context.</param>
    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);

        // Configuration de la relation Category hiérarchique
        _ = modelBuilder.Entity<Category>()
                        .HasOne(c => c.Parent)
                        .WithMany(c => c.Children)
                        .HasForeignKey(c => c.ParentId)
                        .OnDelete(DeleteBehavior.Restrict);

        // Configuration de la relation many-to-many Article-Category
        _ = modelBuilder.Entity<ArticleCategory>()
                        .HasKey(ac => new { ac.ArticleId, ac.CategoryId });

        _ = modelBuilder.Entity<ArticleCategory>()
                        .HasOne(ac => ac.Article)
                        .WithMany(a => a.ArticleCategories)
                        .HasForeignKey(ac => ac.ArticleId)
                        .OnDelete(DeleteBehavior.Cascade);

        _ = modelBuilder.Entity<ArticleCategory>()
                        .HasOne(ac => ac.Category)
                        .WithMany(c => c.ArticleCategories)
                        .HasForeignKey(ac => ac.CategoryId)
                        .OnDelete(DeleteBehavior.Cascade);

        // Index pour améliorer les performances
        _ = modelBuilder.Entity<Category>()
                        .HasIndex(c => c.Name)
                        .IsUnique();

        _ = modelBuilder.Entity<Category>()
                        .HasIndex(c => c.ParentId);

        _ = modelBuilder.Entity<ArticleCategory>()
                        .HasIndex(ac => ac.ConfidenceScore);
    }
}
