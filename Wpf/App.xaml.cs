using App.Services;
using App.Services.Implementations;
using Data.DbContexts;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using NLog.Extensions.Logging;
using System.Globalization;
using System.IO;
using System.Windows;
using System.Windows.Markup;
using Wpf.Infrastructure;
using Wpf.ViewModels;

namespace Wpf;

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

        var currentDomain = AppDomain.CurrentDomain;
        currentDomain.UnhandledException += CurrentDomain_UnhandledException;
    }

    protected override void OnStartup(StartupEventArgs e)
    {
        SetDefaultCulture();

        MigrateDatabase();

        SetUserSettings();

        base.OnStartup(e);
    }

    protected void CurrentDomain_UnhandledException(object sender, UnhandledExceptionEventArgs e)
    {
        var logger = ServiceProvider.GetRequiredService<ILogger<App>>();

        var ex = (Exception)e.ExceptionObject;
        logger.LogError(ex, "Error from sender: {sender}", sender);
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

        services.AddSingleton<MainViewModel>();
    }

    private static void SetDefaultCulture()
    {
        var baseCulture = new CultureInfo("en-UK");
        Thread.CurrentThread.CurrentCulture = baseCulture;
        Thread.CurrentThread.CurrentUICulture = baseCulture;
        CultureInfo.DefaultThreadCurrentCulture = baseCulture;
        CultureInfo.DefaultThreadCurrentUICulture = baseCulture;
        FrameworkElement.LanguageProperty.OverrideMetadata(typeof(FrameworkElement), new FrameworkPropertyMetadata(XmlLanguage.GetLanguage(CultureInfo.CurrentCulture.IetfLanguageTag)));
    }

    private static void SetUserSettings()
    {
        var settings = ServiceProvider.GetRequiredService<ApplicationSettings>();
        if (settings.RunOnStartup)
        {
            NativeMethods.EnableRunOnStartup();
        }
        else
        {
            NativeMethods.DisableRunOnStartup();
        }

        if (!Directory.Exists(settings.BasePath))
        {
            Directory.CreateDirectory(settings.BasePath);
        }
    }

    private static void MigrateDatabase()
    {
        var dbContext = ServiceProvider.GetRequiredService<ApplicationDbContext>();
        dbContext.Database.Migrate();
    }
}
