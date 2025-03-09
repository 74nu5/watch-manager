namespace eShop.ServiceDefaults;

using System.Text;

using Asp.Versioning.ApiExplorer;

using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc.ApiExplorer;
using Microsoft.AspNetCore.OpenApi;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Primitives;
using Microsoft.OpenApi.Any;
using Microsoft.OpenApi.Models;

using Watch.Manager.ServiceDefaults;

internal static class OpenApiOptionsExtensions
{
    public static OpenApiOptions ApplyApiVersionInfo(this OpenApiOptions options, string title, string description)
    {
        _ = options.AddDocumentTransformer((document, context, cancellationToken) =>
        {
            var versionedDescriptionProvider = context.ApplicationServices.GetService<IApiVersionDescriptionProvider>();
            var apiDescription = versionedDescriptionProvider?.ApiVersionDescriptions
                                                              .SingleOrDefault(description => description.GroupName == context.DocumentName);

            if (apiDescription is null)
                return Task.CompletedTask;

            document.Info.Version = apiDescription.ApiVersion.ToString();
            document.Info.Title = title;
            document.Info.Description = BuildDescription(apiDescription, description);
            return Task.CompletedTask;
        });

        return options;
    }

    public static OpenApiOptions ApplySecuritySchemeDefinitions(this OpenApiOptions options)
    {
        _ = options.AddDocumentTransformer<SecuritySchemeDefinitionsTransformer>();
        return options;
    }

    public static OpenApiOptions ApplyAuthorizationChecks(this OpenApiOptions options, string[] scopes)
    {
        _ = options.AddOperationTransformer((operation, context, cancellationToken) =>
        {
            var metadata = context.Description.ActionDescriptor.EndpointMetadata;

            if (!metadata.OfType<IAuthorizeData>().Any())
                return Task.CompletedTask;

            _ = operation.Responses.TryAdd("401", new() { Description = "Unauthorized" });
            _ = operation.Responses.TryAdd("403", new() { Description = "Forbidden" });

            var oAuthScheme = new OpenApiSecurityScheme
            {
                Reference = new() { Type = ReferenceType.SecurityScheme, Id = "oauth2" },
            };

            operation.Security = new List<OpenApiSecurityRequirement>
            {
                new()
                {
                    [oAuthScheme] = scopes,
                },
            };

            return Task.CompletedTask;
        });

        return options;
    }

    public static OpenApiOptions ApplyOperationDeprecatedStatus(this OpenApiOptions options)
    {
        _ = options.AddOperationTransformer((operation, context, cancellationToken) =>
        {
            var apiDescription = context.Description;
            operation.Deprecated |= apiDescription.IsDeprecated();
            return Task.CompletedTask;
        });

        return options;
    }

    public static OpenApiOptions ApplyApiVersionDescription(this OpenApiOptions options)
    {
        _ = options.AddOperationTransformer((operation, context, cancellationToken) =>
        {
            // Find parameter named "api-version" and add a description to it
            var apiVersionParameter = operation.Parameters.FirstOrDefault(p => p.Name == "api-version");

            if (apiVersionParameter is not null)
            {
                apiVersionParameter.Description = "The API version, in the format 'major.minor'.";

                switch (context.DocumentName)
                {
                    case "v1":
                        apiVersionParameter.Schema.Example = new OpenApiString("1.0");
                        break;
                    case "v2":
                        apiVersionParameter.Schema.Example = new OpenApiString("2.0");
                        break;
                }
            }

            return Task.CompletedTask;
        });

        return options;
    }

    // This extension method adds a schema transformer that sets "nullable" to false for all optional properties.
    public static OpenApiOptions ApplySchemaNullableFalse(this OpenApiOptions options)
    {
        _ = options.AddSchemaTransformer((schema, context, cancellationToken) =>
        {
            if (schema.Properties is not null)
            {
                foreach (var property in schema.Properties)
                {
                    if (schema.Required?.Contains(property.Key) != true)
                        property.Value.Nullable = false;
                }
            }

            return Task.CompletedTask;
        });

        return options;
    }

    private static string BuildDescription(ApiVersionDescription api, string description)
    {
        var text = new StringBuilder(description);

        if (api.IsDeprecated)
        {
            if (text.Length > 0)
            {
                if (text[^1] != '.')
                    _ = text.Append('.');

                _ = text.Append(' ');
            }

            _ = text.Append("This API version has been deprecated.");
        }

        if (api.SunsetPolicy is { } policy)
        {
            if (policy.Date is { } when)
            {
                if (text.Length > 0)
                    _ = text.Append(' ');

                _ = text.Append("The API will be sunset on ")
                    .Append(when.Date.ToShortDateString())
                    .Append('.');
            }

            if (policy.HasLinks)
            {
                _ = text.AppendLine();

                var rendered = false;

                foreach (var link in policy.Links.Where(l => l.Type == "text/html"))
                {
                    if (!rendered)
                    {
                        _ = text.Append("<h4>Links</h4><ul>");
                        rendered = true;
                    }

                    _ = text.Append("<li><a href=\"");
                    _ = text.Append(link.LinkTarget.OriginalString);
                    _ = text.Append("\">");
                    _ = text.Append(
                        StringSegment.IsNullOrEmpty(link.Title)
                                ? link.LinkTarget.OriginalString
                                : link.Title.ToString());

                    _ = text.Append("</a></li>");
                }

                if (rendered)
                    _ = text.Append("</ul>");
            }
        }

        return text.ToString();
    }

    private class SecuritySchemeDefinitionsTransformer(IConfiguration configuration) : IOpenApiDocumentTransformer
    {
        public Task TransformAsync(OpenApiDocument document, OpenApiDocumentTransformerContext context, CancellationToken cancellationToken)
        {
            var identitySection = configuration.GetSection("Identity");
            if (!identitySection.Exists())
                return Task.CompletedTask;

            var identityUrlExternal = identitySection.GetRequiredValue("Url");
            var scopes = identitySection.GetRequiredSection("Scopes").GetChildren().ToDictionary(p => p.Key, p => p.Value);
            var securityScheme = new OpenApiSecurityScheme
            {
                Type = SecuritySchemeType.OAuth2,
                Flows = new()
                {
                    // TODO: Change this to use Authorization Code flow with PKCE
                    Implicit = new()
                    {
                        AuthorizationUrl = new($"{identityUrlExternal}/connect/authorize"),
                        TokenUrl = new($"{identityUrlExternal}/connect/token"),
                        Scopes = scopes,
                    },
                },
            };

            document.Components ??= new();
            document.Components.SecuritySchemes.Add("oauth2", securityScheme);
            return Task.CompletedTask;
        }
    }
}
