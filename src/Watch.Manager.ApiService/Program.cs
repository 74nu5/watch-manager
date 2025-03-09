using Microsoft.AspNetCore.Mvc;

using Watch.Manager.ApiService.ViewModels;
using Watch.Manager.Service.Analyse.Abstractions;
using Watch.Manager.Service.Analyse.Extensions;
using Watch.Manager.Service.Analyse.Services;
using Watch.Manager.ServiceDefaults;

var builder = WebApplication.CreateBuilder(args);

// Add service defaults & Aspire components.
builder.AddServiceDefaults();

// Add services to the container.
builder.Services.AddProblemDetails();

builder.Configuration.AddAnalyzeConfiguration();

var withApiVersioning = builder.Services.AddApiVersioning();

builder.AddDefaultOpenApi(withApiVersioning);
builder.AddAnalyzeServices();

var app = builder.Build();

// Configure the HTTP request pipeline.
app.UseExceptionHandler();
var vApi = app.NewVersionedApi("Catalog");
var api = vApi.MapGroup("api/catalog");
api.MapPost(
    "/analyse",
    async ([FromBody] AnalyzeModel analyzeModel, IWebSiteService webService, IExtractEmbeddingAI extractEmbedding, IExtractDataAI extractData, CancellationToken cancellationToken) =>
    {
        var contentSite = await webService.GetWebSiteSource(analyzeModel.UriToAnalyze.ToString(), cancellationToken).ConfigureAwait(false);
        var embedding = await extractEmbedding.GetEmbeddingAsync(contentSite, cancellationToken).ConfigureAwait(false);
        var analyzeResult = await extractData.ExtractDatasAsync(contentSite, cancellationToken).ConfigureAwait(false);

        if (analyzeResult is null)
            return Results.Problem("Analyze failed");

        return Results.Ok(
            new
            {
                embedding,
                analyzeResult,
            });
    });

app.MapDefaultEndpoints();

app.UseDefaultOpenApi();

app.Run();
