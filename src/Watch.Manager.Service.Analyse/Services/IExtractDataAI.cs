namespace Watch.Manager.Service.Analyse.Services;

using Watch.Manager.Service.Analyse.Models;

/// <summary>
///     Defines a contract for extracting and analyzing data from a given content string.
/// </summary>
/// <remarks>
///     This interface is designed to process textual content and return an analysis result encapsulated in
///     an <see cref="ExtractAnalyseModel" />. The implementation may vary depending on the specific data extraction and
///     analysis logic.
/// </remarks>
public interface IExtractDataAI
{
    /// <summary>
    ///     Extracts and analyzes data from the provided content.
    /// </summary>
    /// <remarks>
    ///     This method performs an asynchronous operation to extract and analyze data from the given
    ///     content.  The caller can use the <paramref name="cancellationToken" /> to cancel the operation if
    ///     needed.
    /// </remarks>
    /// <param name="content">The input content to be analyzed. Cannot be null or empty.</param>
    /// <param name="cancellationToken">A token to monitor for cancellation requests. The default value is <see cref="CancellationToken.None" />.</param>
    /// <returns>
    ///     An <see cref="ExtractAnalyseModel" /> containing the results of the analysis, or <see langword="null" /> if the
    ///     extraction fails.
    /// </returns>
    Task<ExtractAnalyseModel?> ExtractDatasAsync(string content, CancellationToken cancellationToken = default);
}
