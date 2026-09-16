using DraftService.Models;
using Microsoft.EntityFrameworkCore;

namespace DraftService.Data;

public class DraftDbContext : DbContext
{
    public DraftDbContext(DbContextOptions<DraftDbContext> options)
        : base(options)
    {
    }

    public DbSet<Draft> Drafts => Set<Draft>();

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<Draft>(entity =>
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

            entity.Property(a => a.UpdatedAt)
                .IsRequired();
        });
    }
}