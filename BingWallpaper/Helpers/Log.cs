using System;
using System.Diagnostics;
using System.Globalization;
using System.IO;
using System.Text;
using System.Windows;

namespace BingWallpaper.Helpers
{
    public static class LogException
    {
        private static readonly string FILE_NAME = $"{Application.ResourceAssembly.GetName().Name}.log";
        private static readonly string FILE_PATH = $"{Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData)}\\{Properties.Settings.Default.CompanyName}";
        private static readonly string FULL_FILE_PATH = $"{FILE_PATH}\\{FILE_NAME}";

        public static void Log(this Exception ex)
        {
            StringBuilder error = new StringBuilder();

            error.AppendLine("Date:                       " + DateTime.Now.ToString("dd/MM/yyyy HH:mm:ss"));
            error.AppendLine("Computer Name:              " + Environment.MachineName);
            error.AppendLine("User Name:                  " + Environment.UserName);
            error.AppendLine("OS:                         " + Environment.OSVersion.ToString());
            error.AppendLine("Culture:                    " + CultureInfo.CurrentCulture.Name);
            error.AppendLine("App UpTime:                 " + (DateTime.Now - Process.GetCurrentProcess().StartTime).ToString());
            error.AppendLine("");

            error.AppendLine("Exception Message:          " + ex.Message);
            error.AppendLine("");

            error.AppendLine("Exception Source:           " + ex.Source);
            error.AppendLine("");

            error.AppendLine("Exception HelpLink:         " + ex.HelpLink);
            error.AppendLine("");

            error.AppendLine("Exception TargetSite:       " + ex.TargetSite);
            error.AppendLine("");

            error.AppendLine("Exception InnerException:   " + ex.InnerException?.ToString().Replace("   ", "                            "));
            error.AppendLine("");

            error.AppendLine("Stack StackTrace:           " + ex.StackTrace?.ToString().Replace("   ", "").Replace("\n", "                            "));
            error.AppendLine("");

            error.AppendLine("Loaded Modules:             ");
            Process thisProcess = Process.GetCurrentProcess();
            foreach (ProcessModule module in thisProcess.Modules)
            {
                error.AppendLine("                            " + module.FileName + " " + module.FileVersionInfo.FileVersion);
            }
            error.AppendLine("\n- - - - - - - - - - * * * * * * * * * - - - - - - - - - -\n");
            error.Save();
        }

        public static void Save(this StringBuilder error)
        {
            if (!File.Exists(FULL_FILE_PATH))
            {
                File.Create(FULL_FILE_PATH).Close();
            }
            File.AppendAllText(FULL_FILE_PATH, error.ToString(), Encoding.UTF8);
        }
    }
}
