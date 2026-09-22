namespace NewsletterService.DTOs;

public class NewsletterDto
{
    public Guid Id { get; set; }

    public DateTime CreatedAt { get; set; }

    public List<ArticleDto> Articles { get; set; } = [];
}