namespace Messaging;

public static class ArticleMessaging
{
    public const string Exchange =
        "articles.published";

    public const string ArticleServiceQueue =
        "article-service.articles-published";

    public const string NewsletterServiceQueue =
        "newsletter-service.articles-published";
}