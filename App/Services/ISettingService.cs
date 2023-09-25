namespace App.Services;

public interface ISettingService
{
    ApplicationSettings LoadData();

    void SaveData(ApplicationSettings applicationSettings);
}
