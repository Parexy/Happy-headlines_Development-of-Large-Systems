using ArticleService.Models;
using Microsoft.EntityFrameworkCore;

namespace ArticleService.Data;

public class ArticleDbContext : DbContext
{
    public ArticleDbContext(DbContextOptions<ArticleDbContext> options)
        : base(options)
    {
    }

    public DbSet<Article> Articles => Set<Article>();

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<Article>(entity =>
        {
            entity.HasKey(a => a.Id);

            entity.Property(a => a.Id)
                .ValueGeneratedOnAdd();

            entity.Property(a => a.Title)
                .IsRequired()
                .HasMaxLength(500);

            entity.Property(a => a.Content)
                .IsRequired();

            entity.Property(a => a.Author)
                .IsRequired()
                .HasMaxLength(200);

            entity.Property(a => a.CreatedAt)
                .IsRequired();

            entity.Property(a => a.PublishedAt)
                .IsRequired();

            entity.Property(a => a.UpdatedAt)
                .IsRequired();
        });
    }
}