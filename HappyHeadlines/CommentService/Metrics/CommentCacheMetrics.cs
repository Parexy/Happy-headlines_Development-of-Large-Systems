using Prometheus;

namespace CommentService.Metrics;

public static class CommentCacheMetrics
{
    public static readonly Counter Hits =
        Prometheus.Metrics.CreateCounter(
            "cache_comment_hits_total",
            "Number of comment cache hits");

    public static readonly Counter Misses =
        Prometheus.Metrics.CreateCounter(
            "cache_comment_misses_total",
            "Number of comment cache misses");
}