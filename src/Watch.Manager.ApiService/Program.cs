using Microsoft.AspNetCore.Mvc;

using Watch.Manager.ApiService.ViewModels;
using Watch.Manager.Service.Analyse.Abstractions;
using Watch.Manager.Service.Analyse.Extensions;
using Watch.Manager.Service.Database.Abstractions;
using Watch.Manager.Service.Database.Entities;
using Watch.Manager.Service.Database.Extensions;

var builder = WebApplication.CreateBuilder(args);

// Add service defaults & Aspire components.
builder.AddServiceDefaults();
builder.AddAzureCosmosClient("cosmosdb");

// Add services to the container.
builder.Services.AddProblemDetails();

builder.Configuration.AddAnalyzeConfiguration();

builder.Services.AddAnalyzeServices();
builder.Services.AddDatabaseServices();

var app = builder.Build();

// Configure the HTTP request pipeline.
app.UseExceptionHandler();

var summaries = new[]
{
    "Freezing", "Bracing", "Chilly", "Cool", "Mild", "Warm", "Balmy", "Hot", "Sweltering", "Scorching",
};

app.MapGet(
    "/analyse",
    async ([FromBody] AnalyzeModel analyzeModel, IWebSiteService webService, ISiteAnalyzeService siteAnalyze, IArticleAnalyseStore store, CancellationToken cancellationToken) =>
    {
        var contentSite = await webService.GetWebSiteSource(analyzeModel.UriToAnalyze.ToString(), cancellationToken).ConfigureAwait(false);
        var analyzeResult = await siteAnalyze.ExtractTagsAsync(contentSite, cancellationToken).ConfigureAwait(false);

        if (analyzeResult is null)
            return Results.Problem("Analyze failed");

        await store.StoreArticleAnalyzeAsync(new()
        {
            PartitionKey = string.Join(",", analyzeResult.Tags),
            Id = Guid.NewGuid().ToString(),
            Tags = analyzeResult.Tags,
            Url = analyzeModel.UriToAnalyze,
            AnalyzeDate = DateTime.UtcNow,
            Authors = analyzeResult.Authors,
            Summary = analyzeResult.Summary,
        }).ConfigureAwait(false);

        return Results.Ok(analyzeResult);
    });

app.MapDefaultEndpoints();

app.Run();
