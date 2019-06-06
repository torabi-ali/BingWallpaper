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
        public static bool SetWallpaper(ImageInfo imageInfo)
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

        public static bool SetLockScreen(ImageInfo imageInfo)
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

        public static List<ImageInfo> GetWallpapers(int days = 1)
        {
            List<ImageInfo> result = new List<ImageInfo>();
            for (int i = 0; i < days; i++)
            {
                result.Add(DownloadBingImage(i));
            }

            return result;
        }

        private static ImageInfo DownloadBingImage(int index = 0)
        {
            var source = $"http://www.bing.com/HPImageArchive.aspx?format=xml&idx={index}&n=1&mkt=en-US";
            ImageInfo imageInfo = new ImageInfo();
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