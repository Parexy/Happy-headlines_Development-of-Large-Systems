using ProfanityService.Models;

namespace ProfanityService.Data;

public interface IProfanityDbContextFactory
{
    ProfanityDbContext Create();
}