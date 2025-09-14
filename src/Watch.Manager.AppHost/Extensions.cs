namespace Watch.Manager.AppHost;

using Aspire.Hosting.Lifecycle;

using JetBrains.Annotations;

using Microsoft.Extensions.Configuration;

/// <summary>
///     Provides extension methods for configuring distributed applications with additional services and features.
/// </summary>
/// <remarks>
///     This static class includes methods to configure distributed applications with features such as
///     forwarded headers, OpenAI integration, and Ollama integration. These methods extend the functionality of the
///     <see
///         cref="IDistributedApplicationBuilder" />
///     interface, enabling developers to easily add and configure services  for
///     their distributed application projects.
/// </remarks>
internal static class Extensions
{
    /// <summary>
    ///     Adds a hook to set the ASPNETCORE_FORWARDEDHEADERS_ENABLED environment variable to true for all projects in the application.
    /// </summary>
    /// <param name="builder">The <see cref="IDistributedApplicationBuilder" /> used to configure the application and its services.</param>
    /// <returns>The <see cref="IDistributedApplicationBuilder" /> with Ollama services added.</returns>
    public static IDistributedApplicationBuilder AddForwardedHeaders(this IDistributedApplicationBuilder builder)
    {
        builder.Services.TryAddLifecycleHook<AddForwardHeadersHook>();
        return builder;
    }

    /// <summary>
    ///     Configures eShop projects to use OpenAI for text embedding and chat.
    /// </summary>
    /// <param name="builder">The <see cref="IDistributedApplicationBuilder" /> used to configure the application and its services.</param>
    /// <param name="servicesApi">The <see cref="IResourceBuilder{ProjectResource}" /> representing the services API project to which OpenAI models will be added.</param>
    /// <returns>The <see cref="IDistributedApplicationBuilder" /> with Ollama services added.</returns>
    public static IDistributedApplicationBuilder AddOpenAI(this IDistributedApplicationBuilder builder, IResourceBuilder<ProjectResource> servicesApi)
    {
        const string OpenAIName = "openai";
        const string TextEmbeddingModelName = "text-embedding-3-small";
        const string ChatModelName = "model-dev-gpt-4.1-mini";

        var openAI = BuildOpenAI(builder, OpenAIName, ChatModelName, TextEmbeddingModelName);

        _ = servicesApi
           .WithReference(openAI)
           .WithEnvironment("AI__OPENAI__EMBEDDINGMODEL", TextEmbeddingModelName)
           .WithEnvironment("AZURE__OPENAI__EMBEDDINGMODEL", TextEmbeddingModelName);

        _ = servicesApi
           .WithReference(openAI)
           .WithEnvironment("AI__OPENAI__CHATMODEL", ChatModelName)
           .WithEnvironment("AZURE__OPENAI__CHATMODEL", ChatModelName);

        return builder;
    }

    /// <summary>
    ///     Configures eShop projects to use Ollama for text embedding and chat.
    /// </summary>
    /// <param name="builder">The <see cref="IDistributedApplicationBuilder" /> used to configure the application and its services.</param>
    /// <param name="catalogApi">The <see cref="IResourceBuilder{ProjectResource}" /> representing the catalog API project to which Ollama models will be added.</param>
    /// <returns>The <see cref="IDistributedApplicationBuilder" /> with Ollama services added.</returns>
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

    private static IResourceBuilder<IResourceWithConnectionString> BuildOpenAI(IDistributedApplicationBuilder builder, string openAIName, string chatModelName, string textEmbeddingModelName)
    {
        // to use an existing OpenAI resource as a connection string, add the following to the AppHost user secrets:
        // "ConnectionStrings": {
        //   "openai": "Key=<API Key>" (to use https://api.openai.com/)
        //     -or-
        //   "openai": "Endpoint=https://<name>.openai.azure.com/" (to use Azure OpenAI)
        // }
        if (builder.Configuration.GetConnectionString(openAIName) is not null)
            return builder.AddConnectionString(openAIName);

        // to use Azure provisioning, add the following to the AppHost user secrets:
        // "Azure": {
        //   "SubscriptionId": "<your subscription ID>",
        //   "ResourceGroupPrefix": "<prefix>",
        //   "Location": "<location>"
        // }
        var openAITyped = builder.AddAzureOpenAI(openAIName);

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
                builder.AddParameter("openAIName"),
                builder.AddParameter("openaiResourceGroup"));
        }

        /*_ = openAITyped
           .AddDeployment(new(chatModelName, "gpt-4o-mini", "2024-07-18"))
           .AddDeployment(new(textEmbeddingModelName, "text-embedding-3-small", "1", skuCapacity: 20)); // 20k tokens per minute are needed to seed the initial embeddings*/

        _ = openAITyped
               .AddDeployment(chatModelName, "gpt-4.1-mini", "2025-04-14");
        _ = openAITyped
               .AddDeployment(textEmbeddingModelName, "text-embedding-3-small", "1"); // 20k tokens per minute are needed to seed the initial embeddings

        return openAITyped;
    }

    [UsedImplicitly]
    private class AddForwardHeadersHook : IDistributedApplicationLifecycleHook
    {
        public Task BeforeStartAsync(DistributedApplicationModel appModel, CancellationToken cancellationToken = default)
        {
            foreach (var p in appModel.GetProjectResources())
                p.Annotations.Add(new EnvironmentCallbackAnnotation(context => { context.EnvironmentVariables["ASPNETCORE_FORWARDEDHEADERS_ENABLED"] = "true"; }));

            return Task.CompletedTask;
        }
    }
}
