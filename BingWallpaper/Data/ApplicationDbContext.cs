using BingWallpaper.Models;
using Microsoft.EntityFrameworkCore;

namespace BingWallpaper.Data
{
    public class ApplicationDbContext : DbContext
    {
        public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options) : base(options)
        {
            Database.EnsureCreated();
        }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<ApplicationImage>(entity =>
            {
                entity.HasKey(p => p.Id);
                entity.HasIndex(p => p.Date);
            });

            base.OnModelCreating(modelBuilder);
        }

        #region DbSets
        public DbSet<ApplicationImage> Images { get; set; }
        #endregion
    }
}
