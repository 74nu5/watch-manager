namespace Watch.Manager.Service.Analyse.Models;

/// <summary>
///     Represents the extracted analysis data including tags, authors, summary, and title.
/// </summary>
public sealed class ExtractAnalyseModel
{
    /// <summary>
    ///     Gets or sets the tags associated with the analysis.
    /// </summary>
    public required string[] Tags { get; set; }

    /// <summary>
    ///     Gets or sets the authors of the analysis.
    /// </summary>
    public required string[] Authors { get; set; }

    /// <summary>
    ///     Gets or sets the summary of the analysis.
    /// </summary>
    public required string Summary { get; set; }

    /// <summary>
    ///     Gets the tags joined as a single comma-separated string.
    /// </summary>
    public string TagsJoin => string.Join(",", this.Tags);

    /// <summary>
    ///     Gets the authors joined as a single comma-separated string.
    /// </summary>
    public string AuthorsJoin => string.Join(",", this.Authors);

    /// <summary>
    ///     Gets or sets the title of the analysis.
    /// </summary>
    public required string Title { get; set; }
}
