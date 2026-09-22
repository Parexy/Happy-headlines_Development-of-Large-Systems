using PublisherService.DTOs;

namespace PublisherService.Services;

public interface IPublisherService
{
    Task<PublishArticleDto> PublishArticle(PublishArticleDto article);
}