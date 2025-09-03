namespace Watch.Manager.ApiService.Parameters.Articles;

using Microsoft.AspNetCore.Mvc;

using Watch.Manager.ApiService.ViewModels;
using Watch.Manager.Service.Analyse.Abstractions;
using Watch.Manager.Service.Analyse.Services;
using Watch.Manager.Service.Database.Abstractions;

/// <summary>
/// Represents the parameters required to analyze an article using AI and database services.
/// </summary>
public record AnalyzeParameter
{
    /// <summary>
    /// Gets or sets the model containing data to be analyzed.
    /// </summary>
    [FromBody]
    public required AnalyzeModel AnalyzeModel { get; set; }

    /// <summary>
    /// Gets or sets service for website operations.
    /// </summary>
    [FromServices]
    public required IWebSiteService WebSiteService { get; set; }

    /// <summary>
    /// Gets or sets service for extracting AI embeddings from articles.
    /// </summary>
    [FromServices]
    public required IExtractEmbeddingAI ExtractEmbeddingAi { get; set; }

    /// <summary>
    /// Gets or sets service for extracting data using AI.
    /// </summary>
    [FromServices]
    public required IExtractDataAI ExtractDataAi { get; set; }

    /// <summary>
    /// Gets or sets store for article analysis data.
    /// </summary>
    [FromServices]
    public required IArticleAnalyseStore ArticleAnalyseStore { get; set; }

    /// <summary>
    /// Gets or sets service for article classification using AI.
    /// </summary>
    [FromServices]
    public required IArticleClassificationAI ClassificationService { get; set; }

    /// <summary>
    /// Gets or sets store for category data.
    /// </summary>
    [FromServices]
    public required ICategoryStore CategoryStore { get; set; }

    /// <summary>
    /// Gets or sets logger for logging analysis operations.
    /// </summary>
    [FromServices]
    public required ILogger<AnalyzeParameter> Logger { get; set; }
}
