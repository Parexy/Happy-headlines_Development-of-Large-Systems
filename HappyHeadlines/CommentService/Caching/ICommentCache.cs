using CommentService.Models;

namespace CommentService.Caching;

public interface ICommentCache
{
    Task<IReadOnlyList<Comment>?> GetAsync(
        int articleId);

    Task SetAsync(
        int articleId,
        IReadOnlyList<Comment> comments);

    Task RemoveAsync(
        int articleId);
}