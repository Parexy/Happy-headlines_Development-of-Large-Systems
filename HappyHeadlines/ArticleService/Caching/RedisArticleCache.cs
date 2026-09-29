using System.Text.Json;
using ArticleService.Models;
using StackExchange.Redis;

namespace ArticleService.Caching;

public sealed class RedisArticleCache : IArticleCache
{
    private readonly IDatabase _database;
    private readonly ILogger<RedisArticleCache> _logger;

    public RedisArticleCache(
        IConnectionMultiplexer connectionMultiplexer,
        ILogger<RedisArticleCache> logger)
    {
        _database =
            connectionMultiplexer.GetDatabase();

        _logger = logger;
    }


    public async Task<Article?> GetAsync(
        int articleId,
        CancellationToken cancellationToken = default)
    {
        try
        {
            var value =
                await _database.StringGetAsync(
                    GetKey(articleId));

            if (value.IsNullOrEmpty)
            {
                return null;
            }

            return JsonSerializer.Deserialize<Article>(
                value.ToString());
        }
        catch (RedisException exception)
        {
            // Redis failure becomes a cache miss.
            // ArticlesController will fall back to PostgreSQL.
            _logger.LogWarning(
                exception,
                "ArticleCache unavailable while reading article {ArticleId}. Falling back to the database.",
                articleId);

            return null;
        }
        catch (JsonException exception)
        {
            _logger.LogWarning(
                exception,
                "Invalid cached article data for article {ArticleId}. Falling back to the database.",
                articleId);

            return null;
        }
    }


    public async Task SetAsync(
        Article article,
        TimeSpan expiration)
    {
        try
        {
            var json =
                JsonSerializer.Serialize(article);

            await _database.StringSetAsync(
                GetKey(article.Id),
                json,
                expiration);
        }
        catch (RedisException exception)
        {
            // Cache warming failing must not affect
            // ArticleService availability.
            _logger.LogWarning(
                exception,
                "ArticleCache unavailable while caching article {ArticleId}.",
                article.Id);
        }
    }


    public async Task RemoveAsync(
        int articleId)
    {
        try
        {
            await _database.KeyDeleteAsync(
                GetKey(articleId));
        }
        catch (RedisException exception)
        {
            // UPDATE/DELETE already succeeded in PostgreSQL.
            // Redis failure must not turn that into HTTP 500.
            _logger.LogWarning(
                exception,
                "ArticleCache unavailable while invalidating article {ArticleId}.",
                articleId);
        }
    }


    private static string GetKey(
        int articleId)
    {
        return $"articles:global:{articleId}";
    }
}