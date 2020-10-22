using BingWallpaper.Helpers;
using BingWallpaper.Models;
using BingWallpaper.Utility;
using System;
using System.Collections.Generic;
using System.Net;
using System.Text;

namespace BingWallpaper.Core
{
    public static class BingWallpaperDownloader
    {
        public static bool SetWallpaper(ApplicationImage imageInfo)
        {
            try
            {
                NativeMethods.SetWallpaper(imageInfo.Path);
                return true;
            }
            catch (Exception ex)
            {
                ex.Log();
                return false;
            }
        }

        public static bool SetLockScreen(ApplicationImage imageInfo)
        {
            try
            {
                NativeMethods.SetLockScreen(imageInfo.Path);
                return true;
            }
            catch (Exception ex)
            {
                ex.Log();
                return false;
            }
        }

        public static List<ApplicationImage> GetWallpapers(DateTime lastDownloadDate)
        {
            var days = (DateTime.Today - lastDownloadDate).Days;
            List<ApplicationImage> result = new List<ApplicationImage>();
            for (int i = 0; i < days; i++)
            {
                var image = DownloadBingImage(i);
                if (image != null)
                {
                    result.Add(image);
                }
            }

            return result;
        }

        private static ApplicationImage DownloadBingImage(int index = 0)
        {
            var source = $"http://www.bing.com/HPImageArchive.aspx?format=xml&idx={index}&n=1&mkt=en-US";
            ApplicationImage imageInfo = new ApplicationImage();
            string xmlData;

            using (var xmlClient = new WebClient { Encoding = Encoding.UTF8 })
            {
                xmlData = xmlClient.DownloadString(source);
            }

            try
            {
                var bingImage = xmlData.DeserializeXml<images>();
                imageInfo.Url = $"https://bing.com{bingImage.image.url}";
                imageInfo.Copyright = $"Copytight: {bingImage.image.copyright}";
                imageInfo.Date = DateTime.Today.AddDays(-1 * index);

                using (var client = new WebClient())
                {
                    client.DownloadFile(imageInfo.Url, imageInfo.Path);
                }

                return imageInfo;
            }
            catch (Exception ex)
            {
                ex.Log();
                return null;
            }
        }
    }
}