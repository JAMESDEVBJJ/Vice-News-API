using System.Text.Json;

namespace ViceNews.API.Models;

public class News
{
    public long Id { get; set; }
    public string Title { get; set; } = string.Empty;
    public string Slug { get; set; } = string.Empty;
    public string Summary { get; set; } = string.Empty;
    public JsonDocument Content { get; set; } = null!;
    public string HeroImageUrl { get; set; } = string.Empty;
    public bool Featured { get; set; }
    public long ViewCount { get; set; }
    public int ReadingTimeMinutes { get; set; }
    public string? SidebarNoteLabel { get; set; }
    public string? SidebarNoteText { get; set; }

    public long CategoryId { get; set; }
    public Category Category { get; set; } = null!;

    public long AuthorId { get; set; }
    public User Author { get; set; } = null!;

    public DateTimeOffset PublishedAt { get; set; }
    public DateTimeOffset CreatedAt { get; set; }
    public DateTimeOffset UpdatedAt { get; set; }

    public ICollection<NewsTag> NewsTags { get; set; } = new List<NewsTag>();
}
