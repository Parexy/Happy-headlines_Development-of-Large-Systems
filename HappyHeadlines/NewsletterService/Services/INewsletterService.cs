using NewsletterService.DTOs;

namespace NewsletterService.Services;

public interface INewsletterService
{
    Task<NewsletterDto> CreateDailyNewsletter();
}