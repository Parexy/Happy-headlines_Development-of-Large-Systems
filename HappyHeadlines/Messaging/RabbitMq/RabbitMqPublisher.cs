using System.Text.Json;
using RabbitMQ.Client;

namespace Messaging.RabbitMq;

public sealed class RabbitMqPublisher
{
    private readonly RabbitMqConnection _rabbitMqConnection;

    public RabbitMqPublisher(
        RabbitMqConnection rabbitMqConnection)
    {
        _rabbitMqConnection = rabbitMqConnection;
    }

    public async Task PublishAsync<T>(
        string exchange,
        T message,
        CancellationToken cancellationToken = default)
    {
        var connection =
            await _rabbitMqConnection.GetConnectionAsync(
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

        var body =
            JsonSerializer.SerializeToUtf8Bytes(message);

        var properties =
            new BasicProperties
            {
                ContentType = "application/json",
                DeliveryMode = DeliveryModes.Persistent
            };

        await channel.BasicPublishAsync(
            exchange: exchange,
            routingKey: string.Empty,
            mandatory: false,
            basicProperties: properties,
            body: body,
            cancellationToken:
                cancellationToken);
    }
}