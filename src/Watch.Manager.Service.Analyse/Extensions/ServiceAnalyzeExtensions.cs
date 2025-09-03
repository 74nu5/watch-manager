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
using Watch.Manager.Service.Analyse.Options;
using Watch.Manager.Service.Analyse.Services;

/// <summary>
///     Provides extension methods for configuring and adding services related to analysis functionality  in an application.
/// </summary>
/// <remarks>
///     This static class includes methods to extend the functionality of <see cref="IConfigurationBuilder" />
///     and <see cref="IHostApplicationBuilder" /> for integrating various services and configurations  such as Azure App
///     Configuration, OpenAI clients, and custom AI services.
/// </remarks>
public static class ServiceAnalyzeExtensions
{
    /// <summary>
    ///     Adds Azure App Configuration to the specified <see cref="IConfigurationBuilder" />.
    /// </summary>
    /// <remarks>
    ///     This method configures the <see cref="IConfigurationBuilder" /> to use a specific Azure App
    ///     Configuration endpoint. Ensure that the connection string used in the configuration is valid and has the
    ///     necessary permissions to access the Azure App Configuration resource.
    /// </remarks>
    /// <param name="configuration">The <see cref="IConfigurationBuilder" /> to which the Azure App Configuration source will be added.</param>
    /// <returns>The <see cref="IConfigurationBuilder" /> with the Azure App Configuration source added.</returns>
    public static IConfigurationBuilder AddAnalyzeConfiguration(this IConfigurationBuilder configuration)
        => configuration.AddAzureAppConfiguration("Endpoint=https://tanus-app-configuration.azconfig.io;Id=f2nS-lw-s0:kdRkJ57qVSe/FshDPeO0;Secret=Kjqm2zGpAxzMbhWasI05KnIWOEJMGn03/yHvCFihplc=");

    /// <summary>
    ///     Configures and registers services required for data analysis and AI-based operations.
    /// </summary>
    /// <remarks>
    ///     This method adds various services to the application's dependency injection container,
    ///     including services for  data sanitization, website processing, and AI-based embedding and chat functionalities.
    ///     The configuration is  determined based on the application's settings, such as whether Ollama or OpenAI services
    ///     are enabled.
    ///         - If Ollama is enabled, the method configures the Ollama API client for embedding and chatfunctionalities.
    ///         - If OpenAI is enabled, the method configures the OpenAI client for embedding and chat functionalities, using models specified in the application's configuration.
    ///     Additionally, this method registers AI services for embedding extraction, data extraction, and article classification.
    /// </remarks>
    /// <param name="builder">The <see cref="IHostApplicationBuilder" /> used to configure the application and its services.</param>
    /// <exception cref="InvalidOperationException">Thrown if the tokenizer for the specified embedding model cannot be found when OpenAI is enabled.</exception>
    public static void AddAnalyzeServices(this IHostApplicationBuilder builder)
    {
        builder.Services.TryAddTransient<SanitizeService>();
        builder.Services.TryAddTransient<SanitizeYoutubeService>();
        builder.Services.TryAddTransient<IWebSiteService, WebSiteService>();

        if (builder.Configuration["OllamaEnabled"] is { } ollamaEnabled && bool.Parse(ollamaEnabled))
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

                builder.Services.TryAddScoped(_ => TokenizerFactory.GetTokenizerForModel(embeddingModel ?? string.Empty) ?? throw new InvalidOperationException("Tokenizer not found"));

                _ = builder.AddOpenAIClientFromConfiguration("openai");
                _ = builder.Services.AddEmbeddingGenerator(sp => sp.GetRequiredService<OpenAIClient>().GetEmbeddingClient(embeddingModel!).AsIEmbeddingGenerator())
                           .UseOpenTelemetry()
                           .UseLogging();

                var chatModel = builder.Configuration.GetSection("AI").Get<AIOptions>()?.OpenAI.ChatModel;

                if (!string.IsNullOrWhiteSpace(connectionString) && !string.IsNullOrWhiteSpace(chatModel))
                {
                    _ = builder.AddOpenAIClientFromConfiguration("openai");
                    _ = builder.Services.AddChatClient(sp => sp.GetRequiredService<OpenAIClient>().GetChatClient(chatModel).AsIChatClient())
                               .UseFunctionInvocation()
                               .UseOpenTelemetry(configure: t => t.EnableSensitiveData = true)
                               .UseLogging();
                }
            }
        }


        builder.Services.TryAddScoped<IExtractEmbeddingAI, ExtractEmbeddingAI>();
        builder.Services.TryAddScoped<IExtractDataAI, ExtractDataAI>();
        builder.Services.TryAddScoped<IArticleClassificationAI, ArticleClassificationAI>();
    }
}
