using App.Dtos;
using Data.Models;
using System.Xml.Serialization;

namespace App.Services.Implementations;

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
        var createdOn = DateTime.Today.AddDays(-1 * index);
        var fileName = $"{createdOn:yyyy-MM-dd}.jpg";
        var filePath = Path.Combine(_settings.BasePath, fileName);
        if (File.Exists(filePath))
        {
            return null;
        }

        var apiAddress = $"https://bing.com/HPImageArchive.aspx?format=xml&idx={index}&n=1&mkt=en-US";
        var xmlResponse = await _httpClient.GetAsync(apiAddress);
        if (!xmlResponse.IsSuccessStatusCode)
        {
            throw new Exception($"Error while downloading the xml file from {apiAddress}");
        }

        using var xmlContentStream = await xmlResponse.Content.ReadAsStreamAsync();
        var serializer = new XmlSerializer(typeof(BingImagesDto));
        if (serializer.Deserialize(xmlContentStream) is not BingImagesDto bingImages)
        {
            throw new Exception($"Error while deserializing {apiAddress}");
        }

        var imageAddress = $"https://bing.com{bingImages.Image.Url}";
        var fileResponse = await _httpClient.GetAsync(imageAddress);
        if (!fileResponse.IsSuccessStatusCode)
        {
            throw new Exception($"Error while downloading the image from {imageAddress}");
        }

        var image = new ImageInfo
        {
            Name = fileName,
            Path = filePath,
            Headline = bingImages.Image.Headline,
            Url = bingImages.Image.Url,
            Copyright = bingImages.Image.Copyright,
            CreatedOn = createdOn
        };

        using var fileContentStream = await fileResponse.Content.ReadAsStreamAsync();
        using var stream = new FileStream(image.Path, FileMode.Create, FileAccess.Write, FileShare.None);
        await fileContentStream.CopyToAsync(stream);

        return image;
    }
}