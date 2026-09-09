using CommentService.Data;
using CommentService.DTOs;
using CommentService.Models;
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
        [HttpGet]
        public async Task<ActionResult<IEnumerable<Comment>>> GetAll()
        {
            var comments = await _db.Comments
                .AsNoTracking()
                .ToListAsync();

            return Ok(comments);
        }


        // GET /api/comments/{id}
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


        // POST /api/comments
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
            catch (Exception ex) when (
                ex is HttpRequestException ||
                ex is BrokenCircuitException)
            {
                return StatusCode(
                    StatusCodes.Status503ServiceUnavailable,
                    "Profanity service is currently unavailable.");
            }

            var comment = new Comment
            {
                ArticleId = request.ArticleId,
                Author = request.Author,
                Content = request.Content,
                CreatedAt = DateTime.UtcNow,
                UpdatedAt = null
            };

            _db.Comments.Add(comment);

            await _db.SaveChangesAsync(cancellationToken);

            return CreatedAtAction(
                nameof(Get),
                new { id = comment.Id },
                comment);
        }


        // DELETE /api/comments/{id}
        [HttpDelete("{id:int}")]
        public async Task<IActionResult> DeleteComment(
            int id,
            CancellationToken cancellationToken)
        {
            var comment = await _db.Comments
                .FirstOrDefaultAsync(
                    c => c.Id == id,
                    cancellationToken);

            if (comment == null)
            {
                return NotFound("Comment not found.");
            }

            _db.Comments.Remove(comment);

            await _db.SaveChangesAsync(cancellationToken);

            return NoContent();
        }
    }
}