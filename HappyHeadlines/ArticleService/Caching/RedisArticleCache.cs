using System.Text.Json;
using ArticleService.Models;
using StackExchange.Redis;

namespace ArticleService.Caching;

public sealed class RedisArticleCache : IArticleCache
{
    private readonly IDatabase _database;

    public RedisArticleCache(
        IConnectionMultiplexer connectionMultiplexer)
    {
        _database = connectionMultiplexer.GetDatabase();
    }

    public async Task<Article?> GetAsync(
        int articleId,
        CancellationToken cancellationToken = default)
    {
        var value = await _database.StringGetAsync(
            GetKey(articleId));

        if (value.IsNullOrEmpty)
        {
            return null;
        }

        return JsonSerializer.Deserialize<Article>(
            value.ToString());
    }

    public async Task SetAsync(
        Article article,
        TimeSpan expiration)
    {
        var json = JsonSerializer.Serialize(article);

        await _database.StringSetAsync(
            GetKey(article.Id),
            json,
            expiration);
    }

    public Task RemoveAsync(int articleId)
    {
        return _database.KeyDeleteAsync(
            GetKey(articleId));
    }

    private static string GetKey(int articleId)
    {
        return $"articles:global:{articleId}";
    }
}