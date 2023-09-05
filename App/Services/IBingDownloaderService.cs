using Data.Models;

namespace App.Services;

public interface IBingDownloaderService
{
    Task<List<ImageInfo>> GetImagesPagedAsync(int pageIndex = 0, int pageSize = 10);

    Task DownloadWallpapers(int days);
}