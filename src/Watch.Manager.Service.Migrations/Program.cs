using Watch.Manager.Service.Database.Context;
using Watch.Manager.Service.Migrations;
using Watch.Manager.ServiceDefaults;

var builder = Host.CreateApplicationBuilder(args);
builder.AddServiceDefaults();

builder.Services.AddHostedService<Worker>();

builder.Services.AddOpenTelemetry()
       .WithTracing(tracing => tracing.AddSource(Worker.ActivitySourceName));

builder.AddSqlServerDbContext<ArticlesContext>("articlesdb");

var host = builder.Build();
host.Run();
