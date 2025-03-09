namespace Watch.Manager.Service.Analyse.Services;

using Watch.Manager.Service.Analyse.Models;

public interface IExtractEmbeddingAI
{
    /// <summary>Gets whether the AI system is enabled.</summary>
    bool IsEnabled { get; }

    /// <summary>Gets an embedding vector for the specified text.</summary>
    ValueTask<float[]?> GetEmbeddingAsync(string text, CancellationToken cancellationToken);

    /// <summary>Gets an embedding vector for the specified catalog item.</summary>
    ValueTask<float[]?> GetEmbeddingAsync(ExtractAnalyseModel item, CancellationToken cancellationToken);

    /// <summary>Gets embedding vectors for the specified catalog items.</summary>
    ValueTask<IReadOnlyList<float[]>?> GetEmbeddingsAsync(IEnumerable<ExtractAnalyseModel> item, CancellationToken cancellationToken);
}
