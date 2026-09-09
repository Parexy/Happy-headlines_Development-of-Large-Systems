using CommentService.Models;
using Microsoft.EntityFrameworkCore;

namespace CommentService.Data;

public class CommentDbContext : DbContext
{
    public CommentDbContext(DbContextOptions<CommentDbContext> options)
        : base(options)
    {
    }

    public DbSet<Comment> Comments => Set<Comment>();

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<Comment>(entity =>
        {
            entity.ToTable("Comments");

            entity.HasKey(a => a.Id);

            entity.Property(a => a.Id)
                .ValueGeneratedOnAdd();

            entity.Property(a => a.Content)
                .IsRequired()
                .HasMaxLength(500);
            
            entity.Property(a => a.Author)
                .IsRequired()
                .HasMaxLength(100);
        });
    }
}