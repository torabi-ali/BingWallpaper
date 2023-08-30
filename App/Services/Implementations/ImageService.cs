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

        public async Task DownloadTodayImageAsync()
        {
            var imageInfo = await _bingDownloaderService.DownloadBingImageAsync(0);

            if (imageInfo != null)
            {
                _applicationDbContext.Images.Add(imageInfo);
                await _applicationDbContext.SaveChangesAsync();

                _logger.LogInformation("Today image successfully downloaded");
            }
        }
    }
}