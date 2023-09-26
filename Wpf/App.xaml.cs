using App.Dtos;
using App.Services;
using App.Services.Implementations;
using Data.DbContexts;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using NLog.Extensions.Logging;
using System.Globalization;
using System.IO;
using System.Windows;
using System.Windows.Markup;
using Wpf.Infrastructure;
using Wpf.ViewModels;
using Wpf.Views;

namespace Wpf;

public partial class App : Application
{
    public static IServiceProvider ServiceProvider { get; private set; }

    public App()
    {
        var services = new ServiceCollection();
        ConfigureServices(services);
        ServiceProvider = services.BuildServiceProvider();

        var currentDomain = AppDomain.CurrentDomain;
        currentDomain.UnhandledException += CurrentDomain_UnhandledException;
    }

    protected override void OnStartup(StartupEventArgs e)
    {
        SetDefaultCulture();

        MigrateDatabase();

        base.OnStartup(e);
    }

    protected void CurrentDomain_UnhandledException(object sender, UnhandledExceptionEventArgs e)
    {
        var logger = ServiceProvider.GetRequiredService<ILogger<App>>();

        var ex = (Exception)e.ExceptionObject;
        logger.LogError(ex, "Unhandled Exception");
    }

    private static void ConfigureServices(ServiceCollection services)
    {
        services.AddDbContext<ApplicationDbContext>(options => { options.UseSqlite($"DataSource={Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "BingWallpaper.db")}"); }, ServiceLifetime.Singleton);

        services.AddHttpClient("Default", client =>
        {
            client.BaseAddress = new Uri("https://bing.com");
        });

        services.AddLogging(logBuilder =>
        {
            logBuilder.ClearProviders();
            logBuilder.AddNLog("NLog.config");
        });

        services.AddSingleton<IBingDownloaderService, BingDownloaderService>();
        services.AddSingleton<ISettingService, SettingService>();

        services.AddSingleton<ApplicationSettings>(opt =>
        {
            var settingService = opt.GetService<ISettingService>();
            var applicationSettings = settingService.LoadData();
            applicationSettings.Apply();

            return applicationSettings;
        });

        services.AddTransient<MainViewModel>();
        services.AddTransient<MainWindow>();

        services.AddTransient<SettingViewModel>();
        services.AddTransient<SettingWindow>();
    }

    private static void SetDefaultCulture()
    {
        var baseCulture = new CultureInfo("en-GB");
        Thread.CurrentThread.CurrentCulture = baseCulture;
        Thread.CurrentThread.CurrentUICulture = baseCulture;
        CultureInfo.DefaultThreadCurrentCulture = baseCulture;
        CultureInfo.DefaultThreadCurrentUICulture = baseCulture;
        FrameworkElement.LanguageProperty.OverrideMetadata(typeof(FrameworkElement), new FrameworkPropertyMetadata(XmlLanguage.GetLanguage(CultureInfo.CurrentCulture.IetfLanguageTag)));
    }

    private static void MigrateDatabase()
    {
        var dbContext = ServiceProvider.GetRequiredService<ApplicationDbContext>();
        dbContext.Database.Migrate();
    }
}
