using System;
using System.Xml.Serialization;

namespace BingWallpaper.Models
{
    /// <remarks/>
    [SerializableAttribute()]
    [XmlType(AnonymousType = true)]
    [XmlRoot(Namespace = "", IsNullable = false)]
    public partial class images
    {
        /// <remarks/>
        public imagesImage image { get; set; }

        /// <remarks/>
        public imagesTooltips tooltips { get; set; }
    }

    /// <remarks/>
    [SerializableAttribute()]
    [XmlType(AnonymousType = true)]
    public partial class imagesImage
    {

        /// <remarks/>
        public uint startdate { get; set; }

        /// <remarks/>
        public ulong fullstartdate { get; set; }

        /// <remarks/>
        public uint enddate { get; set; }

        /// <remarks/>
        public string url { get; set; }

        /// <remarks/>
        public string urlBase { get; set; }

        /// <remarks/>
        public string copyright { get; set; }

        /// <remarks/>
        public string copyrightlink { get; set; }

        /// <remarks/>
        public byte drk { get; set; }

        /// <remarks/>
        public byte top { get; set; }

        /// <remarks/>
        public byte bot { get; set; }

        /// <remarks/>
        public object hotspots { get; set; }
    }

    /// <remarks/>
    [SerializableAttribute()]
    [XmlType(AnonymousType = true)]
    public partial class imagesTooltips
    {
        /// <remarks/>
        public imagesTooltipsLoadMessage loadMessage { get; set; }

        /// <remarks/>
        public imagesTooltipsPreviousImage previousImage { get; set; }

        /// <remarks/>
        public imagesTooltipsNextImage nextImage { get; set; }

        /// <remarks/>
        public imagesTooltipsPlay play { get; set; }

        /// <remarks/>
        public imagesTooltipsPause pause { get; set; }
    }

    /// <remarks/>
    [SerializableAttribute()]
    [XmlType(AnonymousType = true)]
    public partial class imagesTooltipsLoadMessage
    {
        /// <remarks/>
        public string message { get; set; }
    }

    /// <remarks/>
    [SerializableAttribute()]
    [XmlType(AnonymousType = true)]
    public partial class imagesTooltipsPreviousImage
    {
        /// <remarks/>
        public string text { get; set; }
    }

    /// <remarks/>
    [SerializableAttribute()]
    [XmlType(AnonymousType = true)]
    public partial class imagesTooltipsNextImage
    {
        /// <remarks/>
        public string text { get; set; }
    }

    /// <remarks/>
    [SerializableAttribute()]
    [XmlType(AnonymousType = true)]
    public partial class imagesTooltipsPlay
    {
        /// <remarks/>
        public string text { get; set; }
    }

    /// <remarks/>
    [SerializableAttribute()]
    [XmlType(AnonymousType = true)]
    public partial class imagesTooltipsPause
    {
        /// <remarks/>
        public string text { get; set; }
    }
}