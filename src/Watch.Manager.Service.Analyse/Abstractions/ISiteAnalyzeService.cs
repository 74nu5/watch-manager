namespace Watch.Manager.Service.Analyse.Abstractions;

using Watch.Manager.Service.Analyse.Models;

public interface ISiteAnalyzeService
{
    Task<ExtractAnalyseModel?> ExtractTagsAsync(string content, CancellationToken cancellationToken = default);
}
