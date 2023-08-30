using App.Services;
using Data.Models;
using MaterialDesignThemes.Wpf;
using Microsoft.Extensions.DependencyInjection;
using System.Collections.ObjectModel;
using System.Diagnostics;
using Wpf.Helpers;
using Wpf.Utility;

namespace Wpf.ViewModels
{
    public class MainViewModel : BaseViewModel
    {
        public static SnackbarMessageQueue MessageQueue => App.MessageQueue;

        public RelayCommand SetWallpaperCommand { get; set; }

        public RelayCommand SettingCommand { get; set; }

        public RelayCommand AboutCommand { get; set; }

        public RelayCommand CloseCommand { get; set; }

        public ObservableCollection<ImageInfo> Images { get; set; }

        private ImageInfo _selectedImage = new();
        public ImageInfo SelectedImage { get => _selectedImage; set { _selectedImage = value; RaisePropertyChanged(nameof(SelectedImage)); } }

        public MainViewModel()
        {
            SetWallpaperCommand = new RelayCommand(SetWallpaper);
            SettingCommand = new RelayCommand(Setting);
            AboutCommand = new RelayCommand(About);
            CloseCommand = new RelayCommand(Close);

            Initialize().Wait();
        }

        public async Task Initialize()
        {
            MessageQueue.Enqueue("Loading Wallpapers ...");

            Images = new ObservableCollection<ImageInfo>();

            var imageService = App.ServiceProvider.GetRequiredService<IImageService>();
            var images = await imageService.GetImagesAsync();
            foreach (var image in images)
            {
                Images.Add(image);
            }

            SelectedImage = Images.FirstOrDefault();

            MessageQueue.Clear();
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
    }
}
