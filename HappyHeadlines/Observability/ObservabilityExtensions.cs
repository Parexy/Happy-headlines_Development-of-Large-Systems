using System.Diagnostics;
using System.Reflection;
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
        var serviceName =
            builder.Environment.ApplicationName;

        var serviceVersion =
            Assembly.GetEntryAssembly()?
                .GetName()
                .Version?
                .ToString()
            ?? "1.0.0";

        var seqUrl =
            builder.Configuration["Observability:SeqUrl"]
            ?? "http://seq:5341";

        var otlpEndpoint =
            builder.Configuration["Observability:OtlpEndpoint"]
            ?? "http://zipkin:9411/v1/traces";


        // ====================================================
        // Logging - Serilog + Seq
        // ====================================================

        Log.Logger = new LoggerConfiguration()
            .MinimumLevel.Information()
            .Enrich.FromLogContext()
            .Enrich.WithProperty(
                "ServiceName",
                serviceName)
            .WriteTo.Console()
            .WriteTo.Seq(seqUrl)
            .CreateLogger();

        builder.Host.UseSerilog();


        // ====================================================
        // Distributed tracing - OpenTelemetry
        // ====================================================

        Activity.DefaultIdFormat =
            ActivityIdFormat.W3C;

        Activity.ForceDefaultIdFormat = true;

        builder.Services
            .AddOpenTelemetry()
            .ConfigureResource(resource =>
            {
                resource.AddService(
                    serviceName: serviceName,
                    serviceVersion: serviceVersion);
            })
            .WithTracing(tracing =>
            {
                tracing
                    .AddSource(
                        HappyHeadlinesActivitySource.Name)
                    .AddAspNetCoreInstrumentation()
                    .AddHttpClientInstrumentation()
                    .AddEntityFrameworkCoreInstrumentation()
                    .AddOtlpExporter(options =>
                    {
                        options.Endpoint =
                            new Uri(otlpEndpoint);

                        options.Protocol =
                            OtlpExportProtocol.HttpProtobuf;
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


// ============================================================
// Shared ActivitySource
// ============================================================
//
// Used for custom spans that are not created automatically,
// such as RabbitMQ publish and consume operations.
// ============================================================

public static class HappyHeadlinesActivitySource
{
    public const string Name =
        "HappyHeadlines.Messaging";

    public static readonly ActivitySource Source =
        new(Name);
}