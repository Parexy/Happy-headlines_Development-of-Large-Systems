using Microsoft.AspNetCore.Mvc;
using PublisherService.DTOs;
using PublisherService.Services;

namespace PublisherService.Controllers;

[ApiController]
[Route("api/publishers")]
public class PublisherController : ControllerBase
{
    private readonly IPublisherService _publisherService;

    public PublisherController(
        IPublisherService publisherService)
    {
        _publisherService = publisherService;
    }

    [HttpPost]
    public async Task<ActionResult<PublishArticleDto>>
        PublishArticle(
            PublishArticleDto article)
    {
        var publishedArticle =
            await _publisherService.PublishArticle(article);

        return Ok(publishedArticle);
    }
}