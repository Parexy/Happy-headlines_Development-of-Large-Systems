using System.Text.Json.Serialization;
using ArticleService.Data;
using Observability;

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
});

app.MapControllers();

app.Run();