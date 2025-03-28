namespace Watch.Manager.Service.Database;

using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;

using Pgvector;
using Pgvector.EntityFrameworkCore;

using Watch.Manager.Service.Database.Abstractions;
using Watch.Manager.Service.Database.Context;
using Watch.Manager.Service.Database.Entities;

internal class ArticleAnalyseStore(ILogger<ArticleAnalyseStore> logger, ArticlesContext articlesContext) : IArticleAnalyseStore
{
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
            orderedQueryable = orderedQueryable.OrderBy(p => p.EmbeddingBody.CosineDistance(embeddingVector)).ThenBy(p => p.EmbeddingHead.CosineDistance(embeddingVector));


        return await orderedQueryable
                    .Take(5)
                    .ToArrayAsync(cancellationToken).ConfigureAwait(false);
    }
}
