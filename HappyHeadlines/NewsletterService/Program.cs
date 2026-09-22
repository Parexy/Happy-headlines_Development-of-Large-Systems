using Messaging.RabbitMq;
using NewsletterService.Messaging;
using NewsletterService.Services;
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

builder.Services.AddHttpClient<
    INewsletterService,
    NewsletterServiceImpl>(
        client =>
        {
            client.BaseAddress = new Uri(
                builder.Configuration["ArticleService:BaseUrl"]
                ?? "http://article-load-balancer");
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