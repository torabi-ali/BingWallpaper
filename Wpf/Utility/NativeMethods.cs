using Microsoft.Win32;
using System.Reflection;
using System.Runtime.InteropServices;

namespace Wpf.Utility
{
    public static class NativeMethods
    {
        private const int SPI_SETDESKWALLPAPER = 20;
        private const int SPIF_UPDATEINIFILE = 0x01;
        private const int SPIF_SENDWININICHANGE = 0x02;

        public enum WallpaperStyle
        {
            Stretched = 2,
            Centered = 1,
            Tiled = 1,
            Fit = 6,
            Fill = 10
        }

        [DllImport("user32.dll")]
        public static extern int SendMessage(int hWnd, uint wMsg, uint wParam, uint lParam);

        public static void SetLockScreen(string wallpaper)
        {
            try
            {
                Registry.SetValue("HKEY_LOCAL_MACHINE\\SOFTWARE\\Policies\\Microsoft\\Windows\\Personalization", "LockScreenImage", wallpaper);
            }
            catch (UnauthorizedAccessException)
            {
                Console.WriteLine("Failed to update Policy Override. Ensure this user can write to 'HKEY_LOCAL_MACHINE\\SOFTWARE\\Policies\\Microsoft\\Windows\\Personalization'.");
            }
        }

        public static void SetWallpaper(string wallpaper, WallpaperStyle wallpaperStyle = WallpaperStyle.Fill, bool tileWallpaper = false)
        {
            using (var key = Registry.CurrentUser.OpenSubKey(@"Control Panel\Desktop", true))
            {
                key.SetValue(@"WallpaperStyle", ((int)wallpaperStyle).ToString());
                key.SetValue(@"TileWallpaper", tileWallpaper ? 1.ToString() : 0.ToString());
            }

            SystemParametersInfo(SPI_SETDESKWALLPAPER, 0, wallpaper, SPIF_UPDATEINIFILE | SPIF_SENDWININICHANGE);
        }

        public static void EnableRunOnStartup(string applicationName)
        {
            var rkApp = Registry.CurrentUser.OpenSubKey("SOFTWARE\\Microsoft\\Windows\\CurrentVersion\\Run", true);
            rkApp?.SetValue(applicationName, Assembly.GetExecutingAssembly().Location);
        }

        public static void DisableRunOnStartup(string applicationName)
        {
            var rkApp = Registry.CurrentUser.OpenSubKey("SOFTWARE\\Microsoft\\Windows\\CurrentVersion\\Run", true);
            rkApp?.DeleteValue(applicationName, false);
        }

        #region Private Methods

        [DllImport("user32.dll", CharSet = CharSet.Auto)]
        private static extern int SystemParametersInfo(int uAction, int uParam, string lpvParam, int fuWinIni);

        #endregion
    }
}