using Microsoft.EntityFrameworkCore;
using SubscriberService.Models;

namespace SubscriberService.Data;

public class SubscriberDbContext : DbContext
{
    public SubscriberDbContext(
        DbContextOptions<SubscriberDbContext> options)
        : base(options)
    {
    }

    public DbSet<Subscriber> Subscribers => Set<Subscriber>();

    public DbSet<ReleaseToggle> ReleaseToggles
        => Set<ReleaseToggle>();

    public DbSet<Subscriber> Subscribers
        => Set<Subscriber>();

    protected override void OnModelCreating(
        ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<Subscriber>(entity =>
        {
            entity.HasKey(s => s.Id);

            entity.Property(s => s.Id)
                .ValueGeneratedOnAdd();

            entity.Property(s => s.Email)
                .IsRequired()
                .HasMaxLength(320);

            entity.HasIndex(s => s.Email)
                .IsUnique();

            entity.Property(s => s.CreatedAt)
                .IsRequired();
        });

        modelBuilder.Entity<ReleaseToggle>(entity =>
        {
            entity.ToTable("feature_flags");

            entity.HasKey(x => x.Name);

            entity.Property(x => x.Name)
                .HasColumnName("name")
                .HasMaxLength(100);

            entity.Property(x => x.Enabled)
                .HasColumnName("enabled")
                .IsRequired();
        });

        modelBuilder.Entity<Subscriber>(entity =>
        {
            entity.ToTable("subscribers");

            entity.HasKey(x => x.Id);

            entity.Property(x => x.Id)
                .HasColumnName("id");

            entity.Property(x => x.Email)
                .HasColumnName("email")
                .HasMaxLength(320)
                .IsRequired();

            entity.HasIndex(x => x.Email).IsUnique();

            entity.Property(x => x.CreatedAt)
                .HasColumnName("created_at")
                .IsRequired();
        });
    }
}
