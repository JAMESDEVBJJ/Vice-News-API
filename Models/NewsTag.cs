namespace ViceNews.API.Models;

public class NewsTag
{
    public long NewsId { get; set; }
    public News News { get; set; } = null!;

    public long TagId { get; set; }
    public Tag Tag { get; set; } = null!;
}
