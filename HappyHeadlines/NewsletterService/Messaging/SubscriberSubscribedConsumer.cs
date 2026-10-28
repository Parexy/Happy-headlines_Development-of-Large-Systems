using Messaging;
using Messaging.Events;
using Messaging.RabbitMq;

namespace NewsletterService.Messaging;

public sealed class SubscriberSubscribedConsumer
    : BackgroundService
{
    private readonly RabbitMqConsumer _consumer;
    private readonly ILogger<SubscriberSubscribedConsumer> _logger;

    public SubscriberSubscribedConsumer(
        RabbitMqConsumer consumer,
        ILogger<SubscriberSubscribedConsumer> logger)
    {
        _consumer = consumer;
        _logger = logger;
    }

    protected override Task ExecuteAsync(
        CancellationToken stoppingToken)
    {
        return _consumer.SubscribeAsync<SubscriberSubscribed>(
            SubscriberMessaging.Exchange,
            SubscriberMessaging.SubscriberQueue,
            HandleAsync,
            stoppingToken);
    }

    private Task HandleAsync(
        SubscriberSubscribed subscriber,
        CancellationToken cancellationToken)
    {
        _logger.LogInformation(
            "Received SubscriberSubscribed event for " +
            "subscriber {SubscriberId}",
            subscriber.SubscriberId);

        // Simulated welcome email.
        // Replace this with actual email delivery when needed.
        _logger.LogInformation(
            "Welcome email would be sent to {Email}",
            subscriber.Email);

        return Task.CompletedTask;
    }
}