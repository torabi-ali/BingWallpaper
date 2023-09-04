using Data.Models;
using Microsoft.EntityFrameworkCore;

namespace Data.DbContexts;

public class ApplicationDbContext : DbContext
{
    public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options) : base(options)
    { }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<ImageInfo>().Property(p => p.Name).IsRequired().HasMaxLength(16);
        modelBuilder.Entity<ImageInfo>().Property(p => p.Path).IsRequired().HasMaxLength(256);
        modelBuilder.Entity<ImageInfo>().Property(p => p.Headline).IsRequired().HasMaxLength(512);
        modelBuilder.Entity<ImageInfo>().Property(p => p.Url).IsRequired().HasMaxLength(1024);
        modelBuilder.Entity<ImageInfo>().Property(p => p.Copyright).IsRequired().HasMaxLength(2048);
        modelBuilder.Entity<ImageInfo>().HasIndex(p => new { p.Url });
    }

    public DbSet<ImageInfo> Images { get; set; }
}