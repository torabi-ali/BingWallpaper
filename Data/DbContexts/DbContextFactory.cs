using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Design;

namespace Data.DbContexts;

public class DbContextFactory : IDesignTimeDbContextFactory<ApplicationDbContext>
{
    public ApplicationDbContext CreateDbContext(string[] args)
    {
        var optionsBuilder = new DbContextOptionsBuilder<ApplicationDbContext>();
        optionsBuilder.UseSqlite($"DataSource={Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "BingWallpaper.db")};");

        return new ApplicationDbContext(optionsBuilder.Options);
    }
}