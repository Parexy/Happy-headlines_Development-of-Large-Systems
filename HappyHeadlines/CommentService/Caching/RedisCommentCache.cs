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

    public RedisCommentCache(
        IConnectionMultiplexer connectionMultiplexer,
        IOptions<CommentCacheOptions> options)
    {
        _database =
            connectionMultiplexer.GetDatabase();

        _maxArticles =
            options.Value.MaxArticles;
    }


    public async Task<IReadOnlyList<Comment>?> GetAsync(
        int articleId)
    {
        var key = GetArticleKey(articleId);

        var value =
            await _database.StringGetAsync(key);

        if (value.IsNull)
        {
            // Clean up a possible stale LRU entry.
            await _database.SortedSetRemoveAsync(
                LruKey,
                articleId);

            return null;
        }

        // Cache hit:
        // update the article's position in the LRU list.
        await TouchAsync(articleId);

        var comments =
            JsonSerializer.Deserialize<List<Comment>>(
                value.ToString());

        return comments ?? [];
    }


    public async Task SetAsync(
        int articleId,
        IReadOnlyList<Comment> comments)
    {
        var key = GetArticleKey(articleId);

        var json =
            JsonSerializer.Serialize(comments);

        await _database.StringSetAsync(
            key,
            json);

        // This article has just been accessed,
        // so move it to the most-recently-used position.
        await TouchAsync(articleId);

        // Ensure that only the 30 most recently
        // accessed articles remain in the cache.
        await EvictLeastRecentlyUsedAsync();
    }


    public async Task RemoveAsync(
        int articleId)
    {
        var key = GetArticleKey(articleId);

        await _database.KeyDeleteAsync(key);

        await _database.SortedSetRemoveAsync(
            LruKey,
            articleId);
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