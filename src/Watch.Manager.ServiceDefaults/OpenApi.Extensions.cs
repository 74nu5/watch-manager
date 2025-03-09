namespace Watch.Manager.ServiceDefaults;

using Asp.Versioning;

using eShop.ServiceDefaults;

using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;

using Scalar.AspNetCore;

public static partial class Extensions
{
    public static IApplicationBuilder UseDefaultOpenApi(this WebApplication app)
    {
        var configuration = app.Configuration;
        var openApiSection = configuration.GetSection("OpenApi");

        if (!openApiSection.Exists())
            return app;

        _ = app.MapOpenApi();

        if (app.Environment.IsDevelopment())
        {
            _ = app.MapScalarApiReference(options =>
            {
                // Disable default fonts to avoid download unnecessary fonts
                options.DefaultFonts = false;
            });

            _ = app.MapGet("/", () => Results.Redirect("/scalar/v1")).ExcludeFromDescription();
        }

        return app;
    }

    public static IHostApplicationBuilder AddDefaultOpenApi(this IHostApplicationBuilder builder,
                                                            IApiVersioningBuilder? apiVersioning = default)
    {
        var openApi = builder.Configuration.GetSection("OpenApi");
        var identitySection = builder.Configuration.GetSection("Identity");

        var scopes = identitySection.Exists()
                             ? identitySection.GetRequiredSection("Scopes").GetChildren().ToDictionary(p => p.Key, p => p.Value)
                             : new();


        if (!openApi.Exists())
            return builder;

        if (apiVersioning is not null)
        {
            // the default format will just be ApiVersion.ToString(); for example, 1.0.
            // this will format the version as "'v'major[.minor][-status]"
            var versioned = apiVersioning.AddApiExplorer(options => options.GroupNameFormat = "'v'VVV");
            string[] versions = ["v1", "v2"];

            foreach (var description in versions)
            {
                _ = builder.Services.AddOpenApi(description,
                    options =>
                    {
                        _ = options.ApplyApiVersionInfo(openApi.GetRequiredValue("Document:Title"), openApi.GetRequiredValue("Document:Description"));
                        _ = options.ApplyAuthorizationChecks([.. scopes.Keys]);
                        _ = options.ApplySecuritySchemeDefinitions();
                        _ = options.ApplyOperationDeprecatedStatus();
                        _ = options.ApplyApiVersionDescription();
                        _ = options.ApplySchemaNullableFalse();
                        // Clear out the default servers so we can fallback to
                        // whatever ports have been allocated for the service by Aspire
                        _ = options.AddDocumentTransformer((document, context, cancellationToken) =>
                        {
                            document.Servers = [];
                            return Task.CompletedTask;
                        });
                    });
            }
        }

        return builder;
    }
}
