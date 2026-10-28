using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using SubscriberService.Data;
using SubscriberService.Models;

namespace SubscriberService.Controllers;

[ApiController]
[Route("api/subscribers")]
public sealed class SubscribersController : ControllerBase
{
    private readonly ISubscriberDbContextFactory _dbContextFactory;

    public SubscribersController(
        ISubscriberDbContextFactory dbContextFactory)
    {
        _dbContextFactory = dbContextFactory;
    }

    [HttpPost]
    public async Task<IActionResult> Create(
        [FromBody] CreateSubscriberRequest request,
        CancellationToken cancellationToken)
    {
        await using var dbContext = _dbContextFactory.Create();

        var email = request.Email.Trim();

        var subscriberExists = await dbContext.Subscribers
            .AnyAsync(s => s.Email == email, cancellationToken);

        if (subscriberExists)
        {
            return Conflict(new
            {
                error = "This email address is already subscribed."
            });
        }

        var subscriber = new Subscriber
        {
            Email = email
        };

        dbContext.Subscribers.Add(subscriber);

        await dbContext.SaveChangesAsync(cancellationToken);

        return Created(
            $"/api/subscribers/{subscriber.Id}",
            subscriber);
    }

    [HttpDelete]
    public async Task<IActionResult> Remove(
        [FromBody] RemoveSubscriberRequest request,
        CancellationToken cancellationToken)
    {
        await using var dbContext = _dbContextFactory.Create();

        var subscriber = await dbContext.Subscribers
            .FirstOrDefaultAsync(
                s => s.Id == request.Id,
                cancellationToken);

        if (subscriber is null)
        {
            return NotFound();
        }

        dbContext.Subscribers.Remove(subscriber);

        await dbContext.SaveChangesAsync(cancellationToken);

        return NoContent();
    }

    [HttpGet]
    public async Task<IActionResult> GetAll(
        CancellationToken cancellationToken)
    {
        await using var dbContext = _dbContextFactory.Create();

        var subscribers = await dbContext.Subscribers
            .AsNoTracking()
            .Select(s => new
            {
                s.Id,
                s.Email,
                s.CreatedAt
            })
            .ToListAsync(cancellationToken);

        return Ok(subscribers);
    }

}
