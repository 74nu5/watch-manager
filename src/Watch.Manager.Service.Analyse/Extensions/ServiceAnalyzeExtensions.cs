#pragma warning disable KMEXP00
namespace Watch.Manager.Service.Analyse.Extensions;

using Microsoft.Extensions.AI;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;
using Microsoft.Extensions.Hosting;
using Microsoft.KernelMemory.AI;

using OpenAI;

using Watch.Manager.Service.Analyse.Abstractions;
using Watch.Manager.Service.Analyse.Services;

public static class ServiceAnalyzeExtensions
{
    public static IConfigurationBuilder AddAnalyzeConfiguration(this IConfigurationBuilder configuration)
        => configuration.AddAzureAppConfiguration("Endpoint=https://tanus-app-configuration.azconfig.io;Id=f2nS-lw-s0:kdRkJ57qVSe/FshDPeO0;Secret=Kjqm2zGpAxzMbhWasI05KnIWOEJMGn03/yHvCFihplc=");

    public static void AddAnalyzeServices(this IHostApplicationBuilder builder)
    {
        builder.Services.TryAddTransient<SanitizeService>();
        builder.Services.TryAddTransient<IWebSiteService, WebSiteService>();
        
        if (builder.Configuration["OllamaEnabled"] is string ollamaEnabled && bool.Parse(ollamaEnabled))
        {
            _ = builder.AddOllamaApiClient("embedding")
                       .AddEmbeddingGenerator();

            _ = builder.AddOllamaApiClient("chat")
                       .AddChatClient()
                       .UseFunctionInvocation();
        }
        else
        {
            var connectionString = builder.Configuration.GetConnectionString("openai");

            if (!string.IsNullOrWhiteSpace(connectionString))
            {
                var embeddingModel = builder.Configuration["AI:OpenAI:EmbeddingModel"];

                builder.Services.TryAddScoped<ITextTokenizer>(_ => TokenizerFactory.GetTokenizerForModel(embeddingModel ?? string.Empty) ?? throw new InvalidOperationException("Tokenizer not found"));

                _ = builder.AddOpenAIClientFromConfiguration("openai");
                _ = builder.Services.AddEmbeddingGenerator(sp => sp.GetRequiredService<OpenAIClient>().GetEmbeddingClient(embeddingModel!).AsIEmbeddingGenerator())
                           .UseOpenTelemetry()
                           .UseLogging();

                var chatModel = builder.Configuration.GetSection("AI").Get<AIOptions>()?.OpenAI?.ChatModel;

                if (!string.IsNullOrWhiteSpace(connectionString) && !string.IsNullOrWhiteSpace(chatModel))
                {
                    _ = builder.AddOpenAIClientFromConfiguration("openai");
                    _ = builder.Services.AddChatClient(sp => sp.GetRequiredService<OpenAIClient>().GetChatClient(chatModel ?? "gpt-4o-mini").AsIChatClient())
                               .UseFunctionInvocation()
                               .UseOpenTelemetry(configure: t => t.EnableSensitiveData = true)
                               .UseLogging();
                }
            }
        }


        builder.Services.TryAddScoped<IExtractEmbeddingAI, ExtractEmbeddingAI>();
        builder.Services.TryAddScoped<IExtractDataAI, ExtractDataAI>();
    }
}
