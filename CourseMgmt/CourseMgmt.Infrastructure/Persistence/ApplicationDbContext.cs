using CourseMgmt.Domain.Entities;
using Microsoft.EntityFrameworkCore;

namespace CourseMgmt.Infrastructure.Persistence;

/// <summary>
/// Application database context
/// </summary>
public class ApplicationDbContext : DbContext
{
    public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options)
        : base(options)
    {
    }

    public DbSet<Course> Courses { get; set; }
    public DbSet<User> Users { get; set; }
    public DbSet<Category> Categories { get; set; }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);

        // Course configuration
        modelBuilder.Entity<Course>(entity =>
        {
            entity.HasKey(e => e.Id);
            entity.Property(e => e.Title).IsRequired().HasMaxLength(200);
            entity.Property(e => e.Description).IsRequired();
            entity.Property(e => e.Duration).IsRequired().HasMaxLength(50);
            entity.Property(e => e.Level).IsRequired().HasConversion<int>();
            entity.Property(e => e.Status).IsRequired().HasConversion<int>();
            entity.Property(e => e.Thumbnail).HasMaxLength(500);

            entity.HasIndex(e => e.EducatorId);
            entity.HasIndex(e => e.CategoryId);
            entity.HasIndex(e => e.Status);
            entity.HasIndex(e => new { e.Title, e.Description });

            entity.HasOne(e => e.Educator)
                .WithMany()
                .HasForeignKey(e => e.EducatorId)
                .OnDelete(DeleteBehavior.Restrict);

            entity.HasOne(e => e.Category)
                .WithMany()
                .HasForeignKey(e => e.CategoryId)
                .OnDelete(DeleteBehavior.Restrict);
        });

        // User configuration
        modelBuilder.Entity<User>(entity =>
        {
            entity.HasKey(e => e.Id);
            entity.Property(e => e.Name).IsRequired().HasMaxLength(200);
            entity.Property(e => e.Email).IsRequired().HasMaxLength(200);
            entity.Property(e => e.Role).IsRequired().HasMaxLength(50);
            entity.Property(e => e.Department).HasMaxLength(100);

            entity.HasIndex(e => e.Email).IsUnique();
        });

        // Category configuration
        modelBuilder.Entity<Category>(entity =>
        {
            entity.HasKey(e => e.Id);
            entity.Property(e => e.Name).IsRequired().HasMaxLength(100);
            entity.Property(e => e.Color).HasMaxLength(20).HasDefaultValue("#3b82f6");
        });
    }
}

