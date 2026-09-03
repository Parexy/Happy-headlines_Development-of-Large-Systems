using ArticleService.Models;
using Microsoft.EntityFrameworkCore;

namespace ArticleService.Data;

public class ArticleDbContextFactory : IArticleDbContextFactory
{
    private readonly IConfiguration _configuration;

    public ArticleDbContextFactory(IConfiguration configuration)
    {
        _configuration = configuration;
    }

    public ArticleDbContext Create(ArticleRegion region)
    {
        var connectionString =
            _configuration.GetConnectionString(region.ToString());

        if (string.IsNullOrWhiteSpace(connectionString))
        {
            throw new InvalidOperationException(
                $"No database configured for region '{region}'.");
        }

        var options = new DbContextOptionsBuilder<ArticleDbContext>()
            .UseNpgsql(connectionString)
            .Options;

        return new ArticleDbContext(options);
    }
}