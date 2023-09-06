namespace App.Services;

public record ApplicationSettings
{
    public ApplicationSettings(bool runOnStartup, int initialLoadingImageCount, string basePath)
    {
        RunOnStartup = runOnStartup;
        InitialLoadingImageCount = initialLoadingImageCount;
        BasePath = Environment.ExpandEnvironmentVariables(basePath);
    }

    public bool RunOnStartup { get; private set; }

    public int InitialLoadingImageCount { get; private set; }

    public string BasePath { get; private set; }
}
