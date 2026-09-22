using System.Text.Json;
using Messaging;
using Messaging.Events;
using RabbitMQ.Client;

namespace PublisherService.Messaging;

public sealed class RabbitMqArticlePublisher
    : IArticlePublisher,
      IAsyncDisposable
{
    private readonly IConfiguration _configuration;

    private readonly SemaphoreSlim _initializationLock =
        new(1, 1);

    private IConnection? _connection;
    private IChannel? _channel;

    public RabbitMqArticlePublisher(
        IConfiguration configuration)
    {
        _configuration = configuration;
    }

    public async Task PublishAsync(
        ArticlePublished article,
        CancellationToken cancellationToken = default)
    {
        await EnsureInitializedAsync(
            cancellationToken);

        var body =
            JsonSerializer.SerializeToUtf8Bytes(
                article);

        var properties =
            new BasicProperties
            {
                ContentType = "application/json",
                Persistent = true
            };

        await _channel!.BasicPublishAsync(
            exchange: ArticleMessaging.Exchange,
            routingKey: string.Empty,
            mandatory: false,
            basicProperties: properties,
            body: body,
            cancellationToken: cancellationToken);
    }

    private async Task EnsureInitializedAsync(
        CancellationToken cancellationToken)
    {
        if (_channel is not null)
        {
            return;
        }

        await _initializationLock.WaitAsync(
            cancellationToken);

        try
        {
            if (_channel is not null)
            {
                return;
            }

            var factory =
                new ConnectionFactory
                {
                    HostName =
                        _configuration["RabbitMq:Host"]
                        ?? "rabbitmq",

                    Port =
                        int.TryParse(
                            _configuration["RabbitMq:Port"],
                            out var port)
                            ? port
                            : 5672,

                    UserName =
                        _configuration["RabbitMq:UserName"]
                        ?? "happy",

                    Password =
                        _configuration["RabbitMq:Password"]
                        ?? "headlines",

                    AutomaticRecoveryEnabled = true
                };

            _connection =
                await factory.CreateConnectionAsync(
                    cancellationToken);

            _channel =
                await _connection.CreateChannelAsync(
                    cancellationToken:
                        cancellationToken);

            await _channel.ExchangeDeclareAsync(
                exchange: ArticleMessaging.Exchange,
                type: ExchangeType.Fanout,
                durable: true,
                autoDelete: false,
                cancellationToken:
                    cancellationToken);
        }
        finally
        {
            _initializationLock.Release();
        }
    }

    public async ValueTask DisposeAsync()
    {
        if (_channel is not null)
        {
            await _channel.DisposeAsync();
        }

        if (_connection is not null)
        {
            await _connection.DisposeAsync();
        }

        _initializationLock.Dispose();
    }
}