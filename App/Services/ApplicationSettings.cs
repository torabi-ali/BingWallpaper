namespace App.Services;

public record ApplicationSettings
{
    public bool RunOnStartup { get; set; }

    public int InitialLoadingImageCount { get; set; }

    public string BasePath => $"{Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.MyPictures), "BingWallpaper")}";
}
