using ArticleService.Data;
using ArticleService.DTOs;
using ArticleService.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace ArticleService.Controllers;

[ApiController]
[Route("api/articles")]
public class ArticlesController : ControllerBase
{
    private readonly IArticleDbContextFactory _dbContextFactory;

    public ArticlesController(IArticleDbContextFactory dbContextFactory)
    {
        _dbContextFactory = dbContextFactory;
    }


    // CREATE
    // POST /api/articles
    [HttpPost]
    public async Task<ActionResult<ArticleResponse>> Create(
        CreateArticleRequest request)
    {
        await using var db =
            _dbContextFactory.Create(request.Region);

        var now = DateTime.UtcNow;

        var article = new Article
        {
            Title = request.Title,
            Content = request.Content,
            Author = request.Author,

            CreatedAt = now,
            PublishedAt = now,
            UpdatedAt = now
        };

        db.Articles.Add(article);

        await db.SaveChangesAsync();

        var response = ToResponse(
            article,
            request.Region);

        return CreatedAtAction(
            nameof(Get),
            new
            {
                region = request.Region,
                id = article.Id
            },
            response);
    }


    // READ
    // GET /api/articles/Europe/1
    [HttpGet("{region}/{id:int}")]
    public async Task<ActionResult<ArticleResponse>> Get(
        ArticleRegion region,
        int id)
    {
        await using var db =
            _dbContextFactory.Create(region);

        var article = await db.Articles
            .AsNoTracking()
            .FirstOrDefaultAsync(a => a.Id == id);

        if (article == null)
        {
            return NotFound();
        }

        return Ok(ToResponse(article, region));
    }


    // UPDATE
    // PUT /api/articles/Europe/1
    [HttpPut("{region}/{id:int}")]
    public async Task<ActionResult<ArticleResponse>> Update(
        ArticleRegion region,
        int id,
        UpdateArticleRequest request)
    {
        await using var db =
            _dbContextFactory.Create(region);

        var article = await db.Articles
            .FirstOrDefaultAsync(a => a.Id == id);

        if (article == null)
        {
            return NotFound();
        }

        article.Title = request.Title;
        article.Content = request.Content;
        article.Author = request.Author;

        article.UpdatedAt = DateTime.UtcNow;

        await db.SaveChangesAsync();

        return Ok(ToResponse(article, region));
    }


    // DELETE
    // DELETE /api/articles/Europe/1
    [HttpDelete("{region}/{id:int}")]
    public async Task<IActionResult> Delete(
        ArticleRegion region,
        int id)
    {
        await using var db =
            _dbContextFactory.Create(region);

        var article = await db.Articles
            .FirstOrDefaultAsync(a => a.Id == id);

        if (article == null)
        {
            return NotFound();
        }

        db.Articles.Remove(article);

        await db.SaveChangesAsync();

        return NoContent();
    }


    private static ArticleResponse ToResponse(
        Article article,
        ArticleRegion region)
    {
        return new ArticleResponse
        {
            Id = article.Id,
            Title = article.Title,
            Content = article.Content,
            Author = article.Author,
            Region = region,

            CreatedAt = article.CreatedAt,
            PublishedAt = article.PublishedAt,
            UpdatedAt = article.UpdatedAt
        };
    }
}