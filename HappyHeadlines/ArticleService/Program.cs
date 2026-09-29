using System.Text.Json.Serialization;
using ArticleService.Data;
using ArticleService.Messaging;
using ArticleService.Metrics;
using Messaging.RabbitMq;
using Observability;
using Prometheus;
using ArticleService.Caching;
using StackExchange.Redis;

var builder = WebApplication.CreateBuilder(args);

// Shared logging + tracing configuration.
// Sends logs to Seq and traces to Zipkin.
builder.AddObservability();

builder.Services
    .AddControllers()
    .AddJsonOptions(options =>
    {
        options.JsonSerializerOptions.Converters.Add(
            new JsonStringEnumConverter());
    });

builder.Services.AddSingleton<
    IArticleDbContextFactory,
    ArticleDbContextFactory>();

builder.Services.AddSingleton<RabbitMqConnection>();
builder.Services.AddSingleton<RabbitMqConsumer>();

builder.Services.AddHostedService<
    ArticlePublishedConsumer>();

builder.Services.AddSingleton<IConnectionMultiplexer>(_ =>
{
    var connectionString =
        builder.Configuration[
            "ArticleCache:ConnectionString"]
        ?? throw new InvalidOperationException(
            "ArticleCache:ConnectionString is missing.");

    var options =
        ConfigurationOptions.Parse(connectionString);

    // ArticleService must still start when Redis
    // is unavailable.
    options.AbortOnConnectFail = false;

    options.ConnectRetry = 3;
    options.ConnectTimeout = 3000;

    return ConnectionMultiplexer.Connect(options);
});

builder.Services.AddSingleton<
    IArticleCache,
    RedisArticleCache>();
builder.Services.AddSingleton<ArticleCacheMetrics>();

builder.Services.AddHostedService<
    ArticleCacheWarmupService>();

builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

var app = builder.Build();

// Shared Serilog request logging.
app.UseObservability();

app.UseSwagger();

app.UseSwaggerUI(options =>
{
    options.SwaggerEndpoint(
        "/openapi/article.json",
        "Article Service");

    options.SwaggerEndpoint(
        "/openapi/comment.json",
        "Comment Service");

    options.SwaggerEndpoint(
        "/openapi/profanity.json",
        "Profanity Service");

    options.SwaggerEndpoint(
        "/openapi/draft.json",
        "Draft Service");

    options.SwaggerEndpoint(
        "/openapi/newsletter.json",
        "Newsletter Service");

    options.SwaggerEndpoint(
        "/openapi/publisher.json",
        "Publisher Service");
});

// Prometheus metrics endpoint.
app.UseHttpMetrics();
app.MapMetrics();

app.MapControllers();

app.Run();