using System.Windows.Input;

namespace Wpf.Helpers;

public class RelayCommand(Action<object> execute, Predicate<object> canExecute) : ICommand
{
    private readonly Action<object> execute = execute ?? throw new ArgumentNullException(nameof(execute));

    public RelayCommand(Action<object> execute) : this(execute, null)
    { }

    public event EventHandler CanExecuteChanged
    {
        add => CommandManager.RequerySuggested += value;
        remove => CommandManager.RequerySuggested -= value;
    }

    public bool CanExecute(object parameter)
    {
        return canExecute is null || canExecute(parameter);
    }

    public void Execute(object parameter)
    {
        execute(parameter);
    }
}
