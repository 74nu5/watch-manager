namespace Watch.Manager.ServiceDefaults;

using Microsoft.AspNetCore.Builder;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Diagnostics.HealthChecks;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;

using OpenTelemetry.Logs;
using OpenTelemetry.Metrics;
using OpenTelemetry.Trace;

/// <summary>
///     Provides extension methods for configuring service defaults, OpenTelemetry, health checks, and endpoints.
/// </summary>
public static partial class Extensions
{
    /// <summary>
    ///     Adds default service configurations including OpenTelemetry, health checks, service discovery, and HTTP client defaults.
    /// </summary>
    /// <param name="builder">The <see cref="IHostApplicationBuilder" /> to configure.</param>
    /// <returns>The configured <see cref="IHostApplicationBuilder" />.</returns>
    public static IHostApplicationBuilder AddServiceDefaults(this IHostApplicationBuilder builder)
    {
        _ = builder.ConfigureOpenTelemetry();

        _ = builder.AddDefaultHealthChecks();

        _ = builder.Services.AddServiceDiscovery();

        _ = builder.Services.ConfigureHttpClientDefaults(http =>

                // Turn on resilience by default
                // _ = http.AddStandardResilienceHandler();

                // Turn on service discovery by default
                _ = http.AddServiceDiscovery());

        return builder;
    }

    /// <summary>
    ///     Maps default endpoints for health checks and optionally Prometheus scraping.
    /// </summary>
    /// <param name="app">The <see cref="WebApplication" /> to configure.</param>
    /// <returns>The configured <see cref="WebApplication" />.</returns>
    public static WebApplication MapDefaultEndpoints(this WebApplication app)
    {
        if (!app.Environment.IsDevelopment())
            return app;

        // All health checks must pass for app to be considered ready to accept traffic after starting
        _ = app.MapHealthChecks("/health");

        // Only health checks tagged with the "live" tag must pass for app to be considered alive
        _ = app.MapHealthChecks(
            "/alive",
            new()
            {
                Predicate = r => r.Tags.Contains("live"),
            });

        return app;
    }

    /// <summary>
    ///     Configures OpenTelemetry logging, metrics, and tracing for the application.
    /// </summary>
    /// <param name="builder">The <see cref="IHostApplicationBuilder" /> to configure.</param>
    /// <returns>The configured <see cref="IHostApplicationBuilder" />.</returns>
    private static IHostApplicationBuilder ConfigureOpenTelemetry(this IHostApplicationBuilder builder)
    {
        _ = builder.Logging.AddOpenTelemetry(logging =>
        {
            logging.IncludeFormattedMessage = true;
            logging.IncludeScopes = true;
        });

        _ = builder.Services.AddOpenTelemetry()
                   .WithMetrics(metrics => _ = metrics.AddAspNetCoreInstrumentation()
                                                      .AddHttpClientInstrumentation()
                                                      .AddRuntimeInstrumentation())
                   .WithTracing(tracing =>
                    {
                        if (builder.Environment.IsDevelopment())
                            _ = tracing.SetSampler(new AlwaysOnSampler());

                        _ = tracing.AddAspNetCoreInstrumentation()
                                   .AddHttpClientInstrumentation();
                    });

        _ = builder.AddOpenTelemetryExporters();

        return builder;
    }

    /// <summary>
    ///     Adds default health checks to the application, including a liveness check.
    /// </summary>
    /// <param name="builder">The <see cref="IHostApplicationBuilder" /> to configure.</param>
    /// <returns>The configured <see cref="IHostApplicationBuilder" />.</returns>
    private static IHostApplicationBuilder AddDefaultHealthChecks(this IHostApplicationBuilder builder)
    {
        _ = builder.Services.AddHealthChecks().AddCheck("self", () => HealthCheckResult.Healthy(), ["live"]);

        return builder;
    }

    /// <summary>
    ///     Adds OpenTelemetry exporters based on configuration, such as OTLP, Prometheus, or Azure Monitor.
    /// </summary>
    /// <param name="builder">The <see cref="IHostApplicationBuilder" /> to configure.</param>
    /// <returns>The configured <see cref="IHostApplicationBuilder" />.</returns>
    private static IHostApplicationBuilder AddOpenTelemetryExporters(this IHostApplicationBuilder builder)
    {
        var useOtlpExporter = !string.IsNullOrWhiteSpace(builder.Configuration["OTEL_EXPORTER_OTLP_ENDPOINT"]);

        if (useOtlpExporter)
        {
            _ = builder.Services.Configure<OpenTelemetryLoggerOptions>(logging => logging.AddOtlpExporter());
            _ = builder.Services.ConfigureOpenTelemetryMeterProvider(metrics => metrics.AddOtlpExporter());
            _ = builder.Services.ConfigureOpenTelemetryTracerProvider(tracing => tracing.AddOtlpExporter());
        }

        // Uncomment the following lines to enable the Azure Monitor exporter (requires the Azure.Monitor.OpenTelemetry.AspNetCore package)
        // if (!string.IsNullOrEmpty(builder.Configuration["APPLICATIONINSIGHTS_CONNECTION_STRING"]))
        // {
        //     builder.Services.AddOpenTelemetry()
        //        .UseAzureMonitor();
        // }
        return builder;
    }
}
