using Messaging.Events;

namespace PublisherService.Messaging;

public interface IArticlePublisher
{
    Task PublishAsync(
        ArticlePublished article,
        CancellationToken cancellationToken = default);
}