namespace Watch.Manager.Web.Services.Extensions;

using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;
using Microsoft.Extensions.Hosting;

public static class WebServicesExtensions
{
    public static void AddWebServices(this IHostApplicationBuilder builder)
    {
        builder.Services.AddHttpClient();
        builder.Configuration.AddAzureAppConfiguration("Endpoint=https://tanus-app-configuration.azconfig.io;Id=f2nS-lw-s0:kdRkJ57qVSe/FshDPeO0;Secret=Kjqm2zGpAxzMbhWasI05KnIWOEJMGn03/yHvCFihplc=");
        builder.Services.TryAddTransient<AnalyzeService>();
    }
}
