# US-001 : Ajout d'un flux RSS

## 📝 Description

**En tant que** utilisateur de Watch Manager  
**Je veux** ajouter un flux RSS en saisissant son URL  
**Afin de** suivre automatiquement les nouveaux articles de cette source

## 🎯 Critères d'acceptation

### Fonctionnalités principales
- [ ] **GIVEN** je suis sur la page de gestion des flux RSS  
      **WHEN** je clique sur le bouton "Ajouter un flux"  
      **THEN** un dialogue s'ouvre avec un champ URL et un bouton "Tester le flux"

- [ ] **GIVEN** j'ai saisi une URL RSS valide (ex: https://devblogs.microsoft.com/dotnet/feed/)  
      **WHEN** je clique sur "Tester le flux"  
      **THEN** le système valide le flux et affiche le titre, la description et le nombre d'articles

- [ ] **GIVEN** le flux a été validé avec succès  
      **WHEN** je clique sur "Ajouter le flux"  
      **THEN** le flux est enregistré et apparaît dans ma liste avec le statut "Actif"

### Comportements de validation
- [ ] **FORMAT URL** : Accepte les URLs HTTP et HTTPS uniquement
- [ ] **TIMEOUT** : Affiche une erreur si le flux ne répond pas en 10 secondes
- [ ] **FORMATS SUPPORTÉS** : Détecte automatiquement RSS 2.0, Atom 1.0, RSS 1.0 (RDF)
- [ ] **AUTO-DÉCOUVERTE** : Si l'URL est une page web, tente de détecter le flux RSS associé
- [ ] **DOUBLONS** : Empêche l'ajout d'un flux déjà présent (basé sur l'URL)

### Critères techniques
- [ ] **Performance** : Validation et ajout en <2 secondes pour un flux standard
- [ ] **Erreurs** : Messages clairs selon le type d'erreur (404, timeout, format invalide, etc.)
- [ ] **Persistance** : Le flux est sauvegardé en base avec toutes les métadonnées extraites

## 🎨 Maquettes

### Dialogue d'ajout - État initial
```
┌─────────────────────────────────────────────────────────────┐
│ ➕ Ajouter un flux RSS                            [✕]       │
├─────────────────────────────────────────────────────────────┤
│                                                              │
│ URL du flux RSS *                                            │
│ [                                                        ]    │
│ 💡 Ex: https://devblogs.microsoft.com/dotnet/feed/         │
│                                                              │
│ 🔍 [Tester le flux]                                         │
│                                                              │
│               [Annuler]  [Ajouter le flux] (désactivé)      │
└─────────────────────────────────────────────────────────────┘
```

### Dialogue d'ajout - Validation en cours
```
┌─────────────────────────────────────────────────────────────┐
│ ➕ Ajouter un flux RSS                            [✕]       │
├─────────────────────────────────────────────────────────────┤
│                                                              │
│ URL du flux RSS *                                            │
│ [https://devblogs.microsoft.com/dotnet/feed/            ]    │
│                                                              │
│ ⏳ Validation en cours...                                   │
│ [████████░░░░░░░░]                                          │
│                                                              │
│               [Annuler]  [Ajouter le flux] (désactivé)      │
└─────────────────────────────────────────────────────────────┘
```

### Dialogue d'ajout - Flux valide détecté
```
┌─────────────────────────────────────────────────────────────┐
│ ➕ Ajouter un flux RSS                            [✕]       │
├─────────────────────────────────────────────────────────────┤
│                                                              │
│ URL du flux RSS *                                            │
│ [https://devblogs.microsoft.com/dotnet/feed/            ]    │
│ 🔍 [Retester]                                               │
│                                                              │
│ ✅ Flux RSS 2.0 valide détecté!                             │
│ ┌──────────────────────────────────────────────────────┐    │
│ │ 📰 Titre: .NET Blog                                  │    │
│ │ 📝 Description: The official .NET team blog from     │    │
│ │    Microsoft, covering .NET news, tips and tricks... │    │
│ │ 📊 25 articles trouvés (dernier: il y a 2 jours)    │    │
│ └──────────────────────────────────────────────────────┘    │
│                                                              │
│ Nom du flux (optionnel)                                     │
│ [.NET Blog Official                                     ]    │
│                                                              │
│ Catégorie                                                    │
│ [.NET & C# ▼]                                               │
│                                                              │
│ Fréquence de synchronisation                                │
│ ⚪ Toutes les heures  ⚫ Toutes les 4h  ⚪ Quotidien         │
│                                                              │
│               [Annuler]  [Ajouter le flux]                  │
└─────────────────────────────────────────────────────────────┘
```

### Dialogue d'ajout - Erreur
```
┌─────────────────────────────────────────────────────────────┐
│ ➕ Ajouter un flux RSS                            [✕]       │
├─────────────────────────────────────────────────────────────┤
│                                                              │
│ URL du flux RSS *                                            │
│ [https://invalid-url.com/feed.xml                       ]    │
│ 🔍 [Retester]                                               │
│                                                              │
│ ❌ Erreur: Impossible de lire le flux                       │
│ ┌──────────────────────────────────────────────────────┐    │
│ │ Code: 404 Not Found                                  │    │
│ │ Le flux RSS n'existe pas ou n'est plus accessible.  │    │
│ │                                                       │    │
│ │ 💡 Vérifiez l'URL ou essayez de trouver le flux     │    │
│ │    RSS sur le site web.                              │    │
│ └──────────────────────────────────────────────────────┘    │
│                                                              │
│               [Annuler]  [Ajouter le flux] (désactivé)      │
└─────────────────────────────────────────────────────────────┘
```

## 🧪 Scénarios de test

### Test 1 : Ajout d'un flux RSS 2.0 standard
```gherkin
Given je suis sur la page de gestion des flux RSS
And je clique sur "Ajouter un flux"
When je saisis "https://devblogs.microsoft.com/dotnet/feed/"
And je clique sur "Tester le flux"
Then le système affiche "✅ Flux RSS 2.0 valide détecté!"
And le titre ".NET Blog" est affiché
And le nombre d'articles "25 articles trouvés" est affiché
When je clique sur "Ajouter le flux"
Then le flux apparaît dans ma liste avec le statut "Actif"
And une notification de succès s'affiche
```

### Test 2 : Ajout d'un flux Atom
```gherkin
Given je suis sur le dialogue d'ajout de flux
When je saisis "https://github.com/dotnet/runtime/commits/main.atom"
And je clique sur "Tester le flux"
Then le système affiche "✅ Flux Atom 1.0 valide détecté!"
And le titre "Recent Commits to runtime:main" est affiché
When je clique sur "Ajouter le flux"
Then le flux est enregistré avec le format "Atom"
```

### Test 3 : Gestion d'erreur - URL invalide
```gherkin
Given je suis sur le dialogue d'ajout de flux
When je saisis "https://invalid-domain-xyz123.com/feed"
And je clique sur "Tester le flux"
Then le système affiche "❌ Erreur: Impossible de lire le flux"
And le message d'erreur contient "The remote name could not be resolved"
And le bouton "Ajouter le flux" reste désactivé
```

### Test 4 : Auto-découverte depuis une page web
```gherkin
Given je suis sur le dialogue d'ajout de flux
When je saisis "https://devblogs.microsoft.com/dotnet/"
And je clique sur "Tester le flux"
Then le système détecte automatiquement le flux RSS dans le HTML
And affiche "✅ Flux RSS découvert automatiquement"
And l'URL est mise à jour vers "https://devblogs.microsoft.com/dotnet/feed/"
```

### Test 5 : Détection de doublon
```gherkin
Given j'ai déjà un flux "https://devblogs.microsoft.com/dotnet/feed/"
When je tente d'ajouter à nouveau la même URL
And je clique sur "Tester le flux"
Then le système affiche "⚠️ Ce flux est déjà dans votre liste"
And propose de naviguer vers le flux existant
```

## 🔧 Spécifications techniques

### Modèle de données
```csharp
public class RssFeed
{
    public int Id { get; set; }
    public required string Url { get; set; }
    public required string Title { get; set; }
    public string? Description { get; set; }
    public string? FaviconUrl { get; set; }
    public RssFeedType Type { get; set; } // RSS20, Atom10, RSS10
    public RssFeedStatus Status { get; set; } // Active, Error, Paused
    public DateTime CreatedAt { get; set; }
    public DateTime? LastSyncAt { get; set; }
    public DateTime? LastSuccessfulSyncAt { get; set; }
    public string? LastErrorMessage { get; set; }
    public int SyncFrequencyMinutes { get; set; } = 240; // 4 heures par défaut
    public int? CategoryId { get; set; }
    public Category? Category { get; set; }
    public ICollection<RssFeedItem> Items { get; set; } = [];
}

public enum RssFeedType
{
    RSS20,
    Atom10,
    RSS10
}

public enum RssFeedStatus
{
    Active,
    Error,
    Paused
}
```

### API Contract
```http
POST /api/v1/rss-feeds/validate
Authorization: Bearer {token}
Content-Type: application/json

Request Body:
{
  "url": "https://devblogs.microsoft.com/dotnet/feed/"
}

Response (200 OK):
{
  "isValid": true,
  "feedType": "RSS20",
  "metadata": {
    "title": ".NET Blog",
    "description": "The official .NET team blog...",
    "faviconUrl": "https://devblogs.microsoft.com/dotnet/favicon.ico",
    "itemCount": 25,
    "lastPublishedDate": "2025-01-13T10:30:00Z"
  }
}

Response (400 Bad Request):
{
  "isValid": false,
  "error": {
    "code": "FEED_NOT_FOUND",
    "message": "Unable to fetch the RSS feed",
    "details": "404 Not Found"
  }
}
```

```http
POST /api/v1/rss-feeds
Authorization: Bearer {token}
Content-Type: application/json

Request Body:
{
  "url": "https://devblogs.microsoft.com/dotnet/feed/",
  "title": ".NET Blog Official",
  "categoryId": 5,
  "syncFrequencyMinutes": 240
}

Response (201 Created):
{
  "id": 42,
  "url": "https://devblogs.microsoft.com/dotnet/feed/",
  "title": ".NET Blog Official",
  "type": "RSS20",
  "status": "Active",
  "createdAt": "2025-01-15T10:30:00Z",
  "categoryId": 5
}
```

### Logique de validation
```csharp
public class RssFeedValidationService
{
    private readonly HttpClient _httpClient;
    private readonly ILogger<RssFeedValidationService> _logger;

    public async Task<RssFeedValidationResult> ValidateFeedAsync(
        string url, 
        CancellationToken cancellationToken = default)
    {
        try
        {
            // 1. Validation de l'URL
            if (!Uri.TryCreate(url, UriKind.Absolute, out var uri))
                return RssFeedValidationResult.Invalid("Invalid URL format");

            if (uri.Scheme != Uri.UriSchemeHttp && uri.Scheme != Uri.UriSchemeHttps)
                return RssFeedValidationResult.Invalid("Only HTTP/HTTPS protocols are supported");

            // 2. Fetch du contenu avec timeout de 10s
            using var cts = CancellationTokenSource.CreateLinkedTokenSource(cancellationToken);
            cts.CancelAfter(TimeSpan.FromSeconds(10));
            
            var response = await _httpClient.GetAsync(uri, cts.Token).ConfigureAwait(false);
            
            if (!response.IsSuccessStatusCode)
                return RssFeedValidationResult.Invalid(
                    $"HTTP {(int)response.StatusCode}: {response.ReasonPhrase}");

            // 3. Parse du contenu
            await using var stream = await response.Content.ReadAsStreamAsync(cancellationToken)
                .ConfigureAwait(false);
            
            var feed = await FeedReader.ReadAsync(stream, cancellationToken).ConfigureAwait(false);

            // 4. Extraction des métadonnées
            var metadata = new RssFeedMetadata
            {
                Title = feed.Title,
                Description = feed.Description,
                FaviconUrl = ExtractFaviconUrl(feed),
                ItemCount = feed.Items.Count(),
                LastPublishedDate = feed.Items.FirstOrDefault()?.PublishingDate,
                Type = DetermineFeedType(feed)
            };

            return RssFeedValidationResult.Valid(metadata);
        }
        catch (TaskCanceledException)
        {
            return RssFeedValidationResult.Invalid("Request timeout - Feed took too long to respond");
        }
        catch (HttpRequestException ex)
        {
            _logger.LogWarning(ex, "Failed to fetch RSS feed from {Url}", url);
            return RssFeedValidationResult.Invalid($"Network error: {ex.Message}");
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Unexpected error validating RSS feed {Url}", url);
            return RssFeedValidationResult.Invalid("Unable to parse the feed");
        }
    }
    
    private static RssFeedType DetermineFeedType(Feed feed)
    {
        return feed.Type switch
        {
            FeedType.Rss_2_0 => RssFeedType.RSS20,
            FeedType.Atom => RssFeedType.Atom10,
            FeedType.Rss_1_0 => RssFeedType.RSS10,
            _ => RssFeedType.RSS20
        };
    }
}
```

## 📊 Métriques de succès

### Métriques utilisateur
- **Taux de succès d'ajout** : >95% des tentatives avec URLs valides
- **Temps moyen d'ajout** : <30 secondes (validation incluse)
- **Satisfaction UX** : >85% trouvent le processus intuitif

### Métriques techniques
- **Temps de validation** : <2 secondes pour 90% des flux
- **Taux de détection format** : 100% pour RSS 2.0, Atom, RSS 1.0
- **Taux d'échec réseau** : <5% (hors timeouts légitimes)

### Métriques qualité
- **Précision auto-découverte** : >80% de succès sur pages web
- **Qualité messages d'erreur** : >90% des utilisateurs comprennent l'erreur
- **Taux de retry après erreur** : >60% réessaient avec URL corrigée

## 🚀 Définition de terminé

Cette user story sera considérée comme terminée quand :

- [ ] **Code** : Service de validation + API endpoints + Composant Blazor fonctionnels
- [ ] **Tests** : >90% de couverture sur le service de validation
- [ ] **Performance** : <2s pour 95% des validations
- [ ] **Documentation** : XML docs sur APIs publiques + README pour intégration
- [ ] **UX** : Dialogue d'ajout ergonomique avec tous les états (loading, success, error)
- [ ] **Intégration** : Tests end-to-end passants avec flux RSS réels

---

**Estimation** : 5 points  
**Assignee** : À définir  
**Sprint** : Sprint 1 (Gestion flux RSS)  
**Dependencies** : Entity RssFeed + DbContext

*Dernière mise à jour : 2025-01-15*
