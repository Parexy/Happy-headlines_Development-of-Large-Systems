namespace NewsletterService.Clients;

public record SubscriberDto(
    Guid Id,
    string Email,
    DateTime CreatedAt);

public enum SubscriberLookupStatus
{
    Available,
    Disabled,
    Unavailable
}

public record SubscriberLookupResult(
    SubscriberLookupStatus Status,
    IReadOnlyList<SubscriberDto> Subscribers);

public interface ISubscriberServiceClient
{
    Task<SubscriberLookupResult> GetSubscribersAsync(
        CancellationToken cancellationToken = default);
}