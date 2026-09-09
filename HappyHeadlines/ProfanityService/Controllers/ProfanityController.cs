using ProfanityService.Data;
using ProfanityService.DTOs;
using ProfanityService.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace ProfanityService.Controllers
{
    [ApiController]
    [Route("api/profanity")]
    public class ProfanityController : ControllerBase
    {
        private readonly IProfanityDbContextFactory _dbContextFactory;

        public ProfanityController(IProfanityDbContextFactory dbContextFactory)
        {
            _dbContextFactory = dbContextFactory;
        }

        // POST /api/profanity/check
        [HttpPost("check")]
        public async Task<ActionResult<CheckProfanityResponse>> Check(
            CheckProfanityRequest request)
        {
            await using var db = _dbContextFactory.Create();

            var words = request.Text
                .ToLower()
                .Split(
                    new[]
                    {
                    ' ',
                    '.',
                    ',',
                    '!',
                    '?',
                    ';',
                    ':',
                    '\n',
                    '\r'
                    },
                    StringSplitOptions.RemoveEmptyEntries);

            var containsProfanity = await db.Profanity
                .AnyAsync(p => words.Contains(p.Word.ToLower()));

            return Ok(new CheckProfanityResponse
            {
                ContainsProfanity = containsProfanity
            });
        }

        // POST /api/profanity
        // Add a new profanity word
        [HttpPost]
        public async Task<ActionResult<Profanity>> AddWord(
            Profanity request)
        {
            if (string.IsNullOrWhiteSpace(request.Word))
            {
                return BadRequest("Word cannot be empty.");
            }

            await using var db = _dbContextFactory.Create();

            var word = request.Word.Trim().ToLower();

            // Check if the word already exists
            var exists = await db.Profanity
                .AnyAsync(p => p.Word.ToLower() == word);

            if (exists)
            {
                return Conflict("This word already exists.");
            }

            var profanityWord = new Profanity
            {
                Word = word
            };

            db.Profanity.Add(profanityWord);

            await db.SaveChangesAsync();

            return CreatedAtAction(
                nameof(AddWord),
                new { id = profanityWord.Id },
                profanityWord);
        }


        // DELETE /api/profanity/{id}
        // Delete a profanity word
        [HttpDelete("{id:int}")]
        public async Task<IActionResult> DeleteWord(int id)
        {
            await using var db = _dbContextFactory.Create();

            var word = await db.Profanity
                .FirstOrDefaultAsync(p => p.Id == id);

            if (word == null)
            {
                return NotFound("Profanity word not found.");
            }

            db.Profanity.Remove(word);

            await db.SaveChangesAsync();

            return NoContent();
        }
    }
}
