namespace Watch.Manager.Service.Analyse.Extensions;

using Azure;
using Azure.AI.OpenAI;

using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;

using Watch.Manager.Service.Analyse.Abstractions;

public static class ServiceAnalyzeExtensions
{
    public static IConfigurationBuilder AddAnalyzeConfiguration(this IConfigurationBuilder configuration)
        => configuration.AddAzureAppConfiguration("Endpoint=https://tanus-app-configuration.azconfig.io;Id=f2nS-lw-s0:kdRkJ57qVSe/FshDPeO0;Secret=Kjqm2zGpAxzMbhWasI05KnIWOEJMGn03/yHvCFihplc=");

    public static void AddAnalyzeServices(this IServiceCollection services)
    {
        services.TryAddTransient<SanitizeService>();
        services.TryAddTransient<IWebSiteService, WebSiteService>();
        services.TryAddTransient<OpenAIClient>(provider =>
        {
            var configuration = provider.GetRequiredService<IConfiguration>();
            var openApiKey = configuration.GetValue("az-open-ai:key", string.Empty);
            var openApiUrl = configuration.GetValue("az-open-ai:url", string.Empty);
            if (string.IsNullOrWhiteSpace(openApiKey))
                throw new InvalidOperationException("OpenAI key is missing");

            if (string.IsNullOrWhiteSpace(openApiUrl))
                throw new InvalidOperationException("OpenAI url is missing");

            return new(new(openApiUrl), new AzureKeyCredential(openApiKey));
        });

        services.TryAddTransient<ISiteAnalyzeService, SiteAnalyzeService>();
    }
}
