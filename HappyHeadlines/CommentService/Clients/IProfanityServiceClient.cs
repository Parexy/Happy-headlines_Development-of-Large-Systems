public interface IProfanityServiceClient
{
    Task<bool> ContainsProfanityAsync(
        string content,
        CancellationToken cancellationToken = default);
}