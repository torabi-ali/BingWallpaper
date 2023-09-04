using Data.Models;

namespace App.Services;

public interface IBingDownloaderService
{
    Task<IList<ImageInfo>> GetWallpapers(int days);
}