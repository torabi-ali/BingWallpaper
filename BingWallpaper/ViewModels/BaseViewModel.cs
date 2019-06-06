using System;
using System.ComponentModel;
using System.Windows;
using System.Windows.Threading;

namespace BingWallpaper.ViewModels
{
    public class BaseViewModel : INotifyPropertyChanged
    {
        public event PropertyChangedEventHandler PropertyChanged;
        private bool? _CloseWindowFlag;
        private double _CurrentProgress;

        internal void RaisePropertyChanged(string prop)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(prop));
        }

        public bool? CloseWindowFlag
        {
            get => _CloseWindowFlag; set
            {
                _CloseWindowFlag = value;
                RaisePropertyChanged(nameof(CloseWindowFlag));
            }
        }

        public double CurrentProgress
        {
            get => _CurrentProgress;
            set
            {
                _CurrentProgress = value;
                RaisePropertyChanged(nameof(CurrentProgress));
            }
        }

        public virtual void CloseWindow(bool? result = true)
        {
            Application.Current.Dispatcher.BeginInvoke(DispatcherPriority.Background, new Action(() =>
            {
                CloseWindowFlag = CloseWindowFlag == null ? true : !CloseWindowFlag;
            }));
        }
    }
}
