using Microsoft.Win32;
using System;
using System.IO;
using System.Reflection;

namespace BingWallpaper.Utility
{
    public static class WindowsUtility
    {
        private static readonly string ProgramName = Assembly.GetExecutingAssembly().GetName().Name;

        public static WindowsVersion GetWindowsVersion()
        {
            var os = Environment.OSVersion;
            var vs = os.Version;
            WindowsVersion operatingSystem = WindowsVersion.Windows10;
            if (os.Platform == PlatformID.Win32Windows)
            {
                switch (vs.Minor)
                {
                    case 0:
                        operatingSystem = WindowsVersion.Windows95;
                        break;

                    case 10:
                        if (vs.Revision.ToString() == "2222A")
                        {
                            operatingSystem = WindowsVersion.Windows98SE;
                        }
                        else
                        {
                            operatingSystem = WindowsVersion.Windows98;
                        }
                        break;

                    case 90:
                        operatingSystem = WindowsVersion.WindowsME;
                        break;

                    default:
                        break;
                }
            }
            else if (os.Platform == PlatformID.Win32NT)
            {
                switch (vs.Major)
                {
                    case 3:
                        operatingSystem = WindowsVersion.WindowsNT3_51;
                        break;

                    case 4:
                        operatingSystem = WindowsVersion.WindowsNT4_0;
                        break;

                    case 5:
                        if (vs.Minor == 0)
                        {
                            operatingSystem = WindowsVersion.Windows2000;
                        }
                        else
                        {
                            operatingSystem = WindowsVersion.WindowsXP;
                        }
                        break;

                    case 6:
                        if (vs.Minor == 0)
                        {
                            operatingSystem = WindowsVersion.WindowsVista;
                        }
                        else if (vs.Minor == 1)
                        {
                            operatingSystem = WindowsVersion.Windows7;
                        }
                        else if (vs.Minor == 2)
                        {
                            operatingSystem = WindowsVersion.Windows8;
                        }
                        else
                        {
                            operatingSystem = WindowsVersion.Windows8_1;
                        }
                        break;

                    case 10:
                        operatingSystem = WindowsVersion.Windows10;
                        break;

                    default:
                        break;
                }
            }

            return operatingSystem;
        }

        public static string GetSuitableBackgroundFormat()
        {
            var os = GetWindowsVersion();

            if (os == WindowsVersion.Windows8 || os == WindowsVersion.Windows8_1 || os == WindowsVersion.Windows10)
            {
                return "jpg";
            }
            else
            {
                return "bmp";
            }
        }

        public static void CopyAll(string SourcePath, string DestinationPath)
        {
            try
            {
                if (!Directory.Exists(DestinationPath))
                {
                    Directory.CreateDirectory(DestinationPath);
                }
            }
            catch (Exception)
            {
                return;
            }


            foreach (string dirPath in Directory.GetDirectories(SourcePath, "*", SearchOption.AllDirectories))
            {
                Directory.CreateDirectory(dirPath.Replace(SourcePath, DestinationPath));
            }

            foreach (string newPath in Directory.GetFiles(SourcePath, "*.*", SearchOption.AllDirectories))
            {
                File.Copy(newPath, newPath.Replace(SourcePath, DestinationPath), true);
            }
        }

        #region Startup
        public static void EnableRunOnStartup()
        {
            var rkApp = Registry.CurrentUser.OpenSubKey("SOFTWARE\\Microsoft\\Windows\\CurrentVersion\\Run", true);
            rkApp?.SetValue(ProgramName, Assembly.GetExecutingAssembly().Location);
        }

        public static void DisableRunOnStartup()
        {
            var rkApp = Registry.CurrentUser.OpenSubKey("SOFTWARE\\Microsoft\\Windows\\CurrentVersion\\Run", true);
            rkApp?.DeleteValue(ProgramName, false);
        }
        #endregion

        public enum WindowsVersion
        {
            Windows95,
            Windows98,
            Windows98SE,
            WindowsME,
            WindowsNT3_51,
            WindowsNT4_0,
            Windows2000,
            WindowsXP,
            WindowsVista,
            Windows7,
            Windows8,
            Windows8_1,
            Windows10,
        }
    }
}