namespace Watch.Manager.AppHost;

using Aspire.Hosting;
using Aspire.Hosting.Lifecycle;

using Microsoft.Extensions.Configuration;

internal static class Extensions
{
    /// <summary>
    ///     Adds a hook to set the ASPNETCORE_FORWARDEDHEADERS_ENABLED environment variable to true for all projects in the application.
    /// </summary>
    public static IDistributedApplicationBuilder AddForwardedHeaders(this IDistributedApplicationBuilder builder)
    {
        builder.Services.TryAddLifecycleHook<AddForwardHeadersHook>();
        return builder;
    }

    /// <summary>
    ///     Configures eShop projects to use OpenAI for text embedding and chat.
    /// </summary>
    public static IDistributedApplicationBuilder AddOpenAI(this IDistributedApplicationBuilder builder, IResourceBuilder<ProjectResource> servicesApi)
    {
        const string OpenAIName = "openai";
        const string TextEmbeddingModelName = "text-embedding-3-small";
        const string ChatModelName = "gpt-4o-mini";

        // to use an existing OpenAI resource as a connection string, add the following to the AppHost user secrets:
        // "ConnectionStrings": {
        //   "openai": "Key=<API Key>" (to use https://api.openai.com/)
        //     -or-
        //   "openai": "Endpoint=https://<name>.openai.azure.com/" (to use Azure OpenAI)
        // }
        IResourceBuilder<IResourceWithConnectionString> openAI;

        if (builder.Configuration.GetConnectionString(OpenAIName) is not null)
        {
            openAI = builder.AddConnectionString(OpenAIName);
        }
        else
        {
            // to use Azure provisioning, add the following to the AppHost user secrets:
            // "Azure": {
            //   "SubscriptionId": "<your subscription ID>",
            //   "ResourceGroupPrefix": "<prefix>",
            //   "Location": "<location>"
            // }

            var openAITyped = builder.AddAzureOpenAI(OpenAIName);

            // to use an existing Azure OpenAI resource via provisioning, add the following to the AppHost user secrets:
            // "Parameters": {
            //   "openaiName": "<Azure OpenAI resource name>",
            //   "openaiResourceGroup": "<Azure OpenAI resource group>"
            // }
            // - or -
            // leave the parameters out to create a new Azure OpenAI resource
            if (builder.Configuration["Parameters:openaiName"] is not null &&
                builder.Configuration["Parameters:openaiResourceGroup"] is not null)
            {
                _ = openAITyped.AsExisting(
                    builder.AddParameter("openaiName"),
                    builder.AddParameter("openaiResourceGroup"));
            }

            _ = openAITyped
               .AddDeployment(new(ChatModelName, "gpt-4o-mini", "2024-07-18"))
               .AddDeployment(new(TextEmbeddingModelName, "text-embedding-3-small", "1", skuCapacity: 20)); // 20k tokens per minute are needed to seed the initial embeddings

            openAI = openAITyped;
        }

        _ = servicesApi
           .WithReference(openAI)
           .WithEnvironment("AI__OPENAI__EMBEDDINGMODEL", TextEmbeddingModelName);


        servicesApi
               .WithReference(openAI)
               .WithEnvironment("AI__OPENAI__CHATMODEL", ChatModelName);

        return builder;
    }

    /// <summary>
    ///     Configures eShop projects to use Ollama for text embedding and chat.
    /// </summary>
    public static IDistributedApplicationBuilder AddOllama(this IDistributedApplicationBuilder builder, IResourceBuilder<ProjectResource> catalogApi)
    {
        var ollama = builder.AddOllama("ollama")
                            .WithImageTag("latest")
                            .WithDataVolume()

                            // 6950 XT drivers are not available in linux yet, so we need to use CPU for now
                            // .WithGPUSupport(OllamaGpuVendor.AMD)
                            .WithOpenWebUI()
                           .WithEnvironment("OLLAMA_TIMEOUT", 500.ToString());

        var embeddings = ollama.AddModel("embedding", "all-minilm");
        var chat = ollama.AddModel("chat", "llama3.1");

        _ = catalogApi.WithReference(embeddings)
                      .WithEnvironment("OllamaEnabled", "true")
                      .WaitFor(embeddings);

        _ = catalogApi.WithReference(chat)
                      .WithEnvironment("OllamaEnabled", "true")
                      .WaitFor(chat);

        return builder;
    }

    private class AddForwardHeadersHook : IDistributedApplicationLifecycleHook
    {
        public Task BeforeStartAsync(DistributedApplicationModel appModel, CancellationToken cancellationToken = default)
        {
            foreach (var p in appModel.GetProjectResources())
            {
                p.Annotations.Add(new EnvironmentCallbackAnnotation(context => { context.EnvironmentVariables["ASPNETCORE_FORWARDEDHEADERS_ENABLED"] = "true"; }));
            }

            return Task.CompletedTask;
        }
    }
}
