using App.Dtos;
using Data.DbContexts;
using Data.Models;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using System.Xml.Serialization;

namespace App.Services.Implementations;

public class BingDownloaderService : IBingDownloaderService
{
    private readonly ApplicationDbContext _applicationDbContext;
    private readonly ApplicationSettings _settings;
    private readonly IHttpClientFactory _httpClientFactory;
    private readonly ILogger<BingDownloaderService> _logger;

    public BingDownloaderService(ApplicationDbContext applicationDbContext, ApplicationSettings settings, IHttpClientFactory httpClientFactory, ILogger<BingDownloaderService> logger)
    {
        _applicationDbContext = applicationDbContext;
        _settings = settings;
        _httpClientFactory = httpClientFactory;
        _logger = logger;
    }

    public Task<List<ImageInfo>> GetImagesPagedAsync(int pageIndex = 0, int pageSize = 10)
    {
        return _applicationDbContext.Images.AsNoTracking().OrderByDescending(p => p.CreatedOn).Skip(pageIndex * pageSize).Take(pageSize).ToListAsync();
    }

    public async Task DownloadWallpapers(int days)
    {
        for (var i = days - 1; i >= 0; i--)
        {
            await GetBingImageAsync(i);
        }
    }

    private async Task<ImageInfo> GetBingImageAsync(int index)
    {
        var apiAddress = $"/HPImageArchive.aspx?format=xml&idx={index}&n=1&mkt=en-US";
        var httpClient = _httpClientFactory.CreateClient("Default");
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
        var filePath = Path.Combine(_settings.BasePath, fileName);
        var image = new ImageInfo
        {
            Name = fileName,
            Path = filePath,
            Headline = bingImages.Image.Headline,
            Url = bingImages.Image.Url,
            Copyright = bingImages.Image.Copyright,
            CreatedOn = createdOn
        };

        var exists = await _applicationDbContext.Images.AnyAsync(p => p.Url.Equals(image.Url));
        if (!exists)
        {
            _applicationDbContext.Images.Add(image);
            await _applicationDbContext.SaveChangesAsync();

            _logger.LogInformation("Image added with url: {imageUrl}", image.Url);
        }
        else
        {
            _logger.LogInformation("Image already exists with url: {imageUrl}", image.Url);
            return null;
        }

        await DownloadBingImageAsync(bingImages.Image.Url, filePath);

        return image;
    }

    private async Task DownloadBingImageAsync(string imageUrl, string imagePath)
    {
        var httpClient = _httpClientFactory.CreateClient("Default");
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