namespace Watch.Manager.ServiceDefaults;

using Asp.Versioning;

using Microsoft.AspNetCore.Builder;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;

/// <summary>
///     Provides extension methods for configuring and using OpenAPI in the application.
/// </summary>
public static partial class Extensions
{
    /// <summary>
    ///     Configures the application to use the default OpenAPI endpoints if the "OpenApi" section exists in the configuration.
    /// </summary>
    /// <param name="app">The <see cref="WebApplication" /> instance.</param>
    /// <returns>The <see cref="IApplicationBuilder" /> for chaining.</returns>
    public static IApplicationBuilder UseDefaultOpenApi(this WebApplication app)
    {
        var configuration = app.Configuration;
        var openApiSection = configuration.GetSection("OpenApi");

        if (!openApiSection.Exists())
            return app;

        _ = app.MapOpenApi();
        return app;
    }

    /// <summary>
    ///     Adds and configures the default OpenAPI services and documentation for the application if the "OpenApi" section exists in the configuration.
    /// </summary>
    /// <param name="builder">The <see cref="IHostApplicationBuilder" /> instance.</param>
    /// <param name="apiVersioning">The optional <see cref="IApiVersioningBuilder" /> for API versioning support.</param>
    /// <returns>The <see cref="IHostApplicationBuilder" /> for chaining.</returns>
    public static IHostApplicationBuilder AddDefaultOpenApi(this IHostApplicationBuilder builder, IApiVersioningBuilder? apiVersioning = null)
    {
        var openApi = builder.Configuration.GetSection("OpenApi");
        var identitySection = builder.Configuration.GetSection("Identity");

        var scopes = identitySection.Exists()
                             ? identitySection.GetRequiredSection("Scopes").GetChildren().ToDictionary(p => p.Key, p => p.Value)
                             : [];

        if (!openApi.Exists() || apiVersioning is null)
            return builder;

        // the default format will just be ApiVersion.ToString(); for example, 1.0.
        // this will format the version as "'v'major[.minor][-status]"
        _ = apiVersioning.AddApiExplorer(options => options.GroupNameFormat = "'v'VVV");
        string[] versions = ["v1", "v2"];

        foreach (var description in versions)
        {
            _ = builder.Services.AddOpenApi(
                description,
                options =>
                {
                    _ = options.ApplyApiVersionInfo(openApi.GetRequiredValue("Document:Title"), openApi.GetRequiredValue("Document:Description"));
                    _ = options.ApplyAuthorizationChecks([.. scopes.Keys]);
                    _ = options.ApplySecuritySchemeDefinitions();
                    _ = options.ApplyOperationDeprecatedStatus();
                    _ = options.ApplyApiVersionDescription();

                    // _ = options.ApplySchemaNullableFalse();

                    // Clear out the default servers so we can fallback to
                    // whatever ports have been allocated for the service by Aspire
                    _ = options.AddDocumentTransformer((document, _, _) =>
                    {
                        document.Servers = [];
                        return Task.CompletedTask;
                    });
                });
        }

        return builder;
    }
}
