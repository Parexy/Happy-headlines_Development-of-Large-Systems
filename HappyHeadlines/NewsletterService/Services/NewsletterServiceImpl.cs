using Messaging;
using Messaging.Events;
using Messaging.RabbitMq;
using PublisherService.DTOs;

namespace PublisherService.Services;

public class PublisherServiceImpl : IPublisherService
{
    private readonly RabbitMqPublisher _publisher;

    public PublisherServiceImpl(
        RabbitMqPublisher publisher)
    {
        _publisher = publisher;
    }

    public async Task<PublishArticleDto> PublishArticle(
        PublishArticleDto article)
    {
        var articlePublished =
            new ArticlePublished(
                article.Title,
                article.Content,
                article.Author,
                article.Region,
                DateTimeOffset.UtcNow);

        await _publisher.PublishAsync(
            ArticleMessaging.Exchange,
            articlePublished);

        return article;
    }
}