namespace NewsletterService.DTOs;

public class CreateNewsletterRequest
{
    public List<Guid> ArticleIds { get; set; } = [];
}