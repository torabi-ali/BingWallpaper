using System.ComponentModel;

namespace Wpf.ViewModels;

public class BaseViewModel : INotifyPropertyChanged
{
    public event PropertyChangedEventHandler PropertyChanged;

    public bool CloseWindowFlag { get; set; }

    internal void RaisePropertyChanged(string property)
    {
        PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(property));
    }

    internal void CloseWindow()
    {
        CloseWindowFlag = !CloseWindowFlag;
        RaisePropertyChanged(nameof(CloseWindowFlag));
    }
}
