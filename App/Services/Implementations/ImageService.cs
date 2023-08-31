using Data.DbContexts;
using Data.Models;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;

namespace App.Services.Implementations
{
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
            var imageInfos = _bingDownloaderService.GetWallpapers(days);
            await foreach (var image in imageInfos)
            {
                if (image is not null)
                {
                    _applicationDbContext.Images.Add(image);
                }
            }

            await _applicationDbContext.SaveChangesAsync();
            _logger.LogInformation("Today image successfully downloaded");
        }
    }
}