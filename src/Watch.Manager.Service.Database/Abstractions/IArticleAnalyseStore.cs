namespace Watch.Manager.Service.Database.Abstractions;

using Watch.Manager.Service.Database.Entities;

public interface IArticleAnalyseStore
{
    Task StoreArticleAnalyzeAsync(Analyse analyzeModel);
}
