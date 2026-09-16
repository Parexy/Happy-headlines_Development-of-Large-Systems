using DraftService.Models;

namespace DraftService.Data;

public interface IDraftDbContextFactory
{
    DraftDbContext Create();
}