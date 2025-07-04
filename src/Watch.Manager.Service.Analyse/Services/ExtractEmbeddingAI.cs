namespace Watch.Manager.Service.Analyse.Services;

using System.Diagnostics;

using Microsoft.Extensions.AI;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;

using Watch.Manager.Service.Analyse.Models;

public sealed class ExtractEmbeddingAI : IExtractEmbeddingAI
{
    private const int EmbeddingDimensions = 1536;

    private readonly IEmbeddingGenerator<string, Embedding<float>>? embeddingGenerator;

    /// <summary>Logger for use in AI operations.</summary>
    private readonly ILogger logger;

    public ExtractEmbeddingAI(ILogger<ExtractEmbeddingAI> logger, IServiceProvider provider)
    {
        this.embeddingGenerator = provider.GetService<IEmbeddingGenerator<string, Embedding<float>>>();
        this.logger = logger;
    }

    /// <inheritdoc />
    public bool IsEnabled => this.embeddingGenerator is not null;

    /// <inheritdoc />
    public ValueTask<float[]?> GetEmbeddingAsync(ExtractAnalyseModel item, CancellationToken cancellationToken) => this.IsEnabled ? this.GetEmbeddingAsync(CatalogItemToString(item), cancellationToken: cancellationToken) : ValueTask.FromResult<float[]?>(null);

    /// <inheritdoc />
    public async ValueTask<IReadOnlyList<float[]>?> GetEmbeddingsAsync(IEnumerable<ExtractAnalyseModel> items, CancellationToken cancellationToken)
    {
        if (this.IsEnabled)
        {
            var timestamp = Stopwatch.GetTimestamp();

            var embeddings = await this.embeddingGenerator!.GenerateAsync(items.Select(CatalogItemToString), cancellationToken: cancellationToken).ConfigureAwait(false);
            var results = embeddings.Select(m => m.Vector[..EmbeddingDimensions].ToArray()).ToList();

            if (this.logger.IsEnabled(LogLevel.Trace))
                this.logger.LogTrace("Generated {EmbeddingsCount} embeddings in {ElapsedMilliseconds}s", results.Count, Stopwatch.GetElapsedTime(timestamp).TotalSeconds);

            return results;
        }

        return null;
    }

    /// <inheritdoc />
    public async ValueTask<float[]?> GetEmbeddingAsync(string text, CancellationToken cancellationToken)
    {
        if (this.IsEnabled)
        {
            var timestamp = Stopwatch.GetTimestamp();

            var embedding = await this.embeddingGenerator!.GenerateVectorAsync(text, cancellationToken: cancellationToken).ConfigureAwait(false);
            embedding = embedding[..EmbeddingDimensions];

            if (this.logger.IsEnabled(LogLevel.Trace))
                this.logger.LogTrace("Generated embedding in {ElapsedMilliseconds}s: '{Text}'", Stopwatch.GetElapsedTime(timestamp).TotalSeconds, text);

            return embedding.ToArray();
        }

        return null;
    }

    private static string CatalogItemToString(ExtractAnalyseModel item) => $"{item.TagsJoin} {item.Summary} {item.AuthorsJoin}";
}
