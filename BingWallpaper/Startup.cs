using BingWallpaper.Core;
using BingWallpaper.Data;
using BingWallpaper.Properties;
using BingWallpaper.Utility;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Threading.Tasks;
using System.Windows;
using static System.Environment;

namespace BingWallpaper
{
    public partial class App : Application
    {
        private readonly ApplicationDbContext _applicationDbContext = new ApplicationDbContext();

        private void App_Startup(object sender, StartupEventArgs e)
        {
            if (Settings.Default.RunOnStartup)
            {
                WindowsUtility.EnableRunOnStartup();
            }
            else
            {
                WindowsUtility.DisableRunOnStartup();
            }

            #region Setting
            var path = Path.Combine(GetFolderPath(SpecialFolder.MyPictures), "BingWallpaper");
            if (string.IsNullOrEmpty(Settings.Default.FolderPath) || !Directory.Exists(path))
            {
                Directory.CreateDirectory(path);
                Settings.Default.FolderPath = $"{path}\\";
            }

            if (string.IsNullOrEmpty(Settings.Default.CompanyName))
            {
                Settings.Default.CompanyName = Assembly.GetExecutingAssembly().GetName().Name.Replace(' ', '_');
            }

            Settings.Default.Save();
            #endregion

            ApplicationDbContext.Seed();

            Task.Run(() => DownloadTodayImage());
        }

        private void DownloadTodayImage()
        {
            Settings.Default.WaitForNetwork = true;

            var imageInfo = BingWallpaperDownloader.GetWallpapers(1).FirstOrDefault();
            if (imageInfo != null)
            {
                _applicationDbContext.AddNewImage(imageInfo);
                _applicationDbContext.SaveChangesAsync();
            }

            Settings.Default.WaitForNetwork = false;
        }
    }
}
