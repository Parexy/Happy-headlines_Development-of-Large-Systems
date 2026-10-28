
using Microsoft.EntityFrameworkCore;
using SubscriberService.Data;
using SubscriberService.Middleware;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddControllers();

builder.Services.AddOpenApi();

builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

// Subscriber database.
builder.Services.AddDbContext<SubscriberDbContext>(options =>
{
    var connectionString =
        builder.Configuration.GetConnectionString(
            "DefaultConnection")
        ?? throw new InvalidOperationException(
            "Subscriber database connection string is missing.");

    options.UseNpgsql(connectionString);
});

builder.Services.AddSingleton<
    ISubscriberDbContextFactory,
    SubscriberDbContextFactory>();

var app = builder.Build();

if (app.Environment.IsDevelopment())
{
    app.MapOpenApi();
}

// Check the release toggle before executing controllers.
app.UseMiddleware<ReleaseToggleMiddleware>();

app.UseSwagger();
app.UseSwaggerUI();

app.UseAuthorization();

app.MapControllers();

app.Run();
