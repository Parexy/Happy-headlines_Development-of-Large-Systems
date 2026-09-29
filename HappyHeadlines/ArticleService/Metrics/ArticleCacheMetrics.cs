using Prometheus;

namespace ArticleService.Metrics;

public sealed class ArticleCacheMetrics
{
    public Counter Hits { get; }
    public Counter Misses { get; }

    public ArticleCacheMetrics()
    {
        Hits = Prometheus.Metrics.CreateCounter(
            "cache_article_hits_total",
            "Number of article cache hits");

        Misses = Prometheus.Metrics.CreateCounter(
            "cache_article_misses_total",
            "Number of article cache misses");
    }
}