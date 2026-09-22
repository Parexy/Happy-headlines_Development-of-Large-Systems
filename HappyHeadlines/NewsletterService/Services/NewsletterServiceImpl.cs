using NewsletterService.DTOs;

namespace NewsletterService.Services;

public class NewsletterServiceImpl : INewsletterService
{
    private readonly HttpClient _httpClient;

    public NewsletterServiceImpl(HttpClient httpClient)
    {
        _httpClient = httpClient;
    }

    public async Task<NewsletterDto> CreateDailyNewsletter()
    {
        var articles =
            await _httpClient.GetFromJsonAsync<List<ArticleDto>>(
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

        // Send newsletter here later.

        return newsletter;
    }
}