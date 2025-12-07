using Microsoft.EntityFrameworkCore;
using UserMgmt.Domain.Entities;

namespace UserMgmt.Infrastructure.Persistence;

/// <summary>
/// Application database context
/// </summary>
public class ApplicationDbContext : DbContext
{
    public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options)
        : base(options)
    {
    }

    public DbSet<User> Users { get; set; }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);

        // User configuration
        modelBuilder.Entity<User>(entity =>
        {
            entity.HasKey(e => e.Id);
            entity.Property(e => e.Name).IsRequired().HasMaxLength(100);
            entity.Property(e => e.Email).IsRequired().HasMaxLength(200);
            entity.Property(e => e.PasswordHash).IsRequired().HasMaxLength(500);
            entity.Property(e => e.Role).IsRequired().HasConversion<int>();
            entity.Property(e => e.Department).HasMaxLength(100);
            entity.Property(e => e.RefreshToken).HasMaxLength(500);

            entity.HasIndex(e => e.Email).IsUnique();
            entity.HasIndex(e => e.RefreshToken);
        });
    }
}

