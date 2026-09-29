namespace CommentService.Caching;

public sealed class CommentCacheOptions
{
    public const string SectionName = "CommentCache";

    public string ConnectionString { get; set; } =
        "comment-cache:6379";

    public int MaxArticles { get; set; } = 30;
}