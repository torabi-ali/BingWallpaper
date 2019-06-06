using BingWallpaper.Helpers;
using BingWallpaper.Utility;
using MaterialDesignThemes.Wpf;

namespace BingWallpaper.ViewModels
{
    public class SettingViewModel : BaseViewModel
    {
        #region Fields
        public SnackbarMessageQueue MessageQueue { get; set; }
        public string FolderPath { get; set; }
        public bool RunOnStarup { get; set; }
        #endregion

        #region Commands
        public RelayCommand SaveCommand { get; set; }
        public RelayCommand CloseCommand { get; set; }
        #endregion

        public SettingViewModel()
        {
            #region Fields
            MessageQueue = new SnackbarMessageQueue();
            FolderPath = Properties.Settings.Default.FolderPath;
            RunOnStarup = Properties.Settings.Default.RunOnStartup;
            #endregion

            #region Commands
            SaveCommand = new RelayCommand(Save);
            CloseCommand = new RelayCommand(Close);
            #endregion
        }

        private void Save(object parameter)
        {
            if (FolderPath != Properties.Settings.Default.FolderPath)
            {
                MessageQueue.Enqueue("Copying Files to New Path");
                WindowsUtility.CopyAll(Properties.Settings.Default.FolderPath, FolderPath);
            }

            if (RunOnStarup != Properties.Settings.Default.RunOnStartup)
            {
                MessageQueue.Enqueue("Preparing startup option");
                Properties.Settings.Default.RunOnStartup = RunOnStarup;

            }

            Properties.Settings.Default.Save();
            Close(parameter);
        }

        private void Close(object parameter)
        {
            CloseWindow();
        }
    }
}
