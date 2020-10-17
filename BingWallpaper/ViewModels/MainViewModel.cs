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
        public ObservableCollection<ApplicationImage> Images { get; set; }

        private ApplicationImage _selectedImage;
        public ApplicationImage SelectedImage { get => _selectedImage; set { _selectedImage = value; RaisePropertyChanged(nameof(SelectedImage)); } }

        private readonly ApplicationDbContext _dbContext;
        #endregion

        #region Commands
        public RelayCommand SetWallpaperCommand { get; set; }
        public RelayCommand AboutCommand { get; set; }
        public RelayCommand CloseCommand { get; set; }
        #endregion

        public MainViewModel(ApplicationDbContext dbContext)
        {
            _dbContext = dbContext;

            #region Fields
            MessageQueue = new SnackbarMessageQueue();
            Images = new ObservableCollection<ApplicationImage>();
            SelectedImage = new ApplicationImage();
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

            var images = _dbContext.Images.Where(p => p.Date >= DateTime.Today.AddDays(-7));
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
