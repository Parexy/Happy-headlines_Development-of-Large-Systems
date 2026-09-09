using CommentService.Models;

namespace CommentService.Data;

public interface ICommentDbContextFactory
{
    CommentDbContext Create();
}