using Microsoft.EntityFrameworkCore;
using SubscriberService.Data;

namespace SubscriberService.Middleware;

public class ReleaseToggleMiddleware
{
    private readonly RequestDelegate _next;
    private readonly ILogger<ReleaseToggleMiddleware> _logger;

    public ReleaseToggleMiddleware(
        RequestDelegate next,
        ILogger<ReleaseToggleMiddleware> logger)
    {
        _next = next;
        _logger = logger;
    }

    public async Task InvokeAsync(
        HttpContext context,
        SubscriberDbContext db)
    {
        // Only apply this toggle to subscriber endpoints.
        if (!context.Request.Path.StartsWithSegments(
                "/api/subscribers",
                StringComparison.OrdinalIgnoreCase))
        {
            await _next(context);
            return;
        }

        bool enabled;

        try
        {
            // Query the current value for every request.
            enabled = await db.ReleaseToggles
                .AsNoTracking()
                .Where(x => x.Name == "SubscriberService")
                .Select(x => x.Enabled)
                .SingleOrDefaultAsync(context.RequestAborted);
        }
        catch (Exception ex)
            when (!context.RequestAborted.IsCancellationRequested)
        {
            _logger.LogError(
                ex,
                "Unable to read SubscriberService release toggle");

            // Fail closed when the database is unavailable.
            context.Response.StatusCode =
                StatusCodes.Status503ServiceUnavailable;

            return;
        }

        if (!enabled)
        {
            // Hide the subscriber functionality.
            context.Response.StatusCode =
                StatusCodes.Status404NotFound;

            return;
        }

        // Feature enabled: execute the controller.
        await _next(context);
    }
}
