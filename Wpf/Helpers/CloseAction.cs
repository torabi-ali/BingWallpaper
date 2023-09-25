using System.Windows;

namespace Wpf.Helpers;

public static class CloseAction
{
    public static readonly DependencyProperty CloseCommandProperty =
        DependencyProperty.RegisterAttached("CloseCommand", typeof(bool), typeof(CloseAction), new PropertyMetadata(OnCloseCommandChanged));

    private static void OnCloseCommandChanged(DependencyObject dependencyObject, DependencyPropertyChangedEventArgs e)
    {
        if (dependencyObject is Window window)
        {
            window.Close();
        }
    }

    public static void SetCloseCommand(Window target, bool? value)
    {
        target.SetValue(CloseCommandProperty, value);
    }
}
