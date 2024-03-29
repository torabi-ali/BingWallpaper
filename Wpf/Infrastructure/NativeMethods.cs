﻿using System.Reflection;
using System.Runtime.InteropServices;
using Microsoft.Win32;

namespace Wpf.Infrastructure;

public static partial class NativeMethods
{
    private const int SPI_SETDESKWALLPAPER = 20;
    private const int SPIF_UPDATEINIFILE = 0x01;
    private const int SPIF_SENDWININICHANGE = 0x02;

    public enum WallpaperStyle
    {
        Center = 0,
        Tile = 1,
        Stretch = 2,
        Fit = 3,
        Fill = 4,
        Span = 5
    }

    public static void SetLockScreen(string wallpaper)
    {
        using var registryKey = Registry.CurrentUser.OpenSubKey(@"HKEY_LOCAL_MACHINE\SOFTWARE\Policies\Microsoft\Windows\Personalization", true);
        registryKey.SetValue($"LockScreenImage", wallpaper);
    }

    public static void SetWallpaper(string wallpaper, WallpaperStyle wallpaperStyle = WallpaperStyle.Fill, bool tileWallpaper = false)
    {
        using var registryKey = Registry.CurrentUser.OpenSubKey(@"Control Panel\Desktop", true);
        registryKey.SetValue(@"WallpaperStyle", ((int)wallpaperStyle).ToString());
        registryKey.SetValue(@"TileWallpaper", tileWallpaper ? 1.ToString() : 0.ToString());

        _ = SystemParametersInfo(SPI_SETDESKWALLPAPER, 0, wallpaper, SPIF_UPDATEINIFILE | SPIF_SENDWININICHANGE);
    }

    public static void EnableRunOnStartup()
    {
        using var registryKey = Registry.CurrentUser.OpenSubKey(@"SOFTWARE\Microsoft\Windows\CurrentVersion\Run", true);
        registryKey.SetValue(Assembly.GetExecutingAssembly().GetName().Name, Environment.ProcessPath);
    }

    public static void DisableRunOnStartup()
    {
        using var registryKey = Registry.CurrentUser.OpenSubKey(@"SOFTWARE\Microsoft\Windows\CurrentVersion\Run", true);
        registryKey.DeleteValue(Assembly.GetExecutingAssembly().GetName().Name, false);
    }

    #region Private Methods

    [DllImport("user32.dll", CharSet = CharSet.Auto)]
    private static extern int SystemParametersInfo(int uAction, int uParam, string lpvParam, int fuWinIni);

    #endregion
}