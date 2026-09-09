using CommentService.Data;
using CommentService.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace CommentService.Controllers
{
    [ApiController]
    [Route("api/comments")]
    public class CommentController : ControllerBase
    {

        private readonly ICommentDbContextFactory _dbContextFactory;

        public CommentController(ICommentDbContextFactory dbContextFactory)
        {
            _dbContextFactory = dbContextFactory;
        }

        // GET /api/comments
        // Get all comments
        [HttpGet]
        public async Task<ActionResult<IEnumerable<Comment>>> GetAll()
        {
            await using var db = _dbContextFactory.Create();

            var comments = await db.Comments
                .AsNoTracking()
                .ToListAsync();

            return Ok(comments);
        }


        // GET /api/comments/{id}
        // Get one comment
        [HttpGet("{id:int}")]
        public async Task<ActionResult<Comment>> Get(int id)
        {
            await using var db = _dbContextFactory.Create();

            var comment = await db.Comments
                .AsNoTracking()
                .FirstOrDefaultAsync(c => c.Id == id);

            if (comment == null)
            {
                return NotFound("Comment not found.");
            }

            return Ok(comment);
        }

        // GET /api/comments/article/{articleId}
        // Get all comments for an article
        [HttpGet("article/{articleId:int}")]
        public async Task<ActionResult<IEnumerable<Comment>>> GetByArticle(
            int articleId)
        {
            await using var db = _dbContextFactory.Create();

            var comments = await db.Comments
                .AsNoTracking()
                .Where(c => c.ArticleId == articleId)
                .OrderBy(c => c.CreatedAt)
                .ToListAsync();

            return Ok(comments);
        }

        // POST /api/comments
        // Add a new comment
        [HttpPost]
        public async Task<ActionResult<Comment>> AddComment(
            Comment request)
        {
            if (request.ArticleId <= 0)
            {
                return BadRequest("ArticleId must be greater than 0.");
            }

            if (string.IsNullOrWhiteSpace(request.Author))
            {
                return BadRequest("Author cannot be empty.");
            }

            if (string.IsNullOrWhiteSpace(request.Content))
            {
                return BadRequest("Comment cannot be empty.");
            }

            await using var db = _dbContextFactory.Create();

            var comment = new Comment
            {
                ArticleId = request.ArticleId,
                Author = request.Author,
                Content = request.Content,
                CreatedAt = DateTime.UtcNow,
                UpdatedAt = DateTime.UtcNow,
            };

            db.Comments.Add(comment);

            await db.SaveChangesAsync();

            return CreatedAtAction(
                nameof(Get),
                new { id = comment.Id },
                comment);
        }

        // PUT /api/comments/{id}
        // Update a comment
        [HttpPut("{id:int}")]
        public async Task<ActionResult<Comment>> UpdateComment(
            int id,
            Comment request)
        {
            if (request.ArticleId <= 0)
            {
                return BadRequest("ArticleId must be greater than 0.");
            }

            if (string.IsNullOrWhiteSpace(request.Author))
            {
                return BadRequest("Author cannot be empty.");
            }

            if (string.IsNullOrWhiteSpace(request.Content))
            {
                return BadRequest("Comment cannot be empty.");
            }

            await using var db = _dbContextFactory.Create();

            var comment = await db.Comments
                .FirstOrDefaultAsync(c => c.Id == id);

            if (comment == null)
            {
                return NotFound("Comment not found.");
            }

            comment.ArticleId = request.ArticleId;
            comment.Author = request.Author;
            comment.Content = request.Content;
            comment.UpdatedAt = DateTime.UtcNow;

            await db.SaveChangesAsync();

            return Ok(comment);
        }


         // DELETE /api/comments/{id}
        // Delete a comment
        [HttpDelete("{id:int}")]
        public async Task<IActionResult> DeleteComment(int id)
        {
            await using var db = _dbContextFactory.Create();

            var comment = await db.Comments
                .FirstOrDefaultAsync(c => c.Id == id);

            if (comment == null)
            {
                return NotFound("Comment not found.");
            }

            db.Comments.Remove(comment);

            await db.SaveChangesAsync();

            return NoContent();
        }
    }
}
