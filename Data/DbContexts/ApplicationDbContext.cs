using Data.Models;
using Data.Models.Configurations;
using Microsoft.EntityFrameworkCore;

namespace Data.DbContexts;

public class ApplicationDbContext : DbContext
{
    public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options) : base(options)
    { }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.ApplyConfiguration(new ImageInfoConfiguration());
        modelBuilder.ApplyConfiguration(new SettingConfiguration());

        base.OnModelCreating(modelBuilder);
    }

    public DbSet<ImageInfo> Images { get; set; }

    public DbSet<Setting> Settings { get; set; }
}