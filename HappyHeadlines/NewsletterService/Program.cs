using Messaging.RabbitMq;
using NewsletterService.Messaging;
using NewsletterService.Services;
using NewsletterService.Clients;
using Observability;
using System.Text.Json.Serialization;

var builder = WebApplication.CreateBuilder(args);

// Shared logging + tracing.
// Logs -> Seq
// Traces -> Zipkin
builder.AddObservability();

builder.Services
    .AddControllers()
    .AddJsonOptions(options =>
    {
        options.JsonSerializerOptions.Converters.Add(
            new JsonStringEnumConverter());
    });

builder.Services.AddSingleton<RabbitMqConnection>();
builder.Services.AddSingleton<RabbitMqConsumer>();

builder.Services.AddHostedService<ArticlePublishedConsumer>();
builder.Services.AddHostedService<SubscriberSubscribedConsumer>();

builder.Services.AddHttpClient<
    INewsletterService,
    NewsletterServiceImpl>(
        client =>
        {
            client.BaseAddress = new Uri(
                builder.Configuration["ArticleService:BaseUrl"]
                ?? "http://article-load-balancer");
        });

builder.Services
    .AddHttpClient<
        ISubscriberServiceClient,
        SubscriberServiceClient>(client =>
        {
            client.BaseAddress = new Uri(
                builder.Configuration["SubscriberService:BaseUrl"]
                ?? "http://subscriber-service:8080");
        })
    .AddStandardResilienceHandler(options =>
    {
        // Total request timeout.
        options.TotalRequestTimeout.Timeout =
            TimeSpan.FromSeconds(3);

        // Timeout for each attempt.
        options.AttemptTimeout.Timeout =
            TimeSpan.FromSeconds(2);

        // Disable retries.
        options.Retry.MaxRetryAttempts = 0;

        // Circuit breaker.
        options.CircuitBreaker.FailureRatio = 0.5;

        options.CircuitBreaker.MinimumThroughput = 4;

        options.CircuitBreaker.SamplingDuration =
            TimeSpan.FromSeconds(10);

        options.CircuitBreaker.BreakDuration =
            TimeSpan.FromSeconds(30);
    });

builder.Services.AddAuthorization();

builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

var app = builder.Build();

// Serilog request logging.
app.UseObservability();

app.UseSwagger();
app.UseSwaggerUI();

app.UseAuthorization();

app.MapControllers();

app.Run();