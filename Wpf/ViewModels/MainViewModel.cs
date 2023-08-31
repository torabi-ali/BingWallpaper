using App.Services;
using Data.Models;
using MaterialDesignThemes.Wpf;
using Microsoft.Extensions.DependencyInjection;
using System.Collections.ObjectModel;
using System.Diagnostics;
using System.Windows;
using Wpf.Helpers;
using Wpf.Utility;

namespace Wpf.ViewModels
{
    public class MainViewModel : BaseViewModel
    {
        private const int IMAGE_COUNT = 7;

        public static SnackbarMessageQueue MessageQueue { get; private set; }

        public RelayCommand SetWallpaperCommand { get; set; }

        public RelayCommand SettingCommand { get; set; }

        public RelayCommand AboutCommand { get; set; }

        public RelayCommand CloseCommand { get; set; }

        public ObservableCollection<ImageInfo> Images { get; set; }

        private ImageInfo _selectedImage = new();
        public ImageInfo SelectedImage { get => _selectedImage; set { _selectedImage = value; RaisePropertyChanged(nameof(SelectedImage)); } }

        public MainViewModel()
        {
            Images = new ObservableCollection<ImageInfo>();
            MessageQueue = new SnackbarMessageQueue();

            SetWallpaperCommand = new RelayCommand(SetWallpaper);
            SettingCommand = new RelayCommand(Setting);
            AboutCommand = new RelayCommand(About);
            CloseCommand = new RelayCommand(Close);

            Initialize();
        }

        public async void Initialize()
        {
            await LoadImagesAsync();

            await DownloadImagesAsync();
        }

        private void SetWallpaper(object parameter)
        {
            MessageQueue.Enqueue("Trying to Set Wallpaper.");

            NativeMethods.SetWallpaper(SelectedImage.Name);
        }

        private void Setting(object parameter)
        {
            MessageQueue.Enqueue("Please edit this file and restart the app");

            Process.Start(new ProcessStartInfo
            {
                FileName = "appsettings.json",
                UseShellExecute = true
            });
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

            var imageService = App.ServiceProvider.GetRequiredService<IImageService>();
            var images = await imageService.GetImagesAsync(pageSize: IMAGE_COUNT);
            foreach (var image in images)
            {
                Application.Current.Dispatcher.Invoke(delegate
                {
                    Images.Add(image);
                });
            }

            Images = new ObservableCollection<ImageInfo>(Images.OrderByDescending(p => p.CreatedOn));
            SelectedImage = Images.FirstOrDefault();

            MessageQueue.Clear();
        }

        private async Task DownloadImagesAsync()
        {
            MessageQueue.Enqueue("Downloading New Wallpapers ...");

            var imageService = App.ServiceProvider.GetRequiredService<IImageService>();
            var lastImageDays = SelectedImage is null ? 0 : (DateTime.Today - SelectedImage.CreatedOn).Days;
            var bingImageAvailability = IMAGE_COUNT - Images.Count;

            var days = Math.Max(lastImageDays, bingImageAvailability);
            if (days > 0)
            {
                MessageQueue.Enqueue("Downloading New Wallpapers ...");

                await Task.Run(() => imageService.DownloadImagesAsync(days));

                await Task.Run(LoadImagesAsync);

                MessageQueue.Clear();
            }

            MessageQueue.Clear();
        }
    }
}
