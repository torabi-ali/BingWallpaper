using System.Xml.Serialization;

namespace App.Dtos;

[XmlRoot(ElementName = "image")]
public record BingImageDto
{
    [XmlElement(ElementName = "url")]
    public string Url { get; set; }

    [XmlElement(ElementName = "urlBase")]
    public string UrlBase { get; set; }

    [XmlElement(ElementName = "copyright")]
    public string Copyright { get; set; }

    [XmlElement(ElementName = "copyrightlink")]
    public string CopyrightLink { get; set; }

    [XmlElement(ElementName = "headline")]
    public string Headline { get; set; }
}

[XmlRoot(ElementName = "images")]
public record BingImagesDto
{
    [XmlElement(ElementName = "image")]
    public required BingImageDto Image { get; set; }
}

