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

    public DbSet<ProfanityWordView> ProfanityWordsView
        => Set<ProfanityWordView>();

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<Profanity>(entity =>
        {
            entity.ToTable("Profanity");

            entity.HasKey(a => a.Id);

            entity.Property(a => a.Id)
                .ValueGeneratedOnAdd();

            entity.Property(a => a.Word)
                .IsRequired()
                .HasMaxLength(50);
        });

        // Profanity materialized view
        modelBuilder.Entity<ProfanityWordView>(entity =>
        {
            entity.HasNoKey();
            entity.ToView("profanity_words");
        });

    }
}