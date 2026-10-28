namespace SubscriberService.Data;

public interface ISubscriberDbContextFactory
{
    SubscriberDbContext Create();
}