using Microsoft.Extensions.DependencyInjection;

namespace Wpf.ViewModels;

public class ViewModelLocator
{
    public static MainViewModel MainViewModel => App.ServiceProvider.GetRequiredService<MainViewModel>();
}
