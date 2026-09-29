using ArticleService.Data;
using ArticleService.Models;
using Microsoft.EntityFrameworkCore;

namespace ArticleService.Caching;

public sealed class ArticleCacheWarmupService
    : BackgroundService
{
    private readonly IArticleDbContextFactory _dbContextFactory;
    private readonly IArticleCache _cache;
    private readonly ILogger<ArticleCacheWarmupService> _logger;

    public ArticleCacheWarmupService(
        IArticleDbContextFactory dbContextFactory,
        IArticleCache cache,
        ILogger<ArticleCacheWarmupService> logger)
    {
        _dbContextFactory = dbContextFactory;
        _cache = cache;
        _logger = logger;
    }

    protected override async Task ExecuteAsync(
        CancellationToken stoppingToken)
    {
        while (!stoppingToken.IsCancellationRequested)
        {
            try
            {
                await RefreshCacheAsync(stoppingToken);
            }
            catch (Exception exception)
            {
                _logger.LogError(
                    exception,
                    "Failed to refresh ArticleCache.");
            }

            await Task.Delay(
                TimeSpan.FromMinutes(5),
                stoppingToken);
        }
    }

    private async Task RefreshCacheAsync(
        CancellationToken cancellationToken)
    {
        await using var db =
            _dbContextFactory.Create(ArticleRegion.Global);

        var cutoff = DateTime.UtcNow.AddDays(-14);

        var articles = await db.Articles
            .AsNoTracking()
            .Where(article =>
                article.PublishedAt >= cutoff)
            .ToListAsync(cancellationToken);

        foreach (var article in articles)
        {
            var expiresAt =
                article.PublishedAt.AddDays(14);

            var ttl = expiresAt - DateTime.UtcNow;

            if (ttl <= TimeSpan.Zero)
            {
                continue;
            }

            await _cache.SetAsync(
                article,
                ttl);
        }

        _logger.LogInformation(
            "ArticleCache refreshed with {ArticleCount} articles.",
            articles.Count);
    }
}