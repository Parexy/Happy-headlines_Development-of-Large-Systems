using Microsoft.Extensions.Configuration;
using RabbitMQ.Client;

namespace Messaging.RabbitMq;

public sealed class RabbitMqConnection
    : IAsyncDisposable
{
    private readonly IConfiguration _configuration;

    private readonly SemaphoreSlim _lock =
        new(1, 1);

    private IConnection? _connection;

    public RabbitMqConnection(
        IConfiguration configuration)
    {
        _configuration = configuration;
    }

    public async Task<IConnection> GetConnectionAsync(
        CancellationToken cancellationToken = default)
    {
        if (_connection is { IsOpen: true })
        {
            return _connection;
        }

        await _lock.WaitAsync(cancellationToken);

        try
        {
            if (_connection is { IsOpen: true })
            {
                return _connection;
            }

            var factory = new ConnectionFactory
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

            return _connection;
        }
        finally
        {
            _lock.Release();
        }
    }

    public async ValueTask DisposeAsync()
    {
        if (_connection is not null)
        {
            await _connection.DisposeAsync();
        }

        _lock.Dispose();
    }
}