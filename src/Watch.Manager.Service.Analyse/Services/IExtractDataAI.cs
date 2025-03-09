namespace Watch.Manager.Service.Analyse.Services;

using Watch.Manager.Service.Analyse.Models;

public interface IExtractDataAI
{
    Task<ExtractAnalyseModel?> ExtractDatasAsync(string content, CancellationToken cancellationToken = default);
}
