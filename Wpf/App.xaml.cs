using App.Services;
using App.Services.Implementations;
using Data.DbContexts;
using MaterialDesignThemes.Wpf;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using NLog.Extensions.Logging;
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
        public static SnackbarMessageQueue MessageQueue { get; private set; }

        private readonly IConfiguration _config;

        public App()
        {
            _config = new ConfigurationBuilder().AddJsonFile("appsettings.json").Build();

            MessageQueue = new SnackbarMessageQueue();

            var services = new ServiceCollection();
            ConfigureServices(services);
            ServiceProvider = services.BuildServiceProvider();

            var currentDomain = AppDomain.CurrentDomain;
            currentDomain.UnhandledException += CurrentDomain_UnhandledException;
        }

        private async void App_Startup(object sender, StartupEventArgs e)
        {
            var logger = ServiceProvider.GetRequiredService<ILogger<App>>();

            var settings = ServiceProvider.GetRequiredService<ApplicationSettings>();
            if (settings.RunOnStartup)
            {
                NativeMethods.EnableRunOnStartup(settings.ApplicationName);
                logger.LogInformation("Enable run on startup");
            }
            else
            {
                NativeMethods.DisableRunOnStartup(settings.ApplicationName);
                logger.LogInformation("Disable run on startup");
            }

            if (!Directory.Exists(settings.BasePath))
            {
                Directory.CreateDirectory(settings.BasePath);
                logger.LogInformation("Create base path directory");
            }

            await DownloadTodayImageAsync();

            var window = ServiceProvider.GetRequiredService<MainWindow>();
            window.Show();
        }

        private static async Task DownloadTodayImageAsync()
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

            services.AddLogging(logBuilder =>
            {
                logBuilder.ClearProviders();
                logBuilder.AddNLog("NLog.config");
            });

            var applicationSettings = _config.GetSection("ApplicationSettings").Get<ApplicationSettings>();
            services.AddSingleton(applicationSettings);

            services.AddSingleton<IBingDownloaderService, BingDownloaderService>();
            services.AddSingleton<IImageService, ImageService>();

            services.AddSingleton<MainViewModel>();
            services.AddSingleton<MainWindow>();
        }

        private void CurrentDomain_UnhandledException(object sender, UnhandledExceptionEventArgs e)
        {
            var logger = ServiceProvider.GetRequiredService<ILogger<App>>();

            var ex = (Exception)e.ExceptionObject;
            logger.LogError(ex, "");
        }
    }
}
