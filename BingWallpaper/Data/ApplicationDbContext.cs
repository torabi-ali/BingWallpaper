using BingWallpaper.Helpers;
using BingWallpaper.Models;
using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Data.Entity.ModelConfiguration.Conventions;
using System.Data.SQLite;
using System.IO;
using System.Linq;
using System.Reflection;

namespace BingWallpaper.Data
{
    [DbConfigurationType(typeof(SQLiteConfiguration))]
    public class ApplicationDbContext : DbContext
    {
        private static readonly string DbName = Assembly.GetExecutingAssembly().GetName().Name;
        private static readonly string DbPath = Properties.Settings.Default.FolderPath;
        private static readonly string DbFullPath = Path.Combine(DbPath, $"{DbName}.sqlite");

        public ApplicationDbContext()
            : base(new SQLiteConnection()
            {
                ConnectionString = new SQLiteConnectionStringBuilder() { DataSource = DbFullPath, ForeignKeys = true }.ConnectionString
            }, true)
        { }

        #region DbSets
        public DbSet<ImageInfo> Images { get; set; }
        #endregion

        protected override void OnModelCreating(DbModelBuilder modelBuilder)
        {
            modelBuilder.Conventions.Remove<PluralizingTableNameConvention>();
            base.OnModelCreating(modelBuilder);
        }

        public ImageInfo AddNewImage(ImageInfo imageInfo)
        {
            var exists = Images.AsNoTracking().Where(p => p.Url == imageInfo.Url).FirstOrDefault();
            if (exists == null)
            {
                return Images.Add(imageInfo);
            }
            else
            {
                File.Delete(imageInfo.Path);
            }
            return exists;
        }

        public List<ImageInfo> GetImagesRange(int range = 5)
        {
            var images = new List<ImageInfo>();
            return Images.AsNoTracking().OrderByDescending(p => p.Id).Take(range).ToList();
        }

        public ImageInfo GetTodayImage()
        {
            var image = new ImageInfo();
            return Images.AsNoTracking().Where(p => p.Name == image.Name).FirstOrDefault();
        }

        public static void Seed()
        {
            var sql = "CREATE TABLE IF NOT EXISTS 'ImageInfo' ('Id' INTEGER PRIMARY KEY AUTOINCREMENT, 'Name' TEXT, 'Url' TEXT, 'Copyright' TEXT);";

            var dbConnection = new SQLiteConnection($"Data Source={DbFullPath};Version=3;");
            dbConnection.Open();
            var command = new SQLiteCommand(sql, dbConnection);

            try
            {
                command.ExecuteNonQuery();
            }
            catch (Exception ex)
            {
                ex.Log();
            }
        }
    }
}
