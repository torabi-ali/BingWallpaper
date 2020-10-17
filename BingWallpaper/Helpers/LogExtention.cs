using Microsoft.Extensions.Configuration;
using Serilog;
using Serilog.Core;
using System;

namespace BingWallpaper.Helpers
{
    public static class LogExtention
    {
        #region Properties
        public static readonly Logger Logger;
        #endregion

        #region Ctor
        static LogExtention()
        {
            var configuration = new ConfigurationBuilder().AddJsonFile("appsettings.json", optional: false, reloadOnChange: true).Build();

            Logger = new LoggerConfiguration()
                .ReadFrom.Configuration(configuration)
                .CreateLogger();

        }
        #endregion

        #region Methods
        public static void Log(this Exception exception)
        {
            Logger.Error("Execption occured ==> {@exception}", exception);
        }
        #endregion
    }
}
