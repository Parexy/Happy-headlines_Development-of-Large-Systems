using System.Net;
using System.Net.Http.Json;
using System.Text.Json;
using Polly.CircuitBreaker;
using Polly.Timeout;

namespace NewsletterService.Clients;

public class SubscriberServiceClient
    : ISubscriberServiceClient
{
    private readonly HttpClient _httpClient;
    private readonly ILogger<SubscriberServiceClient> _logger;

    public SubscriberServiceClient(
        HttpClient httpClient,
        ILogger<SubscriberServiceClient> logger)
    {
        _httpClient = httpClient;
        _logger = logger;
    }

    public async Task<SubscriberLookupResult> GetSubscribersAsync(
        CancellationToken cancellationToken = default)
    {
        try
        {
            using var response = await _httpClient.GetAsync(
                "/api/subscribers",
                cancellationToken);

            // Release toggle disabled.
            if (response.StatusCode == HttpStatusCode.NotFound)
            {
                return new(
                    SubscriberLookupStatus.Disabled,
                    []);
            }

            if (!response.IsSuccessStatusCode)
            {
                _logger.LogWarning(
                    "SubscriberService returned {StatusCode}",
                    response.StatusCode);

                return new(
                    SubscriberLookupStatus.Unavailable,
                    []);
            }

            var subscribers = await response.Content
                .ReadFromJsonAsync<List<SubscriberDto>>(
                    cancellationToken);

            if (subscribers is null)
            {
                return new(
                    SubscriberLookupStatus.Unavailable,
                    []);
            }

            return new(
                SubscriberLookupStatus.Available,
                subscribers);
        }
        catch (OperationCanceledException)
            when (cancellationToken.IsCancellationRequested)
        {
            throw;
        }
        catch (Exception ex) when (
            ex is HttpRequestException
            or BrokenCircuitException
            or TimeoutRejectedException
            or TaskCanceledException
            or JsonException)
        {
            _logger.LogWarning(
                ex,
                "SubscriberService unavailable. " +
                "Continuing without subscriber data.");

            return new(
                SubscriberLookupStatus.Unavailable,
                []);
        }
    }
}