
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

app.MapPut("/api/admin/subscriber-toggle/{enabled:bool}",
    async (bool enabled, SubscriberDbContext db) =>
    {
        var flag = await db.ReleaseToggles
            .FirstOrDefaultAsync(x => x.Name == "SubscriberService");

        if (flag is null)
            return Results.NotFound();

        flag.Enabled = enabled;

        await db.SaveChangesAsync();

        return Results.Ok(new
        {
            flag.Name,
            flag.Enabled
        });
    });

app.UseAuthorization();

app.MapControllers();

app.Run();
