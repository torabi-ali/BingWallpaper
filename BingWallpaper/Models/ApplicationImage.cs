using BingWallpaper.Utility;
using System;

namespace BingWallpaper.Models
{
    public class ApplicationImage : BaseEntity
    {
        public string Name => $"{Date.ToImageName()}.jpg";
        public DateTime Date { get; set; }
        public string Url { set; get; }
        public string Path { set; get; }
        public string Copyright { set; get; }
    }
}