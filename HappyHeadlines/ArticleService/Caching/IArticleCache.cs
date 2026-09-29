using ArticleService.Models;

namespace ArticleService.Caching;

public interface IArticleCache
{
    Task<Article?> GetAsync(
        int articleId,
        CancellationToken cancellationToken = default);

    Task SetAsync(
        Article article,
        TimeSpan expiration);

    Task RemoveAsync(int articleId);
}