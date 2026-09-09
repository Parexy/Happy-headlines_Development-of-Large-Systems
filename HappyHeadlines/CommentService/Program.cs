using Polly;
using Polly.CircuitBreaker;
using Polly.Registry;
using CommentService.Data;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddControllers();

builder.Services.AddSingleton<
    ICommentDbContextFactory,
    CommentDbContextFactory>();

builder.Services.AddHttpClient<IProfanityServiceClient, ProfanityServiceClient>(
    client =>
    {
        var baseUrl = builder.Configuration["ProfanityService:BaseUrl"]
            ?? throw new InvalidOperationException(
                "ProfanityService:BaseUrl is missing");

        client.BaseAddress = new Uri(baseUrl);
    });


builder.Services.AddResiliencePipeline<string, HttpResponseMessage>(
    "profanity-circuit-breaker",
    pipeline =>
    {
        pipeline.AddCircuitBreaker(
            new CircuitBreakerStrategyOptions<HttpResponseMessage>
            {
                ShouldHandle = new PredicateBuilder<HttpResponseMessage>()
                    .Handle<HttpRequestException>()
                    .HandleResult(response =>
                        (int)response.StatusCode >= 500),

                FailureRatio = 0.5,
                MinimumThroughput = 4,
                SamplingDuration = TimeSpan.FromSeconds(10),
                BreakDuration = TimeSpan.FromSeconds(30)
            });
    });

builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

var app = builder.Build();

app.UseSwagger();
app.UseSwaggerUI();

app.MapControllers();

app.Run();