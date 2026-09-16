using Microsoft.AspNetCore.Builder;
using Microsoft.Extensions.DependencyInjection;
using OpenTelemetry.Exporter;
using OpenTelemetry.Resources;
using OpenTelemetry.Trace;
using Serilog;

namespace Observability;

public static class ObservabilityExtensions
{
    public static WebApplicationBuilder AddObservability(
        this WebApplicationBuilder builder)
    {
        var serviceName = builder.Environment.ApplicationName;

        var seqUrl = builder.Configuration["Observability:SeqUrl"]
                     ?? "http://localhost:5341";

        var zipkinUrl = builder.Configuration["Observability:ZipkinUrl"]
                        ?? "http://localhost:9411";

        builder.Host.UseSerilog((context, services, configuration) =>
        {
            configuration
                .ReadFrom.Configuration(context.Configuration)
                .ReadFrom.Services(services)
                .Enrich.FromLogContext()
                .Enrich.WithProperty("ServiceName", serviceName)
                .WriteTo.Console()
                .WriteTo.Seq(seqUrl);
        });

        builder.Services
            .AddOpenTelemetry()
            .ConfigureResource(resource =>
                resource.AddService(serviceName))
            .WithTracing(tracing =>
            {
                tracing
                    .AddAspNetCoreInstrumentation()
                    .AddHttpClientInstrumentation()
                    .AddOtlpExporter(options =>
                    {
                        options.Endpoint = new Uri(zipkinUrl);
                        options.Protocol = OtlpExportProtocol.HttpProtobuf;
                    });
            });

        return builder;
    }

    public static WebApplication UseObservability(
        this WebApplication app)
    {
        app.UseSerilogRequestLogging();

        return app;
    }
}