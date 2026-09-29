using Prometheus;

namespace CommentService.Metrics;

public sealed class CommentCacheMetrics
{
    public Counter Hits { get; }
    public Counter Misses { get; }

    public CommentCacheMetrics()
    {
        Hits = Prometheus.Metrics.CreateCounter(
            "cache_comment_hits_total",
            "Number of comment cache hits");

        Misses = Prometheus.Metrics.CreateCounter(
            "cache_comment_misses_total",
            "Number of comment cache misses");
    }
}