namespace PublisherService.DTOs;

public sealed record PublishArticleRequest(
    string Title,
    string Content,
    string Author,
    string Region);