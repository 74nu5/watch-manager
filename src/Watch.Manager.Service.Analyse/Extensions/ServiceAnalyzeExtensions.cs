namespace Watch.Manager.Service.Analyse.Extensions;

using Microsoft.Extensions.AI;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;
using Microsoft.Extensions.Hosting;

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
        /*builder.Services.TryAddTransient<OpenAIClient>(provider =>
        {
            var configuration = provider.GetRequiredService<IConfiguration>();
            var openApiKey = configuration.GetValue("az-open-ai:key", string.Empty);
            var openApiUrl = configuration.GetValue("az-open-ai:url", string.Empty);
            if (string.IsNullOrWhiteSpace(openApiKey))
                throw new InvalidOperationException("OpenAI key is missing");

            if (string.IsNullOrWhiteSpace(openApiUrl))
                throw new InvalidOperationException("OpenAI url is missing");

            return new(new(openApiUrl), new AzureKeyCredential(openApiKey));
        });*/

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

                builder.AddOpenAIClientFromConfiguration("openai");
                _ = builder.Services.AddEmbeddingGenerator(sp => sp.GetRequiredService<OpenAIClient>().AsEmbeddingGenerator(embeddingModel!))
                           .UseOpenTelemetry()
                           .UseLogging();

                var chatModel = builder.Configuration.GetSection("AI").Get<AIOptions>()?.OpenAI?.ChatModel;

                if (!string.IsNullOrWhiteSpace(connectionString) && !string.IsNullOrWhiteSpace(chatModel))
                {
                    builder.AddOpenAIClientFromConfiguration("openai");
                    _ = builder.Services.AddChatClient(sp => sp.GetRequiredService<OpenAIClient>().AsChatClient(chatModel ?? "gpt-4o-mini"))
                               .UseFunctionInvocation()
                               .UseOpenTelemetry(configure: t => t.EnableSensitiveData = true)
                               .UseLogging();
                }
            }
        }


        builder.Services.TryAddScoped<IExtractEmbeddingAI, ExtractEmbeddingAI>();
        builder.Services.TryAddScoped<IExtractDataAI, ExtractDataAI>();
        builder.Services.TryAddTransient<ISiteAnalyzeService, SiteAnalyzeService>();
    }
}
