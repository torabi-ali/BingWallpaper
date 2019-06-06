using BingWallpaper.Utility;
using System;

namespace BingWallpaper.Models
{
    public class ImageInfo : BaseEntity
    {
        private static readonly string FileExt = WindowsUtility.GetSuitableBackgroundFormat();

        public ImageInfo()
        {
            Name = $"{DateTime.Today.ToImageName()}.{FileExt}";
        }

        public string Name { set; get; }
        public string Url { set; get; }
        public string Path => System.IO.Path.Combine(Properties.Settings.Default.FolderPath, Name);
        public string Copyright { set; get; }
    }
}