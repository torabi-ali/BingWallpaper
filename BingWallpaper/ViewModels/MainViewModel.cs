using BingWallpaper.Core;
using BingWallpaper.Data;
using BingWallpaper.Helpers;
using BingWallpaper.Models;
using MaterialDesignThemes.Wpf;
using System;
using System.Collections.ObjectModel;
using System.Linq;

namespace BingWallpaper.ViewModels
{
    public class MainViewModel : BaseViewModel
    {
        #region Fields
        public SnackbarMessageQueue MessageQueue { get; set; }
        public ObservableCollection<ImageInfo> Images { get; set; }

        private ImageInfo _selectedImage;
        public ImageInfo SelectedImage { get => _selectedImage; set { _selectedImage = value; RaisePropertyChanged(nameof(SelectedImage)); } }

        private static readonly ApplicationDbContext _dbContext = new ApplicationDbContext();
        #endregion

        #region Commands
        public RelayCommand SetWallpaperCommand { get; set; }
        public RelayCommand AboutCommand { get; set; }
        public RelayCommand CloseCommand { get; set; }
        #endregion

        public MainViewModel()
        {
            #region Fields
            MessageQueue = new SnackbarMessageQueue();
            Images = new ObservableCollection<ImageInfo>();
            SelectedImage = new ImageInfo();
            #endregion

            #region Commands
            SetWallpaperCommand = new RelayCommand(SetWallpaper);
            AboutCommand = new RelayCommand(About);
            CloseCommand = new RelayCommand(Close);
            #endregion

            Initialize();
        }

        public void Initialize()
        {
            MessageQueue.Enqueue("Loading Wallpapers ...");

            var images = _dbContext.ImageInfos.Where(p => p.Date >= DateTime.Today.AddDays(-7));
            foreach (var image in images)
            {
                Images.Add(image);
            }

            SelectedImage = Images.FirstOrDefault();
        }

        private void SetWallpaper(object parameter)
        {
            MessageQueue.Enqueue("Trying to Set Wallpaper.");
            BingWallpaperDownloader.SetWallpaper(SelectedImage);
        }

        private void About(object parameter)
        {
        }

        private void Close(object parameter)
        {
            CloseWindow();
        }
    }
}
