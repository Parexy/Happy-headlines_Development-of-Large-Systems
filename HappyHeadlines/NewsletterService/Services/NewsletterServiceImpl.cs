using System.Net.Http.Json;
using NewsletterService.Clients;
using NewsletterService.DTOs;

namespace NewsletterService.Services;

public class NewsletterServiceImpl : INewsletterService
{
    private readonly HttpClient _httpClient;
    private readonly ISubscriberServiceClient _subscriberClient;

    public NewsletterServiceImpl(
        HttpClient httpClient,
        ISubscriberServiceClient subscriberClient)
    {
        _httpClient = httpClient;
        _subscriberClient = subscriberClient;
    }

    public async Task<NewsletterDto> CreateDailyNewsletter(
        CancellationToken cancellationToken = default)
    {
        // Retrieve articles.
        var articles =
            await _httpClient.GetFromJsonAsync<List<ArticleDto>>(
                "/api/articles/latest",
                cancellationToken) ?? [];

        // Retrieve subscribers through fault-isolated client.
        var subscriberResult =
            await _subscriberClient.GetSubscribersAsync(
                cancellationToken);

        // Generate newsletter even when SubscriberService fails.
        var newsletter = new NewsletterDto
        {
            Id = Guid.NewGuid(),
            CreatedAt = DateTime.UtcNow,
            Articles = articles,

            SubscriberLookupStatus =
                subscriberResult.Status.ToString(),

            SubscriberCount =
                subscriberResult.Status ==
                SubscriberLookupStatus.Available
                    ? subscriberResult.Subscribers.Count
                    : null
        };

        return newsletter;
    }
}