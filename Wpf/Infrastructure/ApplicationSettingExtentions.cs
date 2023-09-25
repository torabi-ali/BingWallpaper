using App.Services;
using System.IO;

namespace Wpf.Infrastructure;

public static class ApplicationSettingsExtentions
{
    public static void Apply(this ApplicationSettings applicationSettings)
    {
        if (!Directory.Exists(applicationSettings.BasePath))
        {
            Directory.CreateDirectory(applicationSettings.BasePath);
        }

        if (applicationSettings.RunOnStartup)
        {
            NativeMethods.EnableRunOnStartup();
        }
        else
        {
            NativeMethods.DisableRunOnStartup();
        }
    }
}