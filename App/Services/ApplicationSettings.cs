namespace App.Services;
public record ApplicationSettings
{
    public string ApplicationName { get; set; }

    public bool RunOnStartup { get; set; }

    public string BasePath => $"{Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments), ApplicationName)}";
}
