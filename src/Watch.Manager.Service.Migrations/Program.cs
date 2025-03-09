using Microsoft.EntityFrameworkCore;

using Watch.Manager.Service.Database.Context;
using Watch.Manager.Service.Migrations;
using Watch.Manager.ServiceDefaults;

var builder = Host.CreateApplicationBuilder(args);
builder.AddServiceDefaults();

builder.Services.AddHostedService<Worker>();

builder.Services.AddOpenTelemetry()
       .WithTracing(tracing => tracing.AddSource(Worker.ActivitySourceName));

builder.AddNpgsqlDbContext<ArticlesContext>("articles-db", configureDbContextOptions: dbContextOptionsBuilder => dbContextOptionsBuilder.UseNpgsql(contextOptionsBuilder => contextOptionsBuilder.UseVector()));

var host = builder.Build();
host.Run();
