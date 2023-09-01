using App.Services;
using App.Services.Implementations;
using Data.DbContexts;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using NLog.Extensions.Logging;
using System.IO;
using System.Windows;
using Wpf.Utility;
using Wpf.ViewModels;

namespace Wpf
{
    public partial class App : Application
    {
        public static IServiceProvider ServiceProvider { get; private set; }

        private readonly IConfiguration _config;

        public App()
        {
            _config = new ConfigurationBuilder().AddJsonFile("appsettings.json").Build();

            var services = new ServiceCollection();
            ConfigureServices(services);
            ServiceProvider = services.BuildServiceProvider();

            var dbContext = ServiceProvider.GetRequiredService<ApplicationDbContext>();
            dbContext.Database.EnsureCreated();

            var currentDomain = AppDomain.CurrentDomain;
            currentDomain.UnhandledException += CurrentDomain_UnhandledException;
        }

        protected override void OnStartup(StartupEventArgs e)
        {
            var logger = ServiceProvider.GetRequiredService<ILogger<App>>();

            var settings = ServiceProvider.GetRequiredService<ApplicationSettings>();
            if (settings.RunOnStartup)
            {
                NativeMethods.EnableRunOnStartup();
                logger.LogInformation("Enable run on startup");
            }
            else
            {
                NativeMethods.DisableRunOnStartup();
                logger.LogInformation("Disable run on startup");
            }

            if (!Directory.Exists(settings.BasePath))
            {
                Directory.CreateDirectory(settings.BasePath);
                logger.LogInformation("Create base path directory");
            }
        }

        private void ConfigureServices(ServiceCollection services)
        {
            var applicationSettings = _config.GetSection("ApplicationSettings").Get<ApplicationSettings>();
            services.AddSingleton(applicationSettings);

            services.AddDbContext<ApplicationDbContext>(options =>
            {
                options.UseSqlite($"DataSource={Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "BingWallpaper.db")};");
            });

            services.AddHttpClient();

            services.AddLogging(logBuilder =>
            {
                logBuilder.ClearProviders();
                logBuilder.AddNLog("NLog.config");
            });

            services.AddSingleton<IBingDownloaderService, BingDownloaderService>();
            services.AddSingleton<IImageService, ImageService>();

            services.AddSingleton<MainViewModel>();
        }

        private void CurrentDomain_UnhandledException(object sender, UnhandledExceptionEventArgs e)
        {
            var logger = ServiceProvider.GetRequiredService<ILogger<App>>();

            var ex = (Exception)e.ExceptionObject;
            logger.LogError(ex, $"Error from snder: {sender}");
        }
    }
}
