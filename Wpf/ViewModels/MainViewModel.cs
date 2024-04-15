using System.Collections.ObjectModel;
using System.Diagnostics;
using System.Windows;
using App.Dtos;
using App.Services;
using Data.Models;
using MaterialDesignThemes.Wpf;
using Microsoft.Extensions.DependencyInjection;
using Wpf.Helpers;
using Wpf.Infrastructure;
using Wpf.Views;

namespace Wpf.ViewModels;

public class MainViewModel : BaseViewModel
{
    private const int BING_IMAGE_AVAILABLE_DAYS = 7;

    private readonly ApplicationSettings applicationSettings;
    private readonly IBingDownloaderService bingDownloaderService;

    public static SnackbarMessageQueue MessageQueue { get; private set; }

    public RelayCommand SetWallpaperCommand { get; set; }

    public RelayCommand SettingCommand { get; set; }

    public RelayCommand AboutCommand { get; set; }

    public RelayCommand CloseCommand { get; set; }

    public ObservableCollection<ImageInfo> Images { get; set; }

    private ImageInfo selectedImage;
    public ImageInfo SelectedImage { get => selectedImage; set { selectedImage = value; RaisePropertyChanged(nameof(SelectedImage)); } }

    public MainViewModel()
    {
        applicationSettings = App.ServiceProvider.GetRequiredService<ApplicationSettings>();
        bingDownloaderService = App.ServiceProvider.GetRequiredService<IBingDownloaderService>();
        MessageQueue = new SnackbarMessageQueue();

        SetWallpaperCommand = new RelayCommand(SetWallpaper);
        SettingCommand = new RelayCommand(Setting);
        AboutCommand = new RelayCommand(About);
        CloseCommand = new RelayCommand(Close);

        InitializeAsync();
    }

    // Just a wrapper to change return type from "Task" to "void"
    public async void InitializeAsync()
    {
        await DownloadImagesAsync();
    }

    private void SetWallpaper(object parameter)
    {
        MessageQueue.Enqueue("Trying to Set Wallpaper.");

        NativeMethods.SetWallpaper(SelectedImage.Path);
    }

    private void Setting(object parameter)
    {
        var settingWindow = App.ServiceProvider.GetRequiredService<SettingWindow>();
        settingWindow.Show();
    }

    private void About(object parameter)
    {
        if (SelectedImage is not null)
        {
            Process.Start(new ProcessStartInfo
            {
                FileName = $"https://bing.com{SelectedImage.Url}",
                UseShellExecute = true
            });
        }
    }

    private void Close(object parameter)
    {
        CloseWindow();
    }

    private async Task DownloadImagesAsync()
    {
        MessageQueue.Enqueue("Downloading New Wallpapers ...");

        var days = SelectedImage is null
            ? BING_IMAGE_AVAILABLE_DAYS
            : Math.Min((DateTime.Today - SelectedImage.CreatedOn).Days, BING_IMAGE_AVAILABLE_DAYS);

        if (days > 0)
        {
            var downloaderTasks = new List<Task>();
            for (var i = days - 1; i >= 0; i--)
            {
                var task = bingDownloaderService.GetBingImageAsync(i);
                downloaderTasks.Add(task);
            }

            await Task.WhenAll(downloaderTasks);
        }

        await LoadImagesAsync();

        MessageQueue.Clear();
    }

    private async Task LoadImagesAsync()
    {
        Images = [];
        var images = await bingDownloaderService.GetImagesPagedAsync(pageSize: applicationSettings.InitialLoadingImageCount);
        foreach (var image in images)
        {
            Application.Current.Dispatcher.Invoke(delegate
            { Images.Add(image); });
        }

        SelectedImage = Images.FirstOrDefault();
    }
}
