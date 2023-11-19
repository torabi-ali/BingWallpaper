using Data.Models;
using Data.Models.Configurations;
using Microsoft.EntityFrameworkCore;

namespace Data.DbContexts;

public class ApplicationDbContext(DbContextOptions<ApplicationDbContext> options) : DbContext(options)
{
    protected override void OnModelCreating(ModelBuilder builder)
    {
        builder.ApplyConfiguration(new ImageInfoConfiguration());
        builder.ApplyConfiguration(new SettingConfiguration());

        base.OnModelCreating(builder);
    }

    public DbSet<ImageInfo> Images { get; set; }

    public DbSet<Setting> Settings { get; set; }
}
