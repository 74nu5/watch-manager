namespace Watch.Manager.Service.Analyse.Abstractions;

using Azure.AI.OpenAI;

public interface ISiteAnalyzeService
{
    Task<string> ExtractTagsAsync(string content, CancellationToken cancellationToken = default);
}
