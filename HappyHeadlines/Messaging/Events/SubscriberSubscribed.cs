namespace Messaging.Events;

public sealed record SubscriberSubscribed(
    Guid SubscriberId,
    string Email,
    DateTime CreatedAt);