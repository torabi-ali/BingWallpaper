using BingWallpaper.Data;
using BingWallpaper.Views;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using System;
using System.IO;
using System.Windows;

namespace BingWallpaper
{
    public partial class App : Application
    {
        public IServiceProvider ServiceProvider { get; private set; }
        public IConfiguration Configuration { get; private set; }

        public App()
        {
            ServiceCollection services = new ServiceCollection();
            ConfigureServices(services);
            ServiceProvider = services.BuildServiceProvider();

            var builder = new ConfigurationBuilder()
                .SetBasePath(Directory.GetCurrentDirectory())
                .AddJsonFile("appsettings.json", optional: false, reloadOnChange: true);

            Configuration = builder.Build();
        }

        private void ConfigureServices(IServiceCollection services)
        {
            services.AddDbContext<ApplicationDbContext>(options =>
            {
                options.UseSqlite("Data Source = Employee.db");
            });

            services.AddSingleton<MainWindow>();
        }

        private void OnStartup(object sender, StartupEventArgs e)
        {
            var mainWindow = ServiceProvider.GetService<MainWindow>();
            mainWindow.Show();
        }
    }
}
