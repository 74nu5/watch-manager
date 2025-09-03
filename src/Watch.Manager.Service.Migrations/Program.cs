using Watch.Manager.Service.Database.Extensions;
using Watch.Manager.Service.Migrations;
using Watch.Manager.ServiceDefaults;

var builder = Host.CreateApplicationBuilder(args);
builder.AddServiceDefaults();

builder.Services.AddHostedService<Worker>();

builder.Services.AddOpenTelemetry()
       .WithTracing(tracing => tracing.AddSource(Worker.ActivitySourceName));

builder.AddDatabaseServices();

var host = builder.Build();
host.Run();
