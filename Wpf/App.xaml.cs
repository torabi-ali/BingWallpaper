using App.Services;
using App.Services.Implementations;
using Data.DbContexts;
using MaterialDesignThemes.Wpf;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using System.IO;
using System.Windows;
using Wpf.Utility;
using Wpf.ViewModels;
using Wpf.Views;

namespace Wpf
{
    public partial class App : Application
    {
        public static IServiceProvider ServiceProvider { get; private set; }
        public static SnackbarMessageQueue MessageQueue = new SnackbarMessageQueue();

        private readonly IConfiguration _config;

        public App()
        {
            _config = new ConfigurationBuilder().AddJsonFile("appsettings.json").Build();

            var services = new ServiceCollection();

            ConfigureServices(services);
            ServiceProvider = services.BuildServiceProvider();
        }

        private async void App_Startup(object sender, StartupEventArgs e)
        {
            var settings = ServiceProvider.GetRequiredService<ApplicationSettings>();
            if (settings.RunOnStartup)
            {
                NativeMethods.EnableRunOnStartup(settings.ApplicationName);
            }
            else
            {
                NativeMethods.DisableRunOnStartup(settings.ApplicationName);
            }

            if (!Directory.Exists(settings.BasePath))
            {
                Directory.CreateDirectory(settings.BasePath);
            }

            await DownloadTodayImageAsync();

            var window = ServiceProvider.GetRequiredService<MainWindow>();
            window.Show();
        }

        private async Task DownloadTodayImageAsync()
        {
            MessageQueue.Enqueue("Downloading New Wallpaper ...");

            var imageService = ServiceProvider.GetRequiredService<IImageService>();
            await imageService.DownloadTodayImageAsync();

            MessageQueue.Clear();
        }

        private void ConfigureServices(ServiceCollection services)
        {
            services.AddDbContext<ApplicationDbContext>(options =>
            {
                options.UseSqlite(_config.GetConnectionString("DefaultConnection"));
            });

            services.AddHttpClient();

            var applicationSettings = _config.GetSection("ApplicationSettings").Get<ApplicationSettings>();
            services.AddSingleton(applicationSettings);

            services.AddTransient<IBingDownloaderService, BingDownloaderService>();
            services.AddTransient<IImageService, ImageService>();
            
            services.AddTransient<MainViewModel>();
            services.AddTransient<MainWindow>();
        }
    }
}
