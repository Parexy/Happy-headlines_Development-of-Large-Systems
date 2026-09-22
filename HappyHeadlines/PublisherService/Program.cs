using Observability;
using PublisherService.Messaging;
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

builder.Services.AddSingleton<
    IArticlePublisher,
    RabbitMqArticlePublisher>();

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