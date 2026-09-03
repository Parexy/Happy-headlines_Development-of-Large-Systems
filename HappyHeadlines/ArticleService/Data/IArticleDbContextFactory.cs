using ArticleService.Models;

namespace ArticleService.Data;

public interface IArticleDbContextFactory
{
    ArticleDbContext Create(ArticleRegion region);
}