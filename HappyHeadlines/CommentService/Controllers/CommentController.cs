using CommentService.Data;
using CommentService.Models;
using CommentService.DTOs;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Polly.CircuitBreaker;

namespace CommentService.Controllers
{
    [ApiController]
    [Route("api/comments")]
    public class CommentController : ControllerBase
    {
        private readonly CommentDbContext _db;
        private readonly IProfanityServiceClient _profanityServiceClient;

        public CommentController(
            CommentDbContext db,
            IProfanityServiceClient profanityServiceClient)
        {
            _db = db;
            _profanityServiceClient = profanityServiceClient;
        }

        // GET /api/comments
        // Get all comments
        [HttpGet]
        public async Task<ActionResult<IEnumerable<Comment>>> GetAll()
        {
            var comments = await _db.Comments
                .AsNoTracking()
                .ToListAsync();

            return Ok(comments);
        }


        // GET /api/comments/{id}
        // Get one comment
        [HttpGet("{id:int}")]
        public async Task<ActionResult<Comment>> Get(int id)
        {
            var comment = await _db.Comments
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
        public async Task<ActionResult<IEnumerable<Comment>>> GetByArticleId(
            int articleId)
        {
            var comments = await _db.Comments
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
            CreateComment request,
            CancellationToken cancellationToken)
        {
            if (string.IsNullOrWhiteSpace(request.Content))
            {
                return BadRequest("Comment cannot be empty.");
            }

            try
            {
                var containsProfanity =
                    await _profanityServiceClient.ContainsProfanityAsync(
                        request.Content,
                        cancellationToken);

                if (containsProfanity)
                {
                    return BadRequest(
                        "Comment contains profanity.");
                }
            }
            catch (BrokenCircuitException)
            {
                return StatusCode(
                    StatusCodes.Status503ServiceUnavailable,
                    "Profanity service is currently unavailable.");
            }
            catch (HttpRequestException)
            {
                return StatusCode(
                    StatusCodes.Status503ServiceUnavailable,
                    "Profanity service could not be reached.");
            }

            var comment = new Comment
            {
                Author = request.Author,
                Content = request.Content,
                ArticleId = request.ArticleId,
                CreatedAt = DateTime.UtcNow,
                UpdatedAt = DateTime.UtcNow
            };

            _db.Comments.Add(comment);

            await _db.SaveChangesAsync(cancellationToken);

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
            if (string.IsNullOrWhiteSpace(request.Content))
            {
                return BadRequest("Comment cannot be empty.");
            }

            var comment = await _db.Comments
                .FirstOrDefaultAsync(c => c.Id == id);

            if (comment == null)
            {
                return NotFound("Comment not found.");
            }

            comment.Content = request.Content;
            comment.Author = request.Author;
            comment.ArticleId = request.ArticleId;
            comment.UpdatedAt = DateTime.UtcNow;

            await _db.SaveChangesAsync();

            return Ok(comment);
        }



        // DELETE /api/comments/{id}
        // Delete a comment
        [HttpDelete("{id:int}")]
        public async Task<IActionResult> DeleteComment(int id)
        {
            var comment = await _db.Comments
                .FirstOrDefaultAsync(c => c.Id == id);

            if (comment == null)
            {
                return NotFound("Comment not found.");
            }

            _db.Comments.Remove(comment);

            await _db.SaveChangesAsync();

            return NoContent();
        }
    }
}
