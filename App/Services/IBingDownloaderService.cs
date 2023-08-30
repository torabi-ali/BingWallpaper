using Data.Models;

namespace App.Services
{
    public interface IBingDownloaderService
    {
        IAsyncEnumerable<ImageInfo> GetWallpapers(int days);

        Task<ImageInfo> DownloadBingImageAsync(int index);
    }
}