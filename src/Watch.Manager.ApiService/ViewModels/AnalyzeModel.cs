namespace Watch.Manager.ApiService.ViewModels;

/// <summary>
///     Represents a model containing the URI of a resource to be analyzed.
/// </summary>
/// <remarks>
///     This record is immutable and is intended to encapsulate the URI of a resource that will be processed
///     or analyzed by a consuming service or method.
/// </remarks>
public sealed record AnalyzeModel
{
    /// <summary>
    ///     Gets the URI of the resource to be analyzed.
    /// </summary>
    public required Uri UriToAnalyze { get; init; }
}
