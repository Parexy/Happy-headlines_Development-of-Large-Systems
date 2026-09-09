using Polly;
using Polly.Registry;
using CommentService.Models;
using System.Net.Http.Json;

public class ProfanityServiceClient : IProfanityServiceClient
{
    private readonly HttpClient _httpClient;
    private readonly ResiliencePipeline<HttpResponseMessage> _pipeline;

    public ProfanityServiceClient(
        HttpClient httpClient,
        ResiliencePipelineProvider<string> pipelineProvider)
    {
        _httpClient = httpClient;

        _pipeline = pipelineProvider
            .GetPipeline<HttpResponseMessage>(
                "profanity-circuit-breaker");
    }

    public async Task<bool> ContainsProfanityAsync(
        string content,
        CancellationToken cancellationToken = default)
    {
        var response = await _pipeline.ExecuteAsync(
            async token =>
            {
                return await _httpClient.PostAsJsonAsync(
                    "/api/profanity/check",
                    new ProfanityCheckRequest
                    {
                        Text = content
                    },
                    token);
            },
            cancellationToken);

        response.EnsureSuccessStatusCode();

        var result = await response.Content
            .ReadFromJsonAsync<ProfanityCheckResponse>(
                cancellationToken: cancellationToken);

        return result?.ContainsProfanity ?? false;
    }
}