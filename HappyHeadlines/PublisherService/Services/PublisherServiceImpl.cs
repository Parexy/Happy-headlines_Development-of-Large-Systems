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
        var now = DateTime.UtcNow;

        if (article.Id == Guid.Empty)
        {
            article.Id = Guid.NewGuid();
        }

        if (article.CreatedAt == default)
        {
            article.CreatedAt = now;
        }

        article.PublishedAt = now;
        article.UpdatedAt = now;

        var articlePublished = new ArticlePublished(
            article.Title,
            article.Content,
            article.Author,
            article.Region,
            article.PublishedAt);

        await _publisher.PublishAsync(
            ArticleMessaging.Exchange,
            articlePublished);

        return article;
    }
}