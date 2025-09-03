namespace Watch.Manager.Service.Analyse.Services;

using Watch.Manager.Service.Analyse.Models;

/// <summary>
///     Defines an interface for extracting embedding vectors using an AI system.
/// </summary>
/// <remarks>
///     This interface provides methods to generate embedding vectors for text or catalog items,  which can
///     be used for tasks such as similarity analysis, clustering, or other AI-driven operations.  Implementations of this
///     interface may vary in how embeddings are generated, but the exposed methods  ensure a consistent API for
///     consumers.
/// </remarks>
public interface IExtractEmbeddingAI
{
    /// <summary>
    ///     Gets whether the AI system is enabled.
    /// </summary>
    bool IsEnabled { get; }

    /// <summary>
    ///     Gets an embedding vector for the specified text.
    /// </summary>
    ValueTask<float[]?> GetEmbeddingAsync(string text, CancellationToken cancellationToken);

    /// <summary>
    ///     Gets an embedding vector for the specified catalog item.
    /// </summary>
    ValueTask<float[]?> GetEmbeddingAsync(ExtractAnalyseModel item, CancellationToken cancellationToken);

    /// <summary>
    ///     Gets embedding vectors for the specified catalog items.
    /// </summary>
    ValueTask<IReadOnlyList<float[]>?> GetEmbeddingsAsync(IEnumerable<ExtractAnalyseModel> item, CancellationToken cancellationToken);
}
