using Microsoft.EntityFrameworkCore;

namespace CommentService.Data;

public class CommentDbContextFactory : ICommentDbContextFactory
{
    private readonly IConfiguration _configuration;

    public CommentDbContextFactory(IConfiguration configuration)
    {
        _configuration = configuration;
    }

    public CommentDbContext Create()
    {
        var connectionString =
            _configuration.GetConnectionString("DefaultConnection");

        if (string.IsNullOrWhiteSpace(connectionString))
        {
            throw new InvalidOperationException(
                "No connection string configured for CommentDatabase.");
        }

        var options = new DbContextOptionsBuilder<CommentDbContext>()
            .UseNpgsql(connectionString)
            .Options;

        return new CommentDbContext(options);
    }
}