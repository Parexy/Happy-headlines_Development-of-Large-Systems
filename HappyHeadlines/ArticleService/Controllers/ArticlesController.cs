using Microsoft.AspNetCore.Mvc;
using ArticleService.Core.Entities;

namespace ArticleService.Controllers;

[ApiController]
[Route("[controller]")]
public class ArticlesController : ControllerBase
{
    private static readonly List<Article> Articles = new();

    // CREATE
    // POST: /Articles
    [HttpPost(Name = "CreateArticle")]
    public ActionResult<Article> Create(Article article)
    {
        article.Id = Articles.Count + 1;
        article.PublishedAt = DateTime.UtcNow;

        Articles.Add(article);

        return CreatedAtAction(
            nameof(Get),
            new { id = article.Id },
            article);
    }


    // READ
    // GET: /Articles/1
    [HttpGet("{id}", Name = "GetArticle")]
    public ActionResult<Article> Get(int id)
    {
        var article = Articles.FirstOrDefault(a => a.Id == id);

        if (article == null)
        {
            return NotFound();
        }

        return Ok(article);
    }


    // UPDATE
    // PUT: /Articles/1
    [HttpPut("{id}", Name = "UpdateArticle")]
    public IActionResult Update(int id, Article updatedArticle)
    {
        var article = Articles.FirstOrDefault(a => a.Id == id);

        if (article == null)
        {
            return NotFound();
        }

        article.Title = updatedArticle.Title;
        article.Content = updatedArticle.Content;

        return Ok(article);
    }


    // DELETE
    // DELETE: /Articles/1
    [HttpDelete("{id}", Name = "DeleteArticle")]
    public IActionResult Delete(int id)
    {
        var article = Articles.FirstOrDefault(a => a.Id == id);

        if (article == null)
        {
            return NotFound();
        }

        Articles.Remove(article);

        return NoContent();
    }
}



