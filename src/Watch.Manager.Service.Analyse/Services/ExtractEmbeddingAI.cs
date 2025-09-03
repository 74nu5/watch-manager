namespace Watch.Manager.Service.Analyse.Services;

using System.Diagnostics;

using Microsoft.Extensions.AI;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;

using Watch.Manager.Service.Analyse.Models;

/// <summary>
///     Provides functionality to generate embeddings for text or models using an AI-based embedding generator.
/// </summary>
/// <remarks>
///     This class is designed to work with an embedding generator that produces embeddings for text input.
///     It supports generating embeddings for individual text inputs or collections of models. The embeddings are truncated
///     to a fixed dimensionality of 1536. The class also provides logging for tracing the embedding generation
///     process.
/// </remarks>
public sealed class ExtractEmbeddingAI : IExtractEmbeddingAI
{
    private const int EmbeddingDimensions = 1536;

    private readonly IEmbeddingGenerator<string, Embedding<float>>? embeddingGenerator;

    /// <summary>Logger for use in AI operations.</summary>
    private readonly ILogger logger;

    /// <summary>
    ///     Initializes a new instance of the <see cref="ExtractEmbeddingAI" /> class.
    /// </summary>
    /// <remarks>
    ///     The constructor resolves an <see cref="IEmbeddingGenerator{TInput, TEmbedding}" /> instance
    ///     from the provided service provider. Ensure that the service provider is configured to supply the required
    ///     dependencies before using this class.
    /// </remarks>
    /// <param name="logger">The logger instance used to log diagnostic and operational messages.</param>
    /// <param name="provider">The service provider used to resolve dependencies, including the embedding generator.</param>
    public ExtractEmbeddingAI(ILogger<ExtractEmbeddingAI> logger, IServiceProvider provider)
    {
        this.embeddingGenerator = provider.GetService<IEmbeddingGenerator<string, Embedding<float>>>();
        this.logger = logger;
    }

    /// <inheritdoc />
    public bool IsEnabled => this.embeddingGenerator is not null;

    /// <inheritdoc />
    public ValueTask<float[]?> GetEmbeddingAsync(ExtractAnalyseModel item, CancellationToken cancellationToken)
        => this.IsEnabled
                   ? this.GetEmbeddingAsync(CatalogItemToString(item), cancellationToken)
                   : ValueTask.FromResult<float[]?>(null);

    /// <inheritdoc />
    public async ValueTask<IReadOnlyList<float[]>?> GetEmbeddingsAsync(IEnumerable<ExtractAnalyseModel> items, CancellationToken cancellationToken)
    {
        if (!this.IsEnabled)
            return null;

        var timestamp = Stopwatch.GetTimestamp();

        var embeddings = await this.embeddingGenerator!.GenerateAsync(items.Select(CatalogItemToString), cancellationToken: cancellationToken).ConfigureAwait(false);
        var results = embeddings.Select(m => m.Vector[..EmbeddingDimensions].ToArray()).ToList();

        if (this.logger.IsEnabled(LogLevel.Trace))
            this.logger.LogTrace("Generated {EmbeddingsCount} embeddings in {ElapsedMilliseconds}s", results.Count, Stopwatch.GetElapsedTime(timestamp).TotalSeconds);

        return results;
    }

    /// <inheritdoc />
    public async ValueTask<float[]?> GetEmbeddingAsync(string text, CancellationToken cancellationToken)
    {
        if (!this.IsEnabled)
            return null;

        var timestamp = Stopwatch.GetTimestamp();
        var embedding = await this.embeddingGenerator!.GenerateVectorAsync(text, cancellationToken: cancellationToken).ConfigureAwait(false);
        embedding = embedding[..EmbeddingDimensions];

        if (this.logger.IsEnabled(LogLevel.Trace))
            this.logger.LogTrace("Generated embedding in {ElapsedMilliseconds}s: '{Text}'", Stopwatch.GetElapsedTime(timestamp).TotalSeconds, text);

        return embedding.ToArray();
    }

    private static string CatalogItemToString(ExtractAnalyseModel item) => $"{item.TagsJoin} {item.Summary} {item.AuthorsJoin}";
}
