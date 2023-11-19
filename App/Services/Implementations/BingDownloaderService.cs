using System.Xml.Serialization;
using App.Dtos;
using Data.DbContexts;
using Data.Models;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;

namespace App.Services.Implementations;

public class BingDownloaderService(ApplicationDbContext applicationDbContext, ApplicationSettings settings, IHttpClientFactory httpClientFactory, ILogger<BingDownloaderService> logger) : IBingDownloaderService
{
    public Task<List<ImageInfo>> GetImagesPagedAsync(int pageIndex = 0, int pageSize = 10)
    {
        return applicationDbContext.Images.AsNoTracking().OrderByDescending(p => p.CreatedOn).Skip(pageIndex * pageSize).Take(pageSize).ToListAsync();
    }

    public async Task<ImageInfo> GetBingImageAsync(int index)
    {
        var apiAddress = $"/HPImageArchive.aspx?format=xml&idx={index}&n=1&mkt=en-US";
        var httpClient = httpClientFactory.CreateClient("Default");
        var xmlResponse = await httpClient.GetAsync(apiAddress);
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

        var createdOn = DateTime.Today.AddDays(-1 * index);
        var fileName = $"{createdOn:yyyy-MM-dd}.jpg";
        var filePath = Path.Combine(settings.BasePath, fileName);
        var image = new ImageInfo
        {
            Name = fileName,
            Path = filePath,
            Headline = bingImages.Image.Headline,
            Url = bingImages.Image.Url,
            Copyright = bingImages.Image.Copyright,
            CreatedOn = createdOn
        };

        var exists = await applicationDbContext.Images.AnyAsync(p => p.Url.Equals(image.Url));
        if (!exists)
        {
            applicationDbContext.Images.Add(image);
            await applicationDbContext.SaveChangesAsync();

            logger.LogInformation("Image added with url: {imageUrl}", image.Url);
        }
        else
        {
            logger.LogInformation("Image already exists with url: {imageUrl}", image.Url);
            return null;
        }

        await DownloadBingImageAsync(bingImages.Image.Url, filePath);

        return image;
    }

    private async Task DownloadBingImageAsync(string imageUrl, string imagePath)
    {
        var httpClient = httpClientFactory.CreateClient("Default");
        var fileResponse = await httpClient.GetAsync(imageUrl);
        if (!fileResponse.IsSuccessStatusCode)
        {
            throw new Exception($"Error while downloading the image from {imageUrl}");
        }

        using var fileContentStream = await fileResponse.Content.ReadAsStreamAsync();
        using var stream = new FileStream(imagePath, FileMode.Create, FileAccess.Write, FileShare.None);
        await fileContentStream.CopyToAsync(stream);
    }
}
