using Messaging;
using Messaging.Events;
using Messaging.RabbitMq;

namespace NewsletterService.Messaging;

public sealed class ArticlePublishedConsumer
    : BackgroundService
{
    private readonly RabbitMqConsumer _consumer;
    private readonly ILogger<ArticlePublishedConsumer> _logger;

    public ArticlePublishedConsumer(
        RabbitMqConsumer consumer,
        ILogger<ArticlePublishedConsumer> logger)
    {
        _consumer = consumer;
        _logger = logger;
    }

    protected override Task ExecuteAsync(
        CancellationToken stoppingToken)
    {
        return _consumer.SubscribeAsync<ArticlePublished>(
            ArticleMessaging.Exchange,
            ArticleMessaging.NewsletterServiceQueue,
            HandleAsync,
            stoppingToken);
    }

    private Task HandleAsync(
        ArticlePublished article,
        CancellationToken cancellationToken)
    {
        _logger.LogInformation(
            "NewsletterService received published article {Title} for {Region}",
            article.Title,
            article.Region);

        // Send immediate newsletter here later.

        return Task.CompletedTask;
    }
}