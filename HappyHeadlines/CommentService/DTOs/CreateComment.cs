namespace CommentService.DTOs
{
    public class CreateComment
    {
        public string Content { get; set; } = string.Empty;

        public string Author { get; set; } = string.Empty;
        public int ArticleId { get; set; }

        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

        public DateTime UpdatedAt { get; set; } = DateTime.UtcNow;

    }
}
