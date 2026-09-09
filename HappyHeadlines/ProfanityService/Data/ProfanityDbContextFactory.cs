using Microsoft.EntityFrameworkCore;

namespace ProfanityService.Data;

public class ProfanityDbContextFactory : IProfanityDbContextFactory
{
    private readonly IConfiguration _configuration;

    public ProfanityDbContextFactory(IConfiguration configuration)
    {
        _configuration = configuration;
    }

    public ProfanityDbContext Create()
    {
        var connectionString =
            _configuration.GetConnectionString("DefaultConnection");

        if (string.IsNullOrWhiteSpace(connectionString))
        {
            throw new InvalidOperationException(
                "No connection string configured for ProfanityDatabase.");
        }

        var options = new DbContextOptionsBuilder<ProfanityDbContext>()
            .UseNpgsql(connectionString)
            .Options;

        return new ProfanityDbContext(options);
    }
}