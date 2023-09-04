using Data.DbContexts;
using Data.Models;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;

namespace App.Services.Implementations;

public class ImageService : IImageService
{
    private readonly IBingDownloaderService _bingDownloaderService;
    private readonly ApplicationDbContext _applicationDbContext;
    private readonly ILogger<ImageService> _logger;

    public ImageService(IBingDownloaderService bingDownloaderService, ApplicationDbContext applicationDbContext, ILogger<ImageService> logger)
    {
        _bingDownloaderService = bingDownloaderService;
        _applicationDbContext = applicationDbContext;
        _logger = logger;
    }

    public Task<List<ImageInfo>> GetImagesAsync(int pageIndex = 0, int pageSize = 10)
    {
        return _applicationDbContext.Images.OrderByDescending(p => p.CreatedOn).Skip(pageIndex * pageSize).Take(pageSize).ToListAsync();
    }

    public async Task DownloadImagesAsync(int days)
    {
        var imageInfos = await _bingDownloaderService.GetWallpapers(days);
        foreach (var image in imageInfos)
        {
            var exists = await _applicationDbContext.Images.AnyAsync(p => p.Url.Equals(image.Url));
            if (!exists)
            {
                _applicationDbContext.Images.Add(image);
                _logger.LogInformation($"Image added with url: {image.Url}");
            }
            else
            {
                _logger.LogInformation($"Image already exists with url: {image.Url}");
            }
        }

        if (imageInfos.Count > 0)
        {
            await _applicationDbContext.SaveChangesAsync();
        }
    }
}