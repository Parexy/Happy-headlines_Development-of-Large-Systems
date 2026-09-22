namespace Messaging.Events;

public sealed record ArticlePublished(
    int ArticleId,
    string Title,
    string Content,
    string Author,
    DateTimeOffset PublishedAt);