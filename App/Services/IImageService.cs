using Data.Models;

namespace App.Services
{
    public interface IImageService
    {
        Task<List<ImageInfo>> GetImagesAsync(int pageIndex = 0, int pageSize = 10);

        Task DownloadImagesAsync(int days);
    }
}