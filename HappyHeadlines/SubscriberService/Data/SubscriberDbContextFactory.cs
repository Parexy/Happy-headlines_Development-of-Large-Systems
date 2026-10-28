using Microsoft.EntityFrameworkCore;

namespace SubscriberService.Data;

public class SubscriberDbContextFactory
    : ISubscriberDbContextFactory
{
    private readonly IConfiguration _configuration;

    public SubscriberDbContextFactory(
        IConfiguration configuration)
    {
        _configuration = configuration;
    }

    public SubscriberDbContext Create()
    {
        var connectionString =
            _configuration.GetConnectionString(
                "DefaultConnection");

        if (string.IsNullOrWhiteSpace(connectionString))
        {
            throw new InvalidOperationException(
                "No database configured for SubscriberService.");
        }

        var options =
            new DbContextOptionsBuilder<SubscriberDbContext>()
                .UseNpgsql(connectionString)
                .Options;

        return new SubscriberDbContext(options);
    }
}
