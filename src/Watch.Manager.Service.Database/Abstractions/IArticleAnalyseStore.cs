namespace Watch.Manager.Service.Database.Abstractions;

using Watch.Manager.Service.Database.Entities;
using Watch.Manager.Service.Database.Models;

public interface IArticleAnalyseStore
{
    Task StoreArticleAnalyzeAsync(Article analyzeModel, CancellationToken cancellationToken);

    IAsyncEnumerable<ArticleResultDto> SearchArticleAsync(string searchTerms, CancellationToken cancellationToken);

    Task<bool> IsArticleExistsAsync(Uri url, CancellationToken cancellationToken);

    Task<string[]> GetAllTagsAsync(CancellationToken cancellationToken);

    ValueTask<(MemoryStream, string? FileName)> GetThumbnailAsync(int id, CancellationToken cancellationToken);
}
