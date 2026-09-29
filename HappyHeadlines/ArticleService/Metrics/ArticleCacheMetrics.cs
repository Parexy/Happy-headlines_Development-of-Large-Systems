using Prometheus;

namespace ArticleService.Metrics;

public static class ArticleCacheMetrics
{
    public static readonly Counter Hits =
        Prometheus.Metrics.CreateCounter(
            "cache_article_hits_total",
            "Number of article cache hits");

    public static readonly Counter Misses =
        Prometheus.Metrics.CreateCounter(
            "cache_article_misses_total",
            "Number of article cache misses");
}