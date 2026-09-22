using Microsoft.AspNetCore.Mvc;
using NewsletterService.DTOs;
using NewsletterService.Services;

namespace NewsletterService.Controllers;

[ApiController]
[Route("api/newsletters")]
public class NewsletterController : ControllerBase
{
    private readonly INewsletterService _newsletterService;

    public NewsletterController(INewsletterService newsletterService)
    {
        _newsletterService = newsletterService;
    }

    [HttpPost("daily")]
    public async Task<ActionResult<NewsletterDto>> CreateDailyNewsletter()
    {
        var newsletter =
            await _newsletterService.CreateDailyNewsletter();

        return Ok(newsletter);
    }
}