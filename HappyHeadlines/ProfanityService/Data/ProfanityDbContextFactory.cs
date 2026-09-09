using ProfanityService.Models;
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
            _configuration.GetConnectionString();

        if (string.IsNullOrWhiteSpace(connectionString))
        {
            throw new InvalidOperationException(
                $"No database configured for region '{region}'.");
        }

        var options = new DbContextOptionsBuilder<ProfanityDbContext>()
            .UseNpgsql(connectionString)
            .Options;

        return new ProfanityDbContext(options);
    }
}