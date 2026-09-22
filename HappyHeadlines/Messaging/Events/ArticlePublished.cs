namespace Messaging.Events;

public sealed record ArticlePublished(
    string Title,
    string Content,
    string Author,
    string Region,
    DateTimeOffset PublishedAt);