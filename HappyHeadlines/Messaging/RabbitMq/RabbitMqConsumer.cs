using System.Text.Json;
using Microsoft.Extensions.Logging;
using RabbitMQ.Client;
using RabbitMQ.Client.Events;

namespace Messaging.RabbitMq;

public sealed class RabbitMqConsumer
{
    private readonly RabbitMqConnection _connection;
    private readonly ILogger<RabbitMqConsumer> _logger;

    public RabbitMqConsumer(
        RabbitMqConnection connection,
        ILogger<RabbitMqConsumer> logger)
    {
        _connection = connection;
        _logger = logger;
    }

    public async Task SubscribeAsync<T>(
        string exchange,
        string queue,
        Func<T, CancellationToken, Task> handler,
        CancellationToken cancellationToken)
    {
        var connection =
            await _connection.GetConnectionAsync(
                cancellationToken);

        await using var channel =
            await connection.CreateChannelAsync(
                cancellationToken:
                    cancellationToken);

        await channel.ExchangeDeclareAsync(
            exchange: exchange,
            type: ExchangeType.Fanout,
            durable: true,
            autoDelete: false,
            cancellationToken:
                cancellationToken);

        await channel.QueueDeclareAsync(
            queue: queue,
            durable: true,
            exclusive: false,
            autoDelete: false,
            cancellationToken:
                cancellationToken);

        await channel.QueueBindAsync(
            queue: queue,
            exchange: exchange,
            routingKey: string.Empty,
            cancellationToken:
                cancellationToken);

        var consumer =
            new AsyncEventingBasicConsumer(channel);

        consumer.ReceivedAsync += async (_, args) =>
        {
            try
            {
                var message =
                    JsonSerializer.Deserialize<T>(
                        args.Body.Span);

                if (message is null)
                {
                    throw new InvalidOperationException(
                        "Message could not be deserialized.");
                }

                await handler(
                    message,
                    cancellationToken);

                await channel.BasicAckAsync(
                    args.DeliveryTag,
                    multiple: false);
            }
            catch (Exception exception)
            {
                _logger.LogError(
                    exception,
                    "Failed processing message from {Queue}",
                    queue);

                await channel.BasicNackAsync(
                    args.DeliveryTag,
                    multiple: false,
                    requeue: false);
            }
        };

        await channel.BasicConsumeAsync(
            queue: queue,
            autoAck: false,
            consumer: consumer,
            cancellationToken:
                cancellationToken);

        try
        {
            await Task.Delay(
                Timeout.Infinite,
                cancellationToken);
        }
        catch (OperationCanceledException)
            when (cancellationToken.IsCancellationRequested)
        {
        }
    }
}