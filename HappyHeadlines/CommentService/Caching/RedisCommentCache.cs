using System.Text.Json;
using CommentService.Models;
using Microsoft.Extensions.Options;
using StackExchange.Redis;

namespace CommentService.Caching;

public sealed class RedisCommentCache : ICommentCache
{
    private const string LruKey =
        "comments:lru";

    private readonly IDatabase _database;
    private readonly int _maxArticles;
    private readonly ILogger<RedisCommentCache> _logger;

    public RedisCommentCache(
        IConnectionMultiplexer connectionMultiplexer,
        IOptions<CommentCacheOptions> options,
        ILogger<RedisCommentCache> logger)
    {
        _database =
            connectionMultiplexer.GetDatabase();

        _maxArticles =
            options.Value.MaxArticles;

        _logger = logger;
    }


    public async Task<IReadOnlyList<Comment>?> GetAsync(
        int articleId)
    {
        try
        {
            var key =
                GetArticleKey(articleId);

            var value =
                await _database.StringGetAsync(key);

            if (value.IsNull)
            {
                // Remove a possible stale LRU entry.
                await _database.SortedSetRemoveAsync(
                    LruKey,
                    articleId);

                return null;
            }

            // Cache hit:
            // mark the article as recently used.
            await TouchAsync(articleId);

            var comments =
                JsonSerializer.Deserialize<List<Comment>>(
                    value.ToString());

            return comments ?? [];
        }
        catch (RedisException exception)
        {
            // Redis is unavailable.
            // Treat this exactly like a cache miss so
            // the controller can query PostgreSQL.
            _logger.LogWarning(
                exception,
                "CommentCache unavailable while reading article {ArticleId}. Falling back to the database.",
                articleId);

            return null;
        }
        catch (JsonException exception)
        {
            // Corrupt cache data must never make the
            // application unavailable either.
            _logger.LogWarning(
                exception,
                "Invalid cached comment data for article {ArticleId}. Falling back to the database.",
                articleId);

            return null;
        }
    }


    public async Task SetAsync(
        int articleId,
        IReadOnlyList<Comment> comments)
    {
        try
        {
            var key =
                GetArticleKey(articleId);

            var json =
                JsonSerializer.Serialize(comments);

            await _database.StringSetAsync(
                key,
                json);

            await TouchAsync(articleId);

            await EvictLeastRecentlyUsedAsync();
        }
        catch (RedisException exception)
        {
            // The database request has already succeeded.
            // Failure to populate the cache must not fail
            // the user's request.
            _logger.LogWarning(
                exception,
                "CommentCache unavailable while caching article {ArticleId}. Continuing without cache.",
                articleId);
        }
    }


    public async Task RemoveAsync(
        int articleId)
    {
        try
        {
            var key =
                GetArticleKey(articleId);

            await _database.KeyDeleteAsync(key);

            await _database.SortedSetRemoveAsync(
                LruKey,
                articleId);
        }
        catch (RedisException exception)
        {
            // Cache invalidation failing must not turn a
            // successful database operation into HTTP 500.
            _logger.LogWarning(
                exception,
                "CommentCache unavailable while invalidating article {ArticleId}.",
                articleId);
        }
    }


    private async Task TouchAsync(
        int articleId)
    {
        var timestamp =
            DateTimeOffset.UtcNow
                .ToUnixTimeMilliseconds();

        await _database.SortedSetAddAsync(
            LruKey,
            articleId,
            timestamp);
    }


    private async Task EvictLeastRecentlyUsedAsync()
    {
        var count =
            await _database.SortedSetLengthAsync(
                LruKey);

        while (count > _maxArticles)
        {
            var leastRecentlyUsed =
                await _database
                    .SortedSetRangeByRankAsync(
                        LruKey,
                        start: 0,
                        stop: 0,
                        order: Order.Ascending);

            if (leastRecentlyUsed.Length == 0)
            {
                return;
            }

            var articleIdValue =
                leastRecentlyUsed[0];

            if (!int.TryParse(
                    articleIdValue.ToString(),
                    out var articleId))
            {
                await _database
                    .SortedSetRemoveAsync(
                        LruKey,
                        articleIdValue);

                count =
                    await _database
                        .SortedSetLengthAsync(
                            LruKey);

                continue;
            }

            await _database.KeyDeleteAsync(
                GetArticleKey(articleId));

            await _database.SortedSetRemoveAsync(
                LruKey,
                articleIdValue);

            count =
                await _database.SortedSetLengthAsync(
                    LruKey);
        }
    }


    private static string GetArticleKey(
        int articleId)
    {
        return $"comments:article:{articleId}";
    }
}