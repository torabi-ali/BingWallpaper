using Data.DbContexts;
using Data.Models;
using Microsoft.EntityFrameworkCore;

namespace App.Services.Implementations
{
    public class ImageService : IImageService
    {
        private readonly IBingDownloaderService _bingDownloaderService;
        private readonly ApplicationDbContext _applicationDbContext;

        public ImageService(IBingDownloaderService bingDownloaderService, ApplicationDbContext applicationDbContext)
        {
            _bingDownloaderService = bingDownloaderService;
            _applicationDbContext = applicationDbContext;
        }

        public Task<List<ImageInfo>> GetImagesAsync(int pageIndex = 0, int pageSize = 10)
        {
            return _applicationDbContext.Images.Skip(pageIndex * pageSize).Take(pageSize).ToListAsync();
        }

        public async Task DownloadTodayImageAsync()
        {
            var imageInfo = await _bingDownloaderService.DownloadBingImageAsync(0);

            if (imageInfo != null)
            {
                _applicationDbContext.Images.Add(imageInfo);
                await _applicationDbContext.SaveChangesAsync();
            }
        }
    }
}