# Instructions pour les agents IA - Watch Manager

## Architecture et contexte

**Watch Manager** est un outil d'accompagnement intelligent pour la veille technique utilisant .NET 9 et Aspire. L'application analyse automatiquement des articles web, extrait des métadonnées via IA, et stocke le contenu avec des embeddings vectoriels pour la recherche sémantique.

### Structure modulaire par domaine

```
Watch.Manager.ApiService/        → API RESTful (endpoints articles/catégories)
Watch.Manager.Web/              → Interface Blazor Server avec FluentUI
Watch.Manager.Service.Analyse/   → Services IA (extraction, embeddings, classification)
Watch.Manager.Service.Database/  → Couche données EF Core + SQL Server Vector Search
Watch.Manager.AppHost/           → Orchestrateur Aspire
Watch.Manager.ServiceDefaults/   → Configuration partagée (OpenTelemetry, health checks)
```

## Patterns et conventions critiques

### Gestion centralisée des packages
- **Central Package Management** : Toutes les versions dans `Directory.Packages.props`
- **Aspire coordination** : Versions groupées par commentaires (`<!-- Version together with Aspire -->`)
- Mise à jour : `Update-NugetPackages.ps1`

### Architecture Minimal APIs avec versioning
```csharp
// Pattern endpoints dans Watch.Manager.ApiService/Extensions/
var vApi = app.NewVersionedApi("Articles");
var api = vApi.MapGroup("api/articles");
api.MapPost("/save", SaveAndAnalyzeArticleAsync)
   .WithName("SaveAndAnalyzeArticle")
   .WithSummary("Description pour Scalar");
```

### Injection par paramètres structurés
```csharp
// Pattern standard pour les endpoints avec [AsParameters]
private static async Task<IResult> SaveArticleAsync(
    [AsParameters] AnalyzeParameter parameters,
    CancellationToken cancellationToken)

// Structure record avec FromServices et FromBody
public record AnalyzeParameter
{
    [FromBody] public required AnalyzeModel AnalyzeModel { get; set; }
    [FromServices] public required IWebSiteService WebSiteService { get; set; }
    [FromServices] public required ILogger<AnalyzeParameter> Logger { get; set; }
}
```

### Gestion d'erreurs et exceptions
```csharp
// Pattern try-catch avec logging structuré dans endpoints
try
{
    var result = await analyzeParameter.Service.ProcessAsync(cancellationToken).ConfigureAwait(false);
    return Results.Ok(result);
}
catch (HttpRequestException httpEx) when (httpEx.StatusCode == HttpStatusCode.NotFound)
{
    analyzeParameter.Logger.LogWarning(httpEx, "Resource not found for {Url}", url);
    return Results.NotFound("Resource not found");
}
catch (Exception ex)
{
    analyzeParameter.Logger.LogError(ex, "Failed to process request");
    return Results.InternalServerError(ex);
}
```

### Configuration Aspire orchestrée
```csharp
// AppHost/Program.cs - Dependencies explicites avec WaitFor()
var apiService = builder.AddProject<Watch_Manager_ApiService>("apiservice")
                        .WithReference(articlesDb)
                        .WaitFor(migrations);
```

## Développement et débogage

### Démarrage local
```bash
# Démarrage complet via Aspire (recommandé)
dotnet run --project src/Watch.Manager.AppHost

# Le dashboard Aspire s'ouvre automatiquement
# SQL Server : localhost:1434 / articlesdb / Password1234
```

### Tests et validation
```bash
# Tests unitaires
dotnet test

# Verification StyleCop (configuration dans stylecop.json)
# Warnings as Errors désactivé dans Directory.Build.props
```

### Base de données et migrations
- **EF Core** avec SQL Server Vector Search (`EFCore.SqlServer.VectorSearch`)
- **Embeddings** : colonnes `vector(1536)` pour recherche sémantique
- **Migrations** : service dédié `Watch.Manager.Service.Migrations`
- Relations many-to-many Article-Category avec scores de confiance

## Services IA et analyse

### Pipeline d'analyse d'articles
1. **SanitizeService** : Nettoyage HTML avec AngleSharp
2. **ExtractDataAi** : Extraction titre/résumé/tags via Microsoft.Extensions.AI
3. **Embeddings** : Génération vectors (tête + corps) pour recherche
4. **Classification automatique** : Assignment catégories avec seuils configurables

### Configuration IA flexible
```csharp
// AppHost - basculement OpenAI/Ollama
var useOpenAI = true;  // Toggle selon environnement
if (useOpenAI) builder.AddOpenAI(apiService);
if (useOllama) builder.AddOllama(apiService);
```

## Technologies et intégrations clés

- **Aspire** : Orchestration, observabilité, service discovery
- **FluentUI** : Composants interface (`Microsoft.FluentUI.AspNetCore.Components`)
- **Scalar** : Documentation API auto-générée avec thème Purple
- **Vector Search** : Recherche sémantique dans SQL Server
- **OpenTelemetry** : Traces, métriques, logs configurés par défaut

## Bonnes pratiques de développement

### Sécurité et robustesse
- **Null safety** : `required` properties sur entités, nullable context activé
- **Sealed classes** : Toutes les classes implémentées comme `sealed` par défaut
- **ConfigureAwait(false)** : Systématiquement utilisé sur tous les appels async
- **Validation précoce** : `throw new InvalidOperationException` avec messages explicites
- **CancellationToken** : Propagé dans toutes les méthodes async avec `= default`

### Code quality et style
- **File-scoped namespaces** : Obligatoire (`csharp_style_namespace_declarations = file_scoped:warning`)
- **Expression-bodied members** : Préférés quand approprié
- **Pattern matching** : Utilisation des patterns C# modernes
- **var inference** : `var` préféré quand le type est évident
- **Primary constructors** : Utilisés pour l'injection de dépendances

### Performance et observabilité
- **Logging structuré** : Templates avec paramètres, jamais de string interpolation
- **OpenTelemetry** : Traces, métriques et logs configurés automatiquement
- **IAsyncEnumerable** : Pour les endpoints retournant des collections (ex: search)
- **Resilience** : HTTP clients avec retry policies (temporairement désactivé)

### Documentation et maintenance
- **XML documentation** : Obligatoire pour toutes les API publiques EN ANGLAIS
- **StyleCop** : Configuration stricte avec suppressions ciblées
- **EditorConfig** : Formatage uniforme, UTF-8 BOM, CRLF sur Windows
- **CORS policies** : Séparées pour développement local (7020) et production Azure

## Conventions de nommage et organisation

### Structure des projets
- Services par domaine : `Watch.Manager.Service.[Domaine]`
- Endpoints : `[Entity]Endpoints.cs` avec méthodes statiques
- ViewModels : Suffixe `ViewModel` pour API responses
- Parameters : Suffixe `Parameter` pour [AsParameters] injection
- Extensions : `[Domain]Extensions.cs` pour méthodes d'extension

### Formatage et style
- **UTF-8 BOM** : Encodage obligatoire pour tous les fichiers
- **CRLF** : Fin de ligne Windows dans .editorconfig
- **Indentation** : 4 espaces, pas de tabs
- **Final newline** : Obligatoire en fin de fichier
- **Trailing whitespace** : Automatiquement supprimé

---

Pour contribuer efficacement, respectez l'architecture modulaire, utilisez les patterns de versioning d'API établis, et testez via le dashboard Aspire pour la cohérence des services.
