using System.Diagnostics.Metrics;

namespace Observability;

public static class CacheMetrics
{
    public const string MeterName = "HappyHeadlines.Cache";

    private static readonly Meter Meter =
        new(MeterName);

    private static readonly Counter<long> Hits =
        Meter.CreateCounter<long>(
            "happyheadlines.cache.hits");

    private static readonly Counter<long> Misses =
        Meter.CreateCounter<long>(
            "happyheadlines.cache.misses");

    public static void RecordHit(string cache)
    {
        Hits.Add(
            1,
            new KeyValuePair<string, object?>(
                "cache",
                cache));
    }

    public static void RecordMiss(string cache)
    {
        Misses.Add(
            1,
            new KeyValuePair<string, object?>(
                "cache",
                cache));
    }
}