namespace ArticleService.Core.Entities
{
    public class Article
    {
        public int Id { get; set; }

        public string Title { get; set; } = string.Empty;

        public string Content { get; set; } = string.Empty;

        public string Author { get; set; } = string.Empty;

        public DateTime PublishedAt { get; set; }
    }
}
