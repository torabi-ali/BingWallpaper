using BingWallpaper.Helpers;
using BingWallpaper.Models;
using BingWallpaper.Utility;
using System;
using System.Net;
using System.Text;
using System.Threading.Tasks;

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

        public static ApplicationImage DownloadBingImage(int index = 0)
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

        public static async Task<ApplicationImage> DownloadBingImageAsync(int index = 0)
        {
            var source = $"http://www.bing.com/HPImageArchive.aspx?format=xml&idx={index}&n=1&mkt=en-US";
            ApplicationImage imageInfo = new ApplicationImage();
            string xmlData;

            using (var xmlClient = new WebClient { Encoding = Encoding.UTF8 })
            {
                xmlData = await xmlClient.DownloadStringTaskAsync(source);
            }

            try
            {
                var bingImage = xmlData.DeserializeXml<images>();
                imageInfo.Url = $"https://bing.com{bingImage.image.url}";
                imageInfo.Copyright = $"Copytight: {bingImage.image.copyright}";
                imageInfo.Date = DateTime.Today.AddDays(-1 * index);

                using (var client = new WebClient())
                {
                    await client.DownloadFileTaskAsync(imageInfo.Url, imageInfo.Path);
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