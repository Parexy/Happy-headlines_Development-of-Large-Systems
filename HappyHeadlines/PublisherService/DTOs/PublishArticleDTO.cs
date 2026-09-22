namespace PublisherService.DTOs;

public class PublishArticleDto
{
    public Guid Id { get; set; }

    public string Title { get; set; } = string.Empty;

    public string Content { get; set; } = string.Empty;

    public string Author { get; set; } = string.Empty;

    public DateTime CreatedAt { get; set; }

    public DateTime PublishedAt { get; set; }

    public DateTime UpdatedAt { get; set; }
}