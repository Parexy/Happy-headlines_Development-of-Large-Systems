using NewsletterService.DTOs;

namespace NewsletterService.Services;

public class NewsletterService : INewsletterService
{
    private readonly HttpClient _httpClient;

    public NewsletterService(HttpClient httpClient)
    {
        _httpClient = httpClient;
    }

    public async Task<NewsletterDto> CreateDailyNewsletter()
    {
        var articles = await _httpClient.GetFromJsonAsync<List<ArticleDto>>(
            "/api/articles/latest");

        if (articles == null)
        {
            articles = [];
        }

        var newsletter = new NewsletterDto
        {
            Id = Guid.NewGuid(),
            CreatedAt = DateTime.UtcNow,
            Articles = articles
        };

        // Send newsletter here

        return newsletter;
    }
}