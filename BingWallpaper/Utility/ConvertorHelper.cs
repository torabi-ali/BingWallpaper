using System;
using System.IO;
using System.Xml.Serialization;

namespace BingWallpaper.Utility
{
    public static class ConvertorHelper
    {
        public static string ToImageName(this DateTime dateTime)
        {
            return dateTime.ToShortDateString().Replace('/', '-');
        }

        public static T DeserializeXml<T>(this string xml) where T : class
        {
            using (var reader = new StringReader(xml))
            {
                return (T)new XmlSerializer(typeof(T)).Deserialize(reader);
            }
        }
    }
}