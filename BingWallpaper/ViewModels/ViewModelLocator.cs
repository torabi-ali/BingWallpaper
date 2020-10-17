using Microsoft.Extensions.DependencyInjection;

namespace BingWallpaper.ViewModels
{
    public class ViewModelLocator
    {
        public MainViewModel MainViewModel => App.ServiceProvider.GetService<MainViewModel>();
    }
}
