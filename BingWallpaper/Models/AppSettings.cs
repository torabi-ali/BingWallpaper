using System;
using System.IO;

namespace BingWallpaper.Models
{
    public class AppSettings
    {
        public static AppSettings Current;

        public AppSettings()
        {
            Current = this;
        }

        public string ApplicationName { get; set; }

        public string BaseDirectory { get; set; }

        public DateTime LastDownloadDate { get; set; }

        public string ApplicationDirectory => Path.Combine(ApplicationName, BaseDirectory);
    }
}
