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
        articlesContext.Articles.Add(analyzeModel);
        await articlesContext.SaveChangesAsync(cancellationToken).ConfigureAwait(false);
    }

    public async Task<Article[]> SearchArticleAsync(float[] embedding, CancellationToken cancellationToken)
    {
        var embeddingVector = new Vector(embedding.AsMemory());
        return await articlesContext.Articles
                                    .OrderBy(p => p.EmbeddingBody.CosineDistance(embeddingVector)).ThenBy(p => p.EmbeddingHead.CosineDistance(embeddingVector))
                                    .Take(5)
                                    .ToArrayAsync(cancellationToken).ConfigureAwait(false);
    }
}
