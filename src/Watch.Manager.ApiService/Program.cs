using Microsoft.AspNetCore.Mvc;

using Watch.Manager.ApiService.ViewModels;
using Watch.Manager.Service.Analyse.Abstractions;
using Watch.Manager.Service.Analyse.Extensions;
using Watch.Manager.Service.Analyse.Services;
using Watch.Manager.Service.Database.Abstractions;
using Watch.Manager.Service.Database.Entities;
using Watch.Manager.Service.Database.Extensions;
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
builder.AddDatabaseServices();

var app = builder.Build();

// Configure the HTTP request pipeline.
app.UseExceptionHandler();
var vApi = app.NewVersionedApi("Catalog");
var api = vApi.MapGroup("api/catalog");
api.MapPost(
    "/analyse",
    async ([FromBody] AnalyzeModel analyzeModel, IWebSiteService webService, IExtractEmbeddingAI extractEmbedding, IExtractDataAI extractData, IArticleAnalyseStore analyseStore, CancellationToken cancellationToken) =>
    {
        var contentSite = await webService.GetWebSiteSource(analyzeModel.UriToAnalyze.ToString(), cancellationToken).ConfigureAwait(false);
        var embeddingsHead = await extractEmbedding.GetEmbeddingAsync(contentSite.Head, cancellationToken).ConfigureAwait(false);
        var embeddingsBody = await extractEmbedding.GetEmbeddingAsync(contentSite.Body, cancellationToken).ConfigureAwait(false);

        if (embeddingsHead is null || embeddingsBody is null)
            return Results.Problem("Embedding failed");

        var analyzeResult = await extractData.ExtractDatasAsync(contentSite.Body, cancellationToken).ConfigureAwait(false);

        if (analyzeResult is null)
            return Results.Problem("Analyze failed");

        Article article = new()
        {
            Summary = analyzeResult.Summary,
            Authors = analyzeResult.Authors,
            Url = analyzeModel.UriToAnalyze,
            EmbeddingHead = new(embeddingsHead.AsMemory()),
            EmbeddingBody = new(embeddingsBody.AsMemory()),
            Tags = analyzeResult.Tags,
            AnalyzeDate = DateTime.UtcNow,
        };

        await analyseStore.StoreArticleAnalyzeAsync(article, cancellationToken).ConfigureAwait(false);

        return Results.Ok(
            new
            {
                analyzeResult,
            });
    });

api.MapGet(
    "/search",
    async ([FromQuery] string text, IExtractEmbeddingAI extractEmbedding, IArticleAnalyseStore analyseStore, CancellationToken cancellationToken) =>
    {
        var embeddings = await extractEmbedding.GetEmbeddingAsync(text, cancellationToken).ConfigureAwait(false);

        if (embeddings is null)
            return Results.Problem("Embedding failed");

        var articles = await analyseStore.SearchArticleAsync(embeddings, cancellationToken).ConfigureAwait(false);
        return Results.Ok(articles);
    });

app.MapDefaultEndpoints();

app.UseDefaultOpenApi();

app.Run();
