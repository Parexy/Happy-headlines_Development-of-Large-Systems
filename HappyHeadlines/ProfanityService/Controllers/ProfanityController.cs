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
        public async Task<ActionResult<ProfanityCheckResponse>> Check(
            ProfanityCheckRequest request)
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

            return Ok(new ProfanityCheckResponse
            {
                ContainsProfanity = containsProfanity
            });
        }

    }
}
