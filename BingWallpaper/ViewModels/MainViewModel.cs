using BingWallpaper.Core;
using BingWallpaper.Data;
using BingWallpaper.Helpers;
using BingWallpaper.Models;
using BingWallpaper.Views;
using MaterialDesignThemes.Wpf;
using System.Collections.ObjectModel;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using System.Windows;

namespace BingWallpaper.ViewModels
{
    public class MainViewModel : BaseViewModel
    {
        #region Fields
        public SnackbarMessageQueue MessageQueue { get; set; }
        public ObservableCollection<ImageInfo> Images { get; set; }

        private ImageInfo _selectedImage;
        public ImageInfo SelectedImage { get => _selectedImage; set { _selectedImage = value; RaisePropertyChanged(nameof(SelectedImage)); } }

        private static readonly ApplicationDbContext _applicationDbContext = new ApplicationDbContext();
        #endregion

        #region Commands
        public RelayCommand SetWallpaperCommand { get; set; }
        public RelayCommand SettingCommand { get; set; }
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
            SettingCommand = new RelayCommand(Setting);
            AboutCommand = new RelayCommand(About);
            CloseCommand = new RelayCommand(Close);
            #endregion

            Task.Run(() => Initialize());
        }

        public void Initialize()
        {
            MessageQueue.Enqueue("Loading Wallpapers ...");

            var wait = 0;
            while (Properties.Settings.Default.WaitForNetwork)
            {
                if (wait < 5)
                {
                    Thread.Sleep(1000); //wait for the new image to add
                    wait++;
                }
                else
                {
                    MessageQueue.Enqueue("There is a problem with Internet Connection");
                    break;
                }
            }

            var images = _applicationDbContext.GetImagesRange();
            foreach (var image in images)
            {
                Application.Current.Dispatcher.Invoke(delegate
                {
                    Images.Add(image);
                });
            }
            SelectedImage = Images.FirstOrDefault();
        }

        private void SetWallpaper(object parameter)
        {
            MessageQueue.Enqueue("Trying to Set Wallpaper.");
            BingWallpaperDownloader.SetWallpaper(SelectedImage);
        }

        private void Setting(object parameter)
        {
            var setting = new SettingWindow();
            setting.Show();
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
