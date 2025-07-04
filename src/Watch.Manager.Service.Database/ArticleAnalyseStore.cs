namespace Watch.Manager.Service.Database;

using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;

using Pgvector;
using Pgvector.EntityFrameworkCore;

using Watch.Manager.Service.Database.Abstractions;
using Watch.Manager.Service.Database.Context;
using Watch.Manager.Service.Database.Entities;

internal sealed class ArticleAnalyseStore(ILogger<ArticleAnalyseStore> logger, ArticlesContext articlesContext) : IArticleAnalyseStore
{
    private const double Threshold = 0.75;

    public async Task StoreArticleAnalyzeAsync(Article analyzeModel, CancellationToken cancellationToken)
    {
        _ = articlesContext.Articles.Add(analyzeModel);
        _ = await articlesContext.SaveChangesAsync(cancellationToken).ConfigureAwait(false);
    }

    public async Task<bool> IsArticleExistsAsync(Uri url, CancellationToken cancellationToken)
        => await articlesContext.Articles.AnyAsync(p => p.Url == url, cancellationToken).ConfigureAwait(false);

    public async Task<Article[]> SearchArticleAsync(float[] embedding, CancellationToken cancellationToken)
    {
        var embeddingVector = new Vector(embedding.AsMemory());
        IQueryable<Article> orderedQueryable = articlesContext.Articles;

        if (embedding.Length > 0)
        {
            //var test = orderedQueryable.Select(a => new
            //{
            //    a.Title,
            //    Body = a.EmbeddingBody.CosineDistance(embeddingVector),
            //    Head = a.EmbeddingHead.CosineDistance(embeddingVector),
            //}).ToList();
            orderedQueryable = orderedQueryable
                              //.Where(p => p.EmbeddingBody.CosineDistance(embeddingVector) < Threshold && p.EmbeddingHead.CosineDistance(embeddingVector) < Threshold)
                              .OrderBy(p => p.EmbeddingBody.CosineDistance(embeddingVector)).ThenBy(p => p.EmbeddingHead.CosineDistance(embeddingVector));
        }


        return await orderedQueryable
                    .OrderByDescending(a => a.AnalyzeDate)
                    .ToArrayAsync(cancellationToken).ConfigureAwait(false);
    }

    public async Task<string[]> GetAllTagsAsync(CancellationToken cancellationToken)
        => await articlesContext.Articles.SelectMany(p => p.Tags).Distinct().OrderBy(s => s).ToArrayAsync(cancellationToken).ConfigureAwait(false);
}
