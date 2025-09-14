using Watch.Manager.Service.Database.Extensions;
using Watch.Manager.Service.Migrations;
using Watch.Manager.ServiceDefaults;

var builder = Host.CreateApplicationBuilder(args);
builder.AddServiceDefaults();

builder.AddDatabaseServices();
builder.Services.AddHostedService<Worker>();

builder.Services.AddOpenTelemetry()
       .WithTracing(tracing => tracing.AddSource(Worker.ActivitySourceName));

var host = builder.Build();
host.Run();
