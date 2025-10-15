# US-004 : Import/Export OPML

## 📝 Description

**En tant que** utilisateur migrant depuis un autre agrégateur RSS  
**Je veux** importer mes abonnements via un fichier OPML  
**Afin de** ne pas avoir à ressaisir tous mes flux manuellement

## 🎯 Critères d'acceptation

### Fonctionnalités principales
- [ ] **GIVEN** je suis sur la page de gestion des flux RSS  
      **WHEN** je clique sur "Importer OPML"  
      **THEN** un dialogue s'ouvre me permettant de sélectionner un fichier .opml

- [ ] **GIVEN** j'ai sélectionné un fichier OPML valide avec 50 flux  
      **WHEN** je lance l'import  
      **THEN** le système affiche une barre de progression et importe tous les flux avec leurs catégories

- [ ] **GIVEN** certains flux du fichier OPML sont déjà dans ma liste  
      **WHEN** l'import s'exécute  
      **THEN** ces flux sont automatiquement ignorés sans erreur

- [ ] **GIVEN** j'ai importé mes flux avec succès  
      **WHEN** je clique sur "Exporter OPML"  
      **THEN** un fichier watch-manager-export.opml est téléchargé avec tous mes flux et catégories

### Comportements de validation
- [ ] **PARSER OPML** : Supporte OPML 1.0 et 2.0
- [ ] **PRÉSERVATION CATÉGORIES** : Les catégories (folders) OPML sont converties en catégories Watch Manager
- [ ] **VALIDATION PRÉ-IMPORT** : Chaque flux est validé avant import (option configurable)
- [ ] **GESTION ERREURS** : Rapport détaillé des flux en erreur avec raisons
- [ ] **DÉDOUBLONNAGE** : Détection des flux existants par URL

### Critères techniques
- [ ] **Performance** : <1 seconde par flux en moyenne (validation incluse)
- [ ] **Compatibilité** : Supporte exports de Feedly, Inoreader, NetNewsWire, The Old Reader
- [ ] **Résilience** : Continue l'import même si certains flux échouent
- [ ] **Rollback** : Option pour annuler un import en cours

## 🎨 Maquettes

### Dialogue d'import OPML - Sélection du fichier
```
┌─────────────────────────────────────────────────────────────┐
│ 📥 Importer des flux (OPML)                       [✕]       │
├─────────────────────────────────────────────────────────────┤
│                                                              │
│ Sélectionnez votre fichier OPML                             │
│ ┌────────────────────────────────────────────────────────┐  │
│ │ [📁 Parcourir...]                                      │  │
│ │                                                         │  │
│ │ 💡 Formats supportés: .opml, .xml                      │  │
│ │    Exporté depuis: Feedly, Inoreader, NetNewsWire...  │  │
│ └────────────────────────────────────────────────────────┘  │
│                                                              │
│                      [Annuler]                              │
└─────────────────────────────────────────────────────────────┘
```

### Dialogue d'import OPML - Aperçu
```
┌─────────────────────────────────────────────────────────────┐
│ 📥 Importer des flux (OPML)                       [✕]       │
├─────────────────────────────────────────────────────────────┤
│                                                              │
│ Fichier sélectionné: feedly-subscriptions.opml             │
│ Taille: 127 KB • Dernière modification: 14/01/2025         │
│                                                              │
│ 📊 Aperçu du fichier:                                       │
│ ┌────────────────────────────────────────────────────────┐  │
│ │ • 47 flux RSS détectés                                 │  │
│ │ • 8 catégories                                         │  │
│ │ • 12 flux déjà présents (seront ignorés)              │  │
│ │ • 35 nouveaux flux à importer                          │  │
│ └────────────────────────────────────────────────────────┘  │
│                                                              │
│ Options d'import                                             │
│ ☑ Préserver les catégories existantes                      │
│ ☑ Valider chaque flux avant import (plus lent)             │
│ ☐ Fusionner avec les flux existants (mettre à jour)        │
│                                                              │
│ Catégories détectées:                                       │
│ • Technology (12 flux)                                      │
│ • .NET & C# (8 flux)                                        │
│ • DevOps & Cloud (7 flux)                                   │
│ • Architecture (5 flux)                                     │
│ • ... et 4 autres                                           │
│                                                              │
│               [Annuler]  [Lancer l'import]                  │
└─────────────────────────────────────────────────────────────┘
```

### Dialogue d'import OPML - Import en cours
```
┌─────────────────────────────────────────────────────────────┐
│ 📥 Import en cours... (25/35)                                │
├─────────────────────────────────────────────────────────────┤
│                                                              │
│ [████████████████░░░░░░░░░] 71%                            │
│ Temps écoulé: 00:45 • Temps restant: ~00:20                │
│                                                              │
│ 📝 Journal d'import:                                        │
│ ┌────────────────────────────────────────────────────────┐  │
│ │ ✅ .NET Blog - Importé avec succès                     │  │
│ │ ✅ Scott Hanselman's Blog - Importé (12 articles)     │  │
│ │ ⚠️ Old TechCrunch Feed - Ignoré (déjà présent)        │  │
│ │ ❌ Broken RSS Feed - Erreur 404 Not Found             │  │
│ │ ✅ Martin Fowler - Importé avec succès                 │  │
│ │ ✅ Troy Hunt - Importé (45 articles)                   │  │
│ │ ⏳ Validation de "Andrew Lock's Blog"...              │  │
│ └────────────────────────────────────────────────────────┘  │
│                                                              │
│                    [Annuler l'import]                       │
└─────────────────────────────────────────────────────────────┘
```

### Dialogue d'import OPML - Résultat final
```
┌─────────────────────────────────────────────────────────────┐
│ ✅ Import terminé                                  [✕]       │
├─────────────────────────────────────────────────────────────┤
│                                                              │
│ 📊 Résumé:                                                  │
│ ┌────────────────────────────────────────────────────────┐  │
│ │ ✅ 32 flux importés avec succès                        │  │
│ │ ⚠️ 3 flux ignorés (déjà présents)                     │  │
│ │ ❌ 2 flux en erreur                                   │  │
│ │ ⏱️ Durée totale: 01:15                                │  │
│ └────────────────────────────────────────────────────────┘  │
│                                                              │
│ ❌ Flux en erreur:                                          │
│ • Broken RSS Feed - Erreur 404 Not Found                   │
│ • Invalid Feed - Format RSS invalide                        │
│                                                              │
│ [📥 Télécharger le rapport] [Fermer]                        │
└─────────────────────────────────────────────────────────────┘
```

### Bouton d'export OPML
```
┌─────────────────────────────────────────────────────────────┐
│ 📡 Mes flux RSS                       [+ Ajouter] [⚙️]      │
├─────────────────────────────────────────────────────────────┤
│                                                              │
│ Actions groupées:                                            │
│ [📥 Importer OPML] [📤 Exporter OPML]                       │
│                                                              │
│ 47 flux actifs • 8 catégories                               │
│                                                              │
│ ... (liste des flux) ...                                    │
└─────────────────────────────────────────────────────────────┘
```

## 🧪 Scénarios de test

### Test 1 : Import OPML standard depuis Feedly
```gherkin
Given j'ai exporté mes flux depuis Feedly (feedly-export.opml)
And le fichier contient 25 flux dans 5 catégories
When je clique sur "Importer OPML"
And je sélectionne le fichier feedly-export.opml
And je lance l'import avec validation activée
Then le système affiche "25 flux détectés, 5 catégories"
And l'import se termine avec succès en moins de 30 secondes
And tous les 25 flux apparaissent dans ma liste
And les 5 catégories sont créées avec leurs flux respectifs
```

### Test 2 : Import avec flux déjà présents
```gherkin
Given j'ai déjà 10 flux RSS dans ma liste
And j'importe un fichier OPML contenant 30 flux
And 5 de ces flux ont les mêmes URLs que mes flux existants
When l'import s'exécute
Then le système affiche "5 flux déjà présents (seront ignorés)"
And seuls les 25 nouveaux flux sont importés
And les 5 flux existants ne sont pas modifiés
And un rapport mentionne "5 flux ignorés (déjà présents)"
```

### Test 3 : Import avec flux en erreur
```gherkin
Given j'importe un fichier OPML avec 20 flux
And 3 de ces flux retournent des erreurs 404
And 2 flux ont des formats RSS invalides
When l'import s'exécute avec validation
Then les 15 flux valides sont importés
And les 5 flux en erreur sont listés dans le rapport
And le rapport indique "15 importés, 5 en erreur"
And je peux télécharger le rapport détaillé
```

### Test 4 : Export OPML de mes flux
```gherkin
Given j'ai 30 flux RSS organisés en 6 catégories
When je clique sur "Exporter OPML"
Then un fichier "watch-manager-export-2025-01-15.opml" est téléchargé
And le fichier contient les 30 flux avec leurs métadonnées
And les 6 catégories sont préservées dans la structure OPML
When je ré-importe ce fichier dans un nouveau compte
Then tous les flux sont restaurés à l'identique
```

### Test 5 : Performance sur gros fichier OPML
```gherkin
Given j'importe un fichier OPML avec 200 flux
When je lance l'import avec validation désactivée
Then l'import se termine en moins de 5 minutes
And la barre de progression se met à jour toutes les secondes
And je peux annuler l'import à tout moment
When j'annule l'import après 100 flux
Then seuls les 100 premiers flux sont présents dans ma liste
```

## 🔧 Spécifications techniques

### Modèle de données OPML
```xml
<?xml version="1.0" encoding="UTF-8"?>
<opml version="2.0">
  <head>
    <title>Watch Manager Subscriptions</title>
    <dateCreated>Wed, 15 Jan 2025 10:30:00 GMT</dateCreated>
    <ownerName>User Name</ownerName>
  </head>
  <body>
    <outline text=".NET &amp; C#" title=".NET &amp; C#">
      <outline type="rss" text=".NET Blog" 
               title=".NET Blog" 
               xmlUrl="https://devblogs.microsoft.com/dotnet/feed/" 
               htmlUrl="https://devblogs.microsoft.com/dotnet/"/>
      <outline type="rss" text="Andrew Lock's Blog" 
               title="Andrew Lock's Blog" 
               xmlUrl="https://andrewlock.net/rss/" 
               htmlUrl="https://andrewlock.net/"/>
    </outline>
    <outline text="DevOps" title="DevOps">
      <outline type="rss" text="Azure Updates" 
               title="Azure Updates" 
               xmlUrl="https://azure.microsoft.com/updates/feed/" 
               htmlUrl="https://azure.microsoft.com/updates/"/>
    </outline>
  </body>
</opml>
```

### API Contract
```http
POST /api/v1/rss-feeds/import/opml
Authorization: Bearer {token}
Content-Type: multipart/form-data

Request Body:
--boundary
Content-Disposition: form-data; name="file"; filename="feedly-export.opml"
Content-Type: application/xml

[contenu du fichier OPML]
--boundary
Content-Disposition: form-data; name="options"

{
  "validateBeforeImport": true,
  "preserveCategories": true,
  "mergeExisting": false
}
--boundary--

Response (202 Accepted):
{
  "importId": "import-123e4567-e89b",
  "status": "processing",
  "feedsDetected": 47,
  "categoriesDetected": 8,
  "estimatedDurationSeconds": 60
}
```

```http
GET /api/v1/rss-feeds/import/opml/{importId}/status
Authorization: Bearer {token}

Response (200 OK):
{
  "importId": "import-123e4567-e89b",
  "status": "in_progress",
  "progress": {
    "total": 47,
    "processed": 25,
    "successful": 23,
    "skipped": 1,
    "failed": 1,
    "percentComplete": 53
  },
  "currentFeed": "Andrew Lock's Blog",
  "estimatedTimeRemainingSeconds": 30
}
```

```http
GET /api/v1/rss-feeds/export/opml
Authorization: Bearer {token}

Response (200 OK):
Content-Type: application/xml
Content-Disposition: attachment; filename="watch-manager-export-2025-01-15.opml"

[contenu OPML généré]
```

### Logique d'import
```csharp
public class OpmlImportService
{
    private readonly IRssFeedRepository _feedRepository;
    private readonly RssFeedValidationService _validationService;
    private readonly ILogger<OpmlImportService> _logger;

    public async Task<OpmlImportResult> ImportOpmlAsync(
        Stream opmlStream,
        OpmlImportOptions options,
        IProgress<OpmlImportProgress>? progress = null,
        CancellationToken cancellationToken = default)
    {
        var result = new OpmlImportResult();
        
        try
        {
            // 1. Parser le fichier OPML
            var opml = await ParseOpmlAsync(opmlStream, cancellationToken).ConfigureAwait(false);
            
            var totalFeeds = CountFeeds(opml);
            result.TotalFeeds = totalFeeds;
            
            // 2. Obtenir les flux existants pour dédoublonnage
            var existingFeeds = await _feedRepository.GetAllAsync(cancellationToken)
                .ConfigureAwait(false);
            var existingUrls = existingFeeds.Select(f => f.Url).ToHashSet();
            
            var processed = 0;
            
            // 3. Traiter chaque catégorie et ses flux
            foreach (var category in opml.Body.Outlines)
            {
                var categoryName = category.Text ?? category.Title ?? "Uncategorized";
                var dbCategory = await GetOrCreateCategoryAsync(categoryName, cancellationToken)
                    .ConfigureAwait(false);
                
                foreach (var feedOutline in category.Outlines.Where(o => o.Type == "rss"))
                {
                    processed++;
                    
                    try
                    {
                        var feedUrl = feedOutline.XmlUrl;
                        
                        // Vérifier doublon
                        if (existingUrls.Contains(feedUrl))
                        {
                            result.SkippedFeeds++;
                            _logger.LogInformation("Feed {Url} already exists, skipping", feedUrl);
                            progress?.Report(new OpmlImportProgress(processed, totalFeeds, feedUrl, "Skipped"));
                            continue;
                        }
                        
                        // Validation optionnelle
                        if (options.ValidateBeforeImport)
                        {
                            var validation = await _validationService.ValidateFeedAsync(
                                feedUrl, cancellationToken).ConfigureAwait(false);
                            
                            if (!validation.IsValid)
                            {
                                result.FailedFeeds.Add(new FailedFeed(feedUrl, validation.Error));
                                progress?.Report(new OpmlImportProgress(processed, totalFeeds, feedUrl, "Failed"));
                                continue;
                            }
                        }
                        
                        // Créer le flux
                        var feed = new RssFeed
                        {
                            Url = feedUrl,
                            Title = feedOutline.Title ?? feedOutline.Text ?? "Untitled Feed",
                            CategoryId = dbCategory.Id,
                            Status = RssFeedStatus.Active,
                            CreatedAt = DateTime.UtcNow
                        };
                        
                        await _feedRepository.AddAsync(feed, cancellationToken).ConfigureAwait(false);
                        result.SuccessfulFeeds++;
                        
                        progress?.Report(new OpmlImportProgress(processed, totalFeeds, feedUrl, "Success"));
                    }
                    catch (Exception ex)
                    {
                        _logger.LogError(ex, "Failed to import feed from OPML");
                        result.FailedFeeds.Add(new FailedFeed(feedOutline.XmlUrl, ex.Message));
                    }
                }
            }
            
            return result;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Failed to import OPML file");
            throw new OpmlImportException("Failed to parse OPML file", ex);
        }
    }
    
    private async Task<Opml> ParseOpmlAsync(Stream stream, CancellationToken cancellationToken)
    {
        using var reader = XmlReader.Create(stream, new XmlReaderSettings 
        { 
            Async = true,
            DtdProcessing = DtdProcessing.Ignore,
            XmlResolver = null
        });
        
        var serializer = new XmlSerializer(typeof(Opml));
        var opml = (Opml?)serializer.Deserialize(reader) 
            ?? throw new OpmlImportException("Invalid OPML format");
        
        return await Task.FromResult(opml).ConfigureAwait(false);
    }
}

public class OpmlImportOptions
{
    public bool ValidateBeforeImport { get; set; } = true;
    public bool PreserveCategories { get; set; } = true;
    public bool MergeExisting { get; set; } = false;
}

public class OpmlImportResult
{
    public int TotalFeeds { get; set; }
    public int SuccessfulFeeds { get; set; }
    public int SkippedFeeds { get; set; }
    public List<FailedFeed> FailedFeeds { get; set; } = [];
    public TimeSpan Duration { get; set; }
}
```

### Export OPML
```csharp
public class OpmlExportService
{
    private readonly IRssFeedRepository _feedRepository;

    public async Task<Stream> ExportOpmlAsync(CancellationToken cancellationToken = default)
    {
        var feeds = await _feedRepository.GetAllWithCategoriesAsync(cancellationToken)
            .ConfigureAwait(false);
        
        var opml = new Opml
        {
            Version = "2.0",
            Head = new OpmlHead
            {
                Title = "Watch Manager Subscriptions",
                DateCreated = DateTime.UtcNow.ToString("R")
            },
            Body = new OpmlBody()
        };
        
        // Grouper par catégorie
        var feedsByCategory = feeds.GroupBy(f => f.Category?.Name ?? "Uncategorized");
        
        foreach (var categoryGroup in feedsByCategory)
        {
            var categoryOutline = new OpmlOutline
            {
                Text = categoryGroup.Key,
                Title = categoryGroup.Key,
                Outlines = []
            };
            
            foreach (var feed in categoryGroup)
            {
                categoryOutline.Outlines.Add(new OpmlOutline
                {
                    Type = "rss",
                    Text = feed.Title,
                    Title = feed.Title,
                    XmlUrl = feed.Url,
                    HtmlUrl = ExtractHtmlUrl(feed.Url)
                });
            }
            
            opml.Body.Outlines.Add(categoryOutline);
        }
        
        // Sérialiser en XML
        var memoryStream = new MemoryStream();
        var serializer = new XmlSerializer(typeof(Opml));
        
        await using var writer = new StreamWriter(memoryStream, Encoding.UTF8, leaveOpen: true);
        serializer.Serialize(writer, opml);
        
        memoryStream.Position = 0;
        return memoryStream;
    }
}
```

## 📊 Métriques de succès

### Métriques utilisateur
- **Taux d'utilisation import** : >40% des utilisateurs importent un OPML
- **Taille moyenne import** : 20-50 flux par fichier
- **Satisfaction** : >90% trouvent l'import/export utile

### Métriques techniques
- **Performance import** : <1s par flux (sans validation), <3s par flux (avec validation)
- **Taux de succès parsing** : >98% des fichiers OPML standards
- **Compatibilité** : 100% avec exports Feedly, Inoreader, NetNewsWire

### Métriques qualité
- **Préservation catégories** : 100% des catégories OPML préservées
- **Dédoublonnage** : 100% de détection des URLs identiques
- **Résilience** : Continue même avec 50% de flux en erreur

## 🚀 Définition de terminé

Cette user story sera considérée comme terminée quand :

- [ ] **Code** : Services d'import/export OPML + API endpoints + UI fonctionnels
- [ ] **Tests** : >85% couverture + tests avec fichiers OPML réels (Feedly, Inoreader)
- [ ] **Performance** : Import de 100 flux en <3 minutes avec validation
- [ ] **Documentation** : Guide utilisateur pour migration depuis autres agrégateurs
- [ ] **UX** : Dialogues ergonomiques avec feedback temps réel (barre de progression)
- [ ] **Compatibilité** : Tests réussis avec 5+ formats OPML différents

---

**Estimation** : 8 points  
**Assignee** : À définir  
**Sprint** : Sprint 3 (Gestion flux RSS)  
**Dependencies** : US-001 (Ajout flux RSS) + US-003 (CRUD flux)

*Dernière mise à jour : 2025-01-15*
