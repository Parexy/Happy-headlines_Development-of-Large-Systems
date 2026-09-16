using DraftService.Data;
using DraftService.DTOs;
using DraftService.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace DraftService.Controllers
{
    [ApiController]
    [Route("api/drafts")]
    public class DraftController : ControllerBase
    {
        private readonly IDraftDbContextFactory _dbContextFactory;

        public DraftController(IDraftDbContextFactory dbContextFactory)
        {
            _dbContextFactory = dbContextFactory;
        }

        // CREATE
        // POST /api/drafts
        [HttpPost]
        public async Task<ActionResult<Draft>> Create(
            CreateDraftRequest request)
        {
            await using var db =
                _dbContextFactory.Create();

            var now = DateTime.UtcNow;

            var draft = new Draft
            {
                Title = request.Title,
                Content = request.Content,
                Author = request.Author,

                CreatedAt = now,
                UpdatedAt = now
            };

            db.Drafts.Add(draft);

            await db.SaveChangesAsync();

            return CreatedAtAction(
                nameof(Get),
                new
                {
                    id = draft.Id
                },
                draft);
        }


        // READ
        // GET /api/drafts/1
        [HttpGet("{id:int}")]
        public async Task<ActionResult<Draft>> Get(
            int id)
        {
            await using var db =
                _dbContextFactory.Create();

            var draft = await db.Drafts
                .AsNoTracking()
                .FirstOrDefaultAsync(d => d.Id == id);

            if (draft == null)
            {
                return NotFound();
            }

            return Ok(draft);
        }


        // UPDATE
        // PUT /api/drafts/1
        [HttpPut("{id:int}")]
        public async Task<ActionResult<Draft>> Update(
            int id,
            UpdateDraftRequest request)
        {
            await using var db =
                _dbContextFactory.Create();

            var draft = await db.Drafts
                .FirstOrDefaultAsync(d => d.Id == id);

            if (draft == null)
            {
                return NotFound();
            }

            draft.Title = request.Title;
            draft.Content = request.Content;
            draft.Author = request.Author;

            draft.UpdatedAt = DateTime.UtcNow;

            await db.SaveChangesAsync();

            return Ok(draft);
        }


        // DELETE
        // DELETE /api/drafts/1
        [HttpDelete("{id:int}")]
        public async Task<IActionResult> Delete(
            int id)
        {
            await using var db =
                _dbContextFactory.Create();

            var draft = await db.Drafts
                .FirstOrDefaultAsync(d => d.Id == id);

            if (draft == null)
            {
                return NotFound();
            }

            db.Drafts.Remove(draft);

            await db.SaveChangesAsync();

            return NoContent();
        }

    }

}