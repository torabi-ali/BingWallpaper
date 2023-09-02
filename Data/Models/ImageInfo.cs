namespace Data.Models;

public class ImageInfo
{
    public int Id { get; set; }

    public string Name { get; set; }

    public string Path { get; set; }

    public string Headline { get; set; }

    public string Url { set; get; }

    public string Copyright { set; get; }

    public DateTime CreatedOn { set; get; }
}