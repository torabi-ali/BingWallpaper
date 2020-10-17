using BingWallpaper.Models;
using Microsoft.EntityFrameworkCore;
using System.IO;
using System.Reflection;

namespace BingWallpaper.Data
{
    public class ApplicationDbContext : DbContext
    {
        private static readonly string DbName = Assembly.GetExecutingAssembly().GetName().Name;
        private static readonly string DbPath = "D:/";
        private static readonly string DbFullPath = Path.Combine(DbPath, $"{DbName}.db");

        public DbSet<ImageInfo> ImageInfos { get; set; }

        protected override void OnConfiguring(DbContextOptionsBuilder options)
        {
            options.UseSqlite($"Data Source={DbFullPath}");
        }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<ImageInfo>(entity =>
            {
                entity.HasKey(p => p.Id);
                entity.HasIndex(p => p.Date);
            });

            base.OnModelCreating(modelBuilder);
        }
    }
}
