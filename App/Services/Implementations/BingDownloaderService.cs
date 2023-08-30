using App.Dtos;
using Data.Models;
using System.Xml.Serialization;

namespace App.Services.Implementations
{
    public class BingDownloaderService : IBingDownloaderService
    {
        private readonly ApplicationSettings _settings;
        private readonly HttpClient _httpClient;

        public BingDownloaderService(ApplicationSettings settings, HttpClient httpClient)
        {
            _settings = settings;
            _httpClient = httpClient;
        }

        public async IAsyncEnumerable<ImageInfo> GetWallpapers(int days)
        {
            for (var i = 0; i < days; i++)
            {
                yield return await DownloadBingImageAsync(i);
            }
        }

        public async Task<ImageInfo> DownloadBingImageAsync(int index)
        {
            var fileName = $"{DateTime.Today:yyyy-MM-dd}.jpg";
            var filePath = Path.Combine(_settings.BasePath, fileName);
            if (File.Exists(filePath))
            {
                return null;
            }

            var xmlResponse = await _httpClient.GetAsync($"https://bing.com/HPImageArchive.aspx?format=xml&idx={index}&n=1&mkt=en-US");
            if (!xmlResponse.IsSuccessStatusCode)
            {
                throw new Exception();
            }

            using var xmlContentStream = await xmlResponse.Content.ReadAsStreamAsync();
            var serializer = new XmlSerializer(typeof(BingImagesDto));
            if (serializer.Deserialize(xmlContentStream) is not BingImagesDto bingImages)
            {
                throw new Exception();
            }

            var fileResponse = await _httpClient.GetAsync($"https://bing.com{bingImages.Image.Url}");
            if (!fileResponse.IsSuccessStatusCode)
            {
                throw new Exception();
            }

            var image = new ImageInfo
            {
                Name = fileName,
                Path = filePath,
                Headline = bingImages.Image.Headline,
                Url = bingImages.Image.Url,
                Copyright = bingImages.Image.Copyright,
                CreatedOn = DateTime.Now
            };

            using var fileContentStream = await fileResponse.Content.ReadAsStreamAsync();
            using var stream = new FileStream(image.Path, FileMode.Create, FileAccess.Write, FileShare.None);
            await fileContentStream.CopyToAsync(stream);

            return image;
        }
    }
}