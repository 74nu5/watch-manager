namespace Watch.Manager.Service.Database;

using System.Text.Json;

using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.VectorData;

using Watch.Manager.Service.Database.Abstractions;
using Watch.Manager.Service.Database.Context;
using Watch.Manager.Service.Database.Entities;
using Watch.Manager.Service.Database.Models;

internal sealed class ArticleAnalyseStore(ILogger<ArticleAnalyseStore> logger, ArticlesContext articlesContext, IVectorSearchable<ArticleSearchEntity> searchable) : IArticleAnalyseStore
{
    private const double Threshold = 0.75;

    public async Task StoreArticleAnalyzeAsync(Article analyzeModel, CancellationToken cancellationToken)
    {
        _ = articlesContext.Articles.Add(analyzeModel);
        _ = await articlesContext.SaveChangesAsync(cancellationToken).ConfigureAwait(false);
    }

    public async Task<bool> IsArticleExistsAsync(Uri url, CancellationToken cancellationToken)
        => await articlesContext.Articles.AnyAsync(p => p.Url == url, cancellationToken).ConfigureAwait(false);

    public async IAsyncEnumerable<ArticleResultDto> SearchArticleAsync(string searchTerms, string tag, CancellationToken cancellationToken)
    {
        if(!string.IsNullOrWhiteSpace(tag))
        {
            // Filter by tag if provided
            await foreach (var article in articlesContext.Articles.AsNoTracking().AsQueryable()
                .Where(a => a.Tags.Contains(tag))
                .OrderByDescending(a => a.AnalyzeDate)
                .AsAsyncEnumerable()
                .WithCancellation(cancellationToken)
                .ConfigureAwait(false))
            {
                yield return new()
                {
                    Id = article.Id,
                    Title = article.Title,
                    Tags = article.Tags,
                    Authors = article.Authors,
                    Summary = article.Summary,
                    Url = article.Url,
                    AnalyzeDate = article.AnalyzeDate,
                    Thumbnail = article.Thumbnail,
                    Score = 0, // Default score when no search terms are provided
                };
            }

            yield break;
        }

        if (string.IsNullOrWhiteSpace(searchTerms))
        {
            await foreach (var article in articlesContext.Articles.AsNoTracking().OrderByDescending(a => a.AnalyzeDate).AsAsyncEnumerable().WithCancellation(cancellationToken).ConfigureAwait(false))
            {
                yield return new()
                {
                    Id = article.Id,
                    Title = article.Title,
                    Tags = article.Tags,
                    Authors = article.Authors,
                    Summary = article.Summary,
                    Url = article.Url,
                    AnalyzeDate = article.AnalyzeDate,
                    Thumbnail = article.Thumbnail,
                    Score = 0, // Default score when no search terms are provided
                };
            }

            yield break;
        }

        //var test = orderedQueryable.Select(a => new
        //{
        //    a.Title,
        //    Body = a.EmbeddingBody.CosineDistance(embeddingVector),
        //    Head = a.EmbeddingHead.CosineDistance(embeddingVector),
        //}).ToList();

        // Todo search
        //orderedQueryable = orderedQueryable
        //.Where(p => p.EmbeddingBody.CosineDistance(embeddingVector) < Threshold && p.EmbeddingHead.CosineDistance(embeddingVector) < Threshold)
        // OrderBy(p => p.EmbeddingBody.CosineDistance(embeddingVector)).ThenBy(p => p.EmbeddingHead.CosineDistance(embeddingVector));


        VectorSearchOptions<ArticleSearchEntity> vectorSearchOptions = new()
        {
            VectorProperty = article => article.EmbeddingBody,
        };

        await foreach (var result in searchable.SearchAsync(searchTerms, 10, vectorSearchOptions, cancellationToken).ConfigureAwait(false))
        {
            yield return new()
            {
                Id = result.Record.Id,
                Title = result.Record.Title,
                Tags = JsonSerializer.Deserialize<string[]>(result.Record.Tags) ?? [],
                Authors = JsonSerializer.Deserialize<string[]>(result.Record.Authors) ?? [],
                Summary = result.Record.Summary,
                Url = new(result.Record.Url),
                AnalyzeDate = result.Record.AnalyzeDate,
                Thumbnail = new(result.Record.Thumbnail),
                Score = result.Score,
            };
        }


        /*return await orderedQueryable
                    .OrderByDescending(a => a.AnalyzeDate)
                    .ToArrayAsync(cancellationToken).ConfigureAwait(false);*/
    }

    public async Task<string[]> GetAllTagsAsync(CancellationToken cancellationToken)
        => await articlesContext.Articles.SelectMany(p => p.Tags).Distinct().OrderBy(s => s).ToArrayAsync(cancellationToken).ConfigureAwait(false);

    /// <inheritdoc />
    public async ValueTask<(MemoryStream, string? FileName)> GetThumbnailAsync(int id, CancellationToken cancellationToken)
    {
        var articleThumbnail = await articlesContext.Articles.Where(article => article.Id == id).Select(article => new
        {
            FileName = Path.GetFileName(article.Thumbnail.LocalPath),
            article.ThumbnailBase64,
        }).FirstOrDefaultAsync(cancellationToken).ConfigureAwait(false);
        if (articleThumbnail is null)
            return ((MemoryStream, string? FileName))(Stream.Null, null);

        // base64 to stream
        var thumbnailBytes = Convert.FromBase64String(articleThumbnail.ThumbnailBase64);
        return (new(thumbnailBytes) { Position = 0 }, articleThumbnail.FileName);
    }
}
