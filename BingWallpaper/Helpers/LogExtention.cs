using Serilog;
using Serilog.Core;
using System;
using System.IO;

namespace BingWallpaper.Helpers
{
    public static class LogExtention
    {
        #region Properties
        public static readonly Logger Logger;

        private static readonly string LogName = "Log-";
        private static readonly string LogPath = "D:/Logs";
        private static readonly string LogFullPath = Path.Combine(LogPath, $"{LogName}.txt");
        #endregion

        #region Ctor
        static LogExtention()
        {
            Logger = new LoggerConfiguration()
                .WriteTo.File(LogFullPath, rollingInterval: RollingInterval.Day, retainedFileCountLimit: 30)
                .MinimumLevel.Debug()
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
