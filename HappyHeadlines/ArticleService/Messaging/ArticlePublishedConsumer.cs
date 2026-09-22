using ArticleService.Data;
using ArticleService.Models;
using Messaging;
using Messaging.Events;
using Messaging.RabbitMq;

namespace ArticleService.Messaging;

public sealed class ArticlePublishedConsumer
    : BackgroundService
{
    private readonly RabbitMqConsumer _consumer;
    private readonly IArticleDbContextFactory _dbContextFactory;
    private readonly ILogger<ArticlePublishedConsumer> _logger;

    public ArticlePublishedConsumer(
        RabbitMqConsumer consumer,
        IArticleDbContextFactory dbContextFactory,
        ILogger<ArticlePublishedConsumer> logger)
    {
        _consumer = consumer;
        _dbContextFactory = dbContextFactory;
        _logger = logger;
    }

    protected override Task ExecuteAsync(
        CancellationToken stoppingToken)
    {
        return _consumer.SubscribeAsync<ArticlePublished>(
            ArticleMessaging.Exchange,
            ArticleMessaging.ArticleServiceQueue,
            HandleAsync,
            stoppingToken);
    }

    private async Task HandleAsync(
        ArticlePublished message,
        CancellationToken cancellationToken)
    {
        if (!Enum.TryParse<ArticleRegion>(
                message.Region,
                true,
                out var region))
        {
            throw new InvalidOperationException(
                $"Unknown article region '{message.Region}'.");
        }

        await using var db =
            _dbContextFactory.Create(region);

        var now = DateTime.UtcNow;

        var article = new Article
        {
            Title = message.Title,
            Content = message.Content,
            Author = message.Author,

            CreatedAt = now,
            PublishedAt = message.PublishedAt.UtcDateTime,
            UpdatedAt = now
        };

        db.Articles.Add(article);

        await db.SaveChangesAsync(cancellationToken);

        _logger.LogInformation(
            "Stored published article {ArticleId} in {Region}",
            article.Id,
            region);
    }
}