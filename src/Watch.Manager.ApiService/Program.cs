using Microsoft.AspNetCore.Mvc;

using Watch.Manager.ApiService.ViewModels;
using Watch.Manager.Service.Analyse.Abstractions;
using Watch.Manager.Service.Analyse.Extensions;

var builder = WebApplication.CreateBuilder(args);

// Add service defaults & Aspire components.
builder.AddServiceDefaults();
builder.AddAzureCosmosClient("cosmosdb");

// Add services to the container.
builder.Services.AddProblemDetails();

builder.Configuration.AddAnalyzeConfiguration();

builder.Services.AddAnalyzeServices();

var app = builder.Build();

// Configure the HTTP request pipeline.
app.UseExceptionHandler();

var summaries = new[]
{
    "Freezing", "Bracing", "Chilly", "Cool", "Mild", "Warm", "Balmy", "Hot", "Sweltering", "Scorching",
};

app.MapGet(
    "/analyse",
    async ([FromBody] AnalyzeModel analyzeModel, IWebSiteService webService, ISiteAnalyzeService siteAnalyze, CancellationToken cancellationToken) =>
    {
        var contentSite = await webService.GetWebSiteSource(analyzeModel.UriToAnalyze.ToString(), cancellationToken).ConfigureAwait(false);
        return await siteAnalyze.ExtractTagsAsync(contentSite, cancellationToken).ConfigureAwait(false);
    });

app.MapDefaultEndpoints();

app.Run();
