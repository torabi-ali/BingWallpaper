using Data.DbContexts;
using Data.Models;
using Microsoft.EntityFrameworkCore;

namespace App.Services.Implementations;

public class SettingService : ISettingService
{
    private readonly ApplicationDbContext _dbContext;

    public SettingService(ApplicationDbContext dbContext)
    {
        _dbContext = dbContext;
    }

    public ApplicationSettings LoadData()
    {
        var applicationSettings = new ApplicationSettings();
        var settings = _dbContext.Settings.AsNoTracking().ToList();

        if (settings.Count == 0)
        {
            LoadObjectWithDefaultData(applicationSettings);
        }
        else
        {
            LoadObject(applicationSettings, settings);
        }

        return applicationSettings;
    }

    public void SaveData(ApplicationSettings applicationSettings)
    {
        var settings = _dbContext.Settings.ToList();

        UnLoadObject(applicationSettings, settings);

        _dbContext.SaveChanges();
    }

    private void LoadObject(ApplicationSettings applicationSettings, IEnumerable<Setting> settings)
    {
        var properties = applicationSettings.GetType().GetProperties();
        foreach (var property in properties)
        {
            var value = settings.Where(p => p.Key.Equals(property.Name)).Select(p => p.Value).SingleOrDefault();
            property.SetValue(applicationSettings, Convert.ChangeType(value, property.PropertyType), null);
        }
    }

    private void LoadObjectWithDefaultData(ApplicationSettings applicationSettings)
    {
        var defaultSettings = new Dictionary<string, object>()
        {
            { "RunOnStartup", "true" },
            { "InitialLoadingImageCount", "10" },
            { "BasePath", Environment.ExpandEnvironmentVariables(@"%userprofile%\Pictures\BingWallpaper") }
        };

        var properties = applicationSettings.GetType().GetProperties();
        foreach (var property in properties)
        {
            var value = defaultSettings.Where(p => p.Key.Equals(property.Name)).Select(p => p.Value).SingleOrDefault();
            property.SetValue(applicationSettings, Convert.ChangeType(value, property.PropertyType), null);
        }
    }

    private void UnLoadObject(ApplicationSettings applicationSettings, IEnumerable<Setting> settings)
    {
        var properties = applicationSettings.GetType().GetProperties();
        foreach (var property in properties)
        {
            var value = property.GetValue(applicationSettings);
            var row = settings.SingleOrDefault(p => p.Key.Equals(property.Name));
            if (row is null)
            {
                row = new Setting { Key = property.Name, Value = value.ToString() };
                _dbContext.Settings.Add(row);
            }
            else
            {
                row.Value = value.ToString();
                _dbContext.Settings.Update(row);
            }
        }
    }
}
