using System.Net;

using Microsoft.AspNetCore.Mvc;

using Watch.Manager.ApiService.Parameters;
using Watch.Manager.ApiService.ViewModels;
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
var vApi = app.NewVersionedApi("Articles");
var api = vApi.MapGroup("api/articles");
api.MapPost(
    "/save",
    async ([AsParameters] AnalyzeParameter analyzeParameter, CancellationToken cancellationToken) =>
    {
        try
        {
            var urlAlreadyExists = await analyzeParameter.ArticleAnalyseStore.IsArticleExistsAsync(analyzeParameter.AnalyzeModel.UriToAnalyze, cancellationToken).ConfigureAwait(false);
            if (urlAlreadyExists)
                return Results.Conflict("Url already exists");

            var (head, body, thumbnail) = await analyzeParameter.WebSiteService.GetWebSiteSource(analyzeParameter.AnalyzeModel.UriToAnalyze.ToString(), CancellationToken.None).ConfigureAwait(false);
            var embeddingsHead = await analyzeParameter.ExtractEmbeddingAi.GetEmbeddingAsync(head, cancellationToken).ConfigureAwait(false);
            var embeddingsBody = await analyzeParameter.ExtractEmbeddingAi.GetEmbeddingAsync(body, cancellationToken).ConfigureAwait(false);

            if (embeddingsHead is null || embeddingsBody is null)
                return Results.Problem("Embedding failed");

            var analyzeResult = await analyzeParameter.ExtractDataAi.ExtractDatasAsync(body, cancellationToken).ConfigureAwait(false);

            if (analyzeResult is null)
                return Results.Problem("Analyze failed");

            Article article = new()
            {
                Summary = analyzeResult.Summary,
                Authors = analyzeResult.Authors,
                Url = analyzeParameter.AnalyzeModel.UriToAnalyze,
                Thumbnail = thumbnail,
                Title = analyzeResult.Title,
                EmbeddingHead = embeddingsHead,
                EmbeddingBody = embeddingsBody,
                Tags = [.. analyzeResult.Tags],
                AnalyzeDate = DateTime.UtcNow,
            };

            await analyzeParameter.ArticleAnalyseStore.StoreArticleAnalyzeAsync(article, cancellationToken).ConfigureAwait(false);

            return Results.Ok(analyzeResult);
        }
        catch (HttpRequestException httpRequestException) when(httpRequestException.StatusCode == HttpStatusCode.NotFound)
        {
            analyzeParameter.Logger.LogWarning(httpRequestException, "Url not found");
            return Results.NotFound("Url not found");
        }
        catch (HttpRequestException httpRequestException) when(httpRequestException.StatusCode == HttpStatusCode.Forbidden)
        {
            analyzeParameter.Logger.LogWarning(httpRequestException, "Url is forbidden");
            return Results.StatusCode(403);
        }
        catch (Exception e)
        {
            analyzeParameter.Logger.LogError(e, "Failed to save article");
            return Results.InternalServerError(e);
        }
    });

api.MapGet(
    "/search",
    Handler);

async IAsyncEnumerable<ArticleViewModel> Handler([FromQuery] string text, [FromServices] IExtractEmbeddingAI extractEmbedding, [FromServices] IArticleAnalyseStore analyseStore, CancellationToken cancellationToken)
{
    await foreach (var article in analyseStore.SearchArticleAsync(text, cancellationToken).ConfigureAwait(false))
    {
        yield return new()
        {
            Id = article.Id,
            Summary = article.Summary,
            Authors = article.Authors,
            Url = article.Url,
            Title = article.Title,
            Tags = article.Tags,
            AnalyzeDate = article.AnalyzeDate,
            Thumbnail = article.Thumbnail,
            Score = article.Score,
        };
    }
}

api.MapGet(
    "tags",
    async (IArticleAnalyseStore articleAnalyseStore, CancellationToken cancellationToken) =>
    {
        var tags = await articleAnalyseStore.GetAllTagsAsync(cancellationToken).ConfigureAwait(false);
        return Results.Ok(tags.Distinct(StringComparer.InvariantCultureIgnoreCase));
    });

app.MapDefaultEndpoints();

app.UseDefaultOpenApi();

app.Run();
