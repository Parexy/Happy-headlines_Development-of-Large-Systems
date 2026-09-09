using System.Text.Json.Serialization;
using ArticleService.Data;

var builder = WebApplication.CreateBuilder(args);

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
});

app.MapControllers();

app.Run();