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
        private readonly CommentDbContext _db;

        public CommentController(CommentDbContext db)
        {
            _db = db;
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


        // POST /api/comments
        // Add a new comment
        [HttpPost]
        public async Task<ActionResult<Comment>> AddComment(
            Comment request)
        {
            if (string.IsNullOrWhiteSpace(request.Text))
            {
                return BadRequest("Comment cannot be empty.");
            }

            var comment = new Comment
            {
                Author = request.Author,
                Text = request.Text
            };

            _db.Comments.Add(comment);

            await _db.SaveChangesAsync();

            return CreatedAtAction(
                nameof(Get),
                new { id = comment.Id },
                comment);
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
