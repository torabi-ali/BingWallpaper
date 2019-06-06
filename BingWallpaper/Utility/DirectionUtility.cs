using System.IO;
using System.Linq;

namespace BingWallpaper.Utility
{
    public static class DirectionUtility
    {
        public static string CleanFileName(this string fileName)
        {
            return Path.GetInvalidFileNameChars().Aggregate(fileName, (current, c) => current.Replace(c.ToString(), string.Empty));
        }
    }
}