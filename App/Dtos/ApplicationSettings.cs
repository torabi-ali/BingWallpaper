namespace App.Dtos;

public record ApplicationSettings
{
    public bool RunOnStartup { get; set; }

    public int InitialLoadingImageCount { get; set; }

    public string BasePath { get; set; }
}
