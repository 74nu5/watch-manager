namespace Watch.Manager.Service.Database.Abstractions;

using Watch.Manager.Service.Database.Entities;

public interface IArticleAnalyseStore
{
    Task StoreArticleAnalyzeAsync(Article analyzeModel, CancellationToken cancellationToken);

    Task<Article[]> SearchArticleAsync(float[] embeddings, CancellationToken cancellationToken);
}
