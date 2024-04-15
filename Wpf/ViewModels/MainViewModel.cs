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

    public async void InitializeAsync()
    {
        await Task.Run(LoadImagesAsync);

        await Task.Run(DownloadImagesAsync);
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

    private async Task LoadImagesAsync()
    {
        MessageQueue.Enqueue("Loading Wallpapers ...");

        Images = [];
        var images = await bingDownloaderService.GetImagesPagedAsync(pageSize: applicationSettings.InitialLoadingImageCount);
        foreach (var image in images)
        {
            Application.Current.Dispatcher.Invoke(delegate { Images.Add(image); });
        }

        if (images.Count > 0)
        {
            SortImages();
        }
    }

    private async Task DownloadImagesAsync()
    {

        var days = SelectedImage is null
            ? BING_IMAGE_AVAILABLE_DAYS
            : Math.Min((DateTime.Today - SelectedImage.CreatedOn).Days, BING_IMAGE_AVAILABLE_DAYS);

        if (days > 0)
        {
            for (var i = days - 1; i >= 0; i--)
            {
                MessageQueue.Clear();
                MessageQueue.Enqueue("Downloading New Wallpapers ...");

                await Task.Run(async () =>
                {
                    var image = await bingDownloaderService.GetBingImageAsync(i);
                    if (image is not null)
                    {
                        Application.Current.Dispatcher.Invoke(delegate { Images.Add(image); });
                    }
                });
            }

            SortImages();
        }

    }

    private void SortImages()
    {
        Images = new ObservableCollection<ImageInfo>(Images.OrderByDescending(p => p.CreatedOn));
        RaisePropertyChanged(nameof(Images));
        SelectedImage = Images.FirstOrDefault();
    }
}
