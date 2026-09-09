using ProfanityService.Models;
using Microsoft.EntityFrameworkCore;

namespace ProfanityService.Data;

public class ProfanityDbContext : DbContext
{
    public ProfanityDbContext(DbContextOptions<ProfanityDbContext> options)
        : base(options)
    {
    }

    public DbSet<Profanity> Profanity => Set<Profanity>();

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<Profanity>(entity =>
        {
            entity.ToTable("ProfanityWords");

            entity.HasKey(a => a.Id);

            entity.Property(a => a.Id)
                .ValueGeneratedOnAdd();

            entity.Property(a => a.Word)
                .IsRequired()
                .HasMaxLength(50);
        });
    }
}