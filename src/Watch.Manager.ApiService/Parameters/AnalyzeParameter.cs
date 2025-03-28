namespace Watch.Manager.ApiService.Parameters;

using Microsoft.AspNetCore.Mvc;

using Watch.Manager.ApiService.ViewModels;
using Watch.Manager.Service.Analyse.Abstractions;
using Watch.Manager.Service.Analyse.Services;
using Watch.Manager.Service.Database.Abstractions;

public record AnalyzeParameter
{
    [FromBody]
    public AnalyzeModel AnalyzeModel { get; set; }

    [FromServices]
    public IWebSiteService WebSiteService { get; set; }

    [FromServices]
    public IExtractEmbeddingAI ExtractEmbeddingAi { get; set; }

    [FromServices]
    public IExtractDataAI ExtractDataAi { get; set; }

    [FromServices]
    public IArticleAnalyseStore ArticleAnalyseStore { get; set; }
}
