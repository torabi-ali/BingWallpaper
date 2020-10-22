using BingWallpaper.Core;
using BingWallpaper.Data;
using BingWallpaper.Helpers;
using BingWallpaper.Models;
using MaterialDesignThemes.Wpf;
using Microsoft.Extensions.Configuration;
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
        private readonly IConfiguration _configuration;
        private readonly DateTime LastDownloadDate;
        private readonly DateTime BaseDate = DateTime.Today.AddDays(-6);
        #endregion

        #region Commands
        public RelayCommand SetWallpaperCommand { get; set; }
        public RelayCommand AboutCommand { get; set; }
        public RelayCommand CloseCommand { get; set; }
        #endregion

        public MainViewModel(ApplicationDbContext dbContext, IConfiguration configuration)
        {
            _dbContext = dbContext;
            _configuration = configuration;
            LastDownloadDate = new DateTime(Math.Max(_configuration.GetValue("LastDownloadDate", BaseDate).Ticks, BaseDate.Ticks));

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
            DownloadLatestImages();
        }

        private void Initialize()
        {
            MessageQueue.Enqueue("Loading Wallpapers ...");

            var tmpImages = _dbContext.Images.Where(p => p.Date >= DateTime.Today.AddDays(-7));
            foreach (var item in tmpImages)
            {
                Images.Add(item);
            }
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

        private void DownloadLatestImages()
        {
            var tmpImages = BingWallpaperDownloader.GetWallpapers(LastDownloadDate);
            foreach (var item in tmpImages)
            {
                _dbContext.Images.Add(item);
                Images.Add(item);
            }

            if (tmpImages.Any())
            {
                _dbContext.SaveChanges();
                _configuration["LastDownloadDate"] = DateTime.Today.ToShortDateString();
            }

            SelectedImage = Images.OrderByDescending(p => p.Date).FirstOrDefault();
        }
    }
}
