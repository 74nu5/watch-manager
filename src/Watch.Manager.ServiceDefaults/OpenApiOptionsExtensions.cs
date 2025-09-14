namespace Watch.Manager.ServiceDefaults;

using System.Text;

using Asp.Versioning.ApiExplorer;

using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc.ApiExplorer;
using Microsoft.AspNetCore.OpenApi;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Primitives;
using Microsoft.OpenApi;

/// <summary>
///     Provides extension methods for configuring OpenAPI options.
/// </summary>
internal static class OpenApiOptionsExtensions
{
    /// <summary>
    ///     Applies API version information to the OpenAPI document.
    /// </summary>
    /// <param name="options">The OpenAPI options.</param>
    /// <param name="title">The API title.</param>
    /// <param name="description">The API description.</param>
    /// <returns>The updated <see cref="OpenApiOptions" />.</returns>
    public static OpenApiOptions ApplyApiVersionInfo(this OpenApiOptions options, string title, string description)
    {
        _ = options.AddDocumentTransformer((document, context, _) =>
        {
            var versionedDescriptionProvider = context.ApplicationServices.GetService<IApiVersionDescriptionProvider>();
            var apiDescription = versionedDescriptionProvider?.ApiVersionDescriptions.SingleOrDefault(desc => desc.GroupName == context.DocumentName);

            if (apiDescription is null)
                return Task.CompletedTask;

            document.Info.Version = apiDescription.ApiVersion.ToString();
            document.Info.Title = title;
            document.Info.Description = BuildDescription(apiDescription, description);
            return Task.CompletedTask;
        });

        return options;
    }

    /// <summary>
    ///     Adds security scheme definitions to the OpenAPI document.
    /// </summary>
    /// <param name="options">The OpenAPI options.</param>
    /// <returns>The updated <see cref="OpenApiOptions" />.</returns>
    public static OpenApiOptions ApplySecuritySchemeDefinitions(this OpenApiOptions options)
    {
        _ = options.AddDocumentTransformer<SecuritySchemeDefinitionsTransformer>();
        return options;
    }

    /// <summary>
    ///     Adds authorization checks and security requirements to OpenAPI operations.
    /// </summary>
    /// <param name="options">The OpenAPI options.</param>
    /// <param name="scopes">The required OAuth2 scopes.</param>
    /// <returns>The updated <see cref="OpenApiOptions" />.</returns>
    public static OpenApiOptions ApplyAuthorizationChecks(this OpenApiOptions options, string[] scopes)
    {
        _ = options.AddOperationTransformer((operation, context, cancellationToken) =>
        {
            var metadata = context.Description.ActionDescriptor.EndpointMetadata;

            if (!metadata.OfType<IAuthorizeData>().Any())
                return Task.CompletedTask;

            operation.Responses ??= [];

            _ = operation.Responses.TryAdd("401", new OpenApiResponse { Description = "Unauthorized" });
            _ = operation.Responses.TryAdd("403", new OpenApiResponse { Description = "Forbidden" });

            var oAuthScheme = new OpenApiSecuritySchemeReference("oauth2")
            {
                Reference = new() { Type = ReferenceType.SecurityScheme },
            };

            operation.Security =
            [
                new()
                {
                    [oAuthScheme] = [.. scopes],
                },
            ];

            return Task.CompletedTask;
        });

        return options;
    }

    /// <summary>
    ///     Marks OpenAPI operations as deprecated if the corresponding API description is deprecated.
    /// </summary>
    /// <param name="options">The OpenAPI options.</param>
    /// <returns>The updated <see cref="OpenApiOptions" />.</returns>
    public static OpenApiOptions ApplyOperationDeprecatedStatus(this OpenApiOptions options)
    {
        _ = options.AddOperationTransformer((operation, context, _) =>
        {
            var apiDescription = context.Description;
            operation.Deprecated |= apiDescription.IsDeprecated();
            return Task.CompletedTask;
        });

        return options;
    }

    /// <summary>
    ///     Adds a description and example to the "api-version" parameter in OpenAPI operations.
    /// </summary>
    /// <param name="options">The OpenAPI options.</param>
    /// <returns>The updated <see cref="OpenApiOptions" />.</returns>
    public static OpenApiOptions ApplyApiVersionDescription(this OpenApiOptions options)
    {
        _ = options.AddOperationTransformer((operation, _, _) =>
        {
            // Find parameter named "api-version" and add a description to it
            operation.Parameters ??= [];
            var apiVersionParameter = operation.Parameters.FirstOrDefault(p => p.Name == "api-version");

            var unused = apiVersionParameter?.Description = "The API version, in the format 'major.minor'.";

            return Task.CompletedTask;
        });

        return options;
    }

    /// <summary>
    ///     Builds the API description, including deprecation and sunset policy information if applicable.
    /// </summary>
    /// <param name="api">The API version description.</param>
    /// <param name="description">The base description.</param>
    /// <returns>The composed description string.</returns>
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

        if (api.SunsetPolicy is not { } policy)
            return text.ToString();

        if (policy.Date is { } when)
        {
            if (text.Length > 0)
                _ = text.Append(' ');

            _ = text.Append("The API will be sunset on ")
                    .Append(when.Date.ToShortDateString())
                    .Append('.');
        }

        if (!policy.HasLinks)
            return text.ToString();

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

        return text.ToString();
    }

    /// <summary>
    ///     Transformer for adding OAuth2 security scheme definitions to the OpenAPI document.
    /// </summary>
    private sealed class SecuritySchemeDefinitionsTransformer(IConfiguration configuration) : IOpenApiDocumentTransformer
    {
        /// <inheritdoc />
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
                        Scopes = scopes!,
                    },
                },
            };

            document.Components ??= new();
            document.Components.SecuritySchemes ??= new Dictionary<string, IOpenApiSecurityScheme>();
            document.Components.SecuritySchemes.Add("oauth2", securityScheme);
            return Task.CompletedTask;
        }
    }
}
