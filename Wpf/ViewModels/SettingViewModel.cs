using App.Dtos;
using App.Services;
using Microsoft.Extensions.DependencyInjection;
using Wpf.Helpers;
using Wpf.Infrastructure;

namespace Wpf.ViewModels;

public class SettingViewModel : BaseViewModel
{
    private readonly ISettingService _settingService;

    public ApplicationSettings ApplicationSettings { get; private set; }

    public RelayCommand SaveCommand { get; set; }

    public RelayCommand CloseCommand { get; set; }

    public SettingViewModel()
    {
        _settingService = App.ServiceProvider.GetRequiredService<ISettingService>();
        ApplicationSettings = App.ServiceProvider.GetRequiredService<ApplicationSettings>();

        SaveCommand = new RelayCommand(SaveSettings);
        CloseCommand = new RelayCommand(Close);
    }

    private void SaveSettings(object parameter)
    {
        _settingService.SaveData(ApplicationSettings);

        ApplicationSettings.Apply();
        CloseWindow();
    }

    private void Close(object parameter)
    {
        CloseWindow();
    }
}
