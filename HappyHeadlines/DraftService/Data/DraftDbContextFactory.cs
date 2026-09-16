using Microsoft.EntityFrameworkCore;

namespace DraftService.Data;

public class DraftDbContextFactory : IDraftDbContextFactory
{
    private readonly IConfiguration _configuration;

    public DraftDbContextFactory(IConfiguration configuration)
    {
        _configuration = configuration;
    }

    public DraftDbContext Create()
    {
        var connectionString =
            _configuration.GetConnectionString("DefaultConnection");

        if (string.IsNullOrWhiteSpace(connectionString))
        {
            throw new InvalidOperationException(
                "No connection string configured for DraftDatabase.");
        }

        var options = new DbContextOptionsBuilder<DraftDbContext>()
            .UseNpgsql(connectionString)
            .Options;

        return new DraftDbContext(options);
    }
}