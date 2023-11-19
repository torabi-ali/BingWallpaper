using App.Dtos;
using Data.DbContexts;
using Data.Models;
using Microsoft.EntityFrameworkCore;

namespace App.Services.Implementations;

public class SettingService(ApplicationDbContext dbContext) : ISettingService
{
    public ApplicationSettings LoadData()
    {
        var applicationSettings = new ApplicationSettings();
        var settings = dbContext.Settings.AsNoTracking().ToList();

        if (settings.Count == 0)
        {
            LoadObjectWithDefaultData(applicationSettings);
            SaveData(applicationSettings);
        }
        else
        {
            LoadObject(applicationSettings, settings);
        }

        return applicationSettings;
    }

    public void SaveData(ApplicationSettings applicationSettings)
    {
        var settings = dbContext.Settings.ToList();

        UnLoadObject(applicationSettings, settings);

        dbContext.SaveChanges();
    }

    private static void LoadObject(ApplicationSettings applicationSettings, IEnumerable<Setting> settings)
    {
        var properties = applicationSettings.GetType().GetProperties();
        foreach (var property in properties)
        {
            var value = settings.Where(p => p.Key.Equals(property.Name)).Select(p => p.Value).SingleOrDefault();
            property.SetValue(applicationSettings, Convert.ChangeType(value, property.PropertyType), null);
        }
    }

    private static void LoadObjectWithDefaultData(ApplicationSettings applicationSettings)
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
                dbContext.Settings.Add(row);
            }
            else
            {
                row.Value = value.ToString();
                dbContext.Settings.Update(row);
            }
        }
    }
}
