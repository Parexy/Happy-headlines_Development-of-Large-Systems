using System.Text.Json.Serialization;
using ProfanityService.Data;
using Observability;

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
    IProfanityDbContextFactory,
    ProfanityDbContextFactory>();

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