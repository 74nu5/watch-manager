# US-001 : Sélection d'articles non consultés

## 📝 Description

**En tant que** développeur utilisant Watch Manager  
**Je veux** recevoir automatiquement des suggestions d'articles que je n'ai pas encore lus  
**Afin de** découvrir de nouveaux contenus pertinents sans effort de recherche

## 🎯 Critères d'acceptation

### Fonctionnalités principales
- [ ] **GIVEN** je suis connecté à Watch Manager  
      **WHEN** j'accède à la section "Découverte"  
      **THEN** je vois 5 articles que je n'ai jamais ouverts

- [ ] **GIVEN** j'ai lu 80% des articles d'une catégorie  
      **WHEN** le système génère des suggestions  
      **THEN** il privilégie les articles des autres catégories

- [ ] **GIVEN** il n'y a plus d'articles non-lus dans mes domaines d'intérêt  
      **WHEN** je demande des suggestions  
      **THEN** le système me propose d'explorer de nouveaux domaines

### Comportements attendus
- [ ] **EXCLUSION** : Les articles déjà lus ne sont jamais re-suggérés
- [ ] **HISTORIQUE** : Le système garde trace des articles vus même partiellement (>30s)
- [ ] **REFRESH** : Nouvelles suggestions disponibles toutes les 24h minimum
- [ ] **EMPTY STATE** : Message informatif si aucun nouveau contenu disponible

### Critères techniques
- [ ] **PERFORMANCE** : Génération des suggestions en <500ms
- [ ] **SCALABILITÉ** : Fonctionne avec 10k+ articles en base
- [ ] **ÉTAT PERSISTANT** : Suggestions sauvegardées entre les sessions
- [ ] **API ENDPOINT** : `/api/v1/recommendations/unread`

## 🎨 Maquettes

### Interface suggestions principales
```
┌─────────────────────────────────────────────────────────┐
│ 🎲 Articles à découvrir                                │ 
├─────────────────────────────────────────────────────────┤
│                                                         │
│ 📄 [ASP.NET Core] Minimal APIs with Entity Framework   │
│     🏷️ Backend • API • .NET • 📊 Niveau: Intermédiaire │
│     ⏱️ 8 min • 👁️ Nouveau pour vous                    │
│                                                         │
│ 📄 [Kubernetes] Pod Security Standards in Practice     │  
│     🏷️ DevOps • Container • Security • 📊 Avancé      │
│     ⏱️ 12 min • 👁️ Nouveau pour vous                   │
│                                                         │
│ 📄 [Frontend] React Server Components Deep Dive        │
│     🏷️ React • SSR • Performance • 📊 Expert          │  
│     ⏱️ 15 min • 👁️ Nouveau pour vous                   │
│                                                         │
│ [📱 Voir plus] [🔄 Actualiser] [⚙️ Personnaliser]     │
└─────────────────────────────────────────────────────────┘
```

### Empty state - Pas de nouveaux articles
```
┌─────────────────────────────────────────────────────────┐
│                    🎯 Félicitations !                  │
│                                                         │
│              Vous êtes à jour dans vos                 │
│              domaines de prédilection                  │
│                                                         │
│    💡 Suggestions pour continuer votre veille :        │
│                                                         │
│    🔍 [Explorer de nouveaux domaines]                  │
│    📚 [Approfondir vos connaissances actuelles]        │
│    🔄 [Vérifier les nouveaux contenus demain]          │
│                                                         │
└─────────────────────────────────────────────────────────┘
```

## 🧪 Scénarios de test

### Test 1 : Premier utilisateur
```gherkin
Given je suis un nouvel utilisateur de Watch Manager
And il y a 100 articles en base de données  
When j'accède à la section découverte
Then je vois 5 articles suggérés
And aucun article affiché n'a été lu par moi
And les suggestions couvrent au moins 3 catégories différentes
```

### Test 2 : Utilisateur actif  
```gherkin
Given j'ai lu 50 articles la semaine dernière
And il y a 20 nouveaux articles ajoutés aujourd'hui
When je consulte mes suggestions
Then les 20 nouveaux articles sont prioritaires
And je ne vois aucun des 50 articles déjà lus
```

### Test 3 : Utilisateur exhaustif
```gherkin  
Given j'ai lu 95% des articles de la base
And il n'y a que 5 articles non-lus  
When je demande des suggestions
Then les 5 articles restants sont affichés
And je vois un message m'encourageant à explorer de nouveaux domaines
```

## 🔧 Spécifications techniques

### Modèle de données
```csharp
public class UserReadHistory
{
    public int UserId { get; set; }
    public int ArticleId { get; set; }  
    public DateTime ReadAt { get; set; }
    public TimeSpan ReadDuration { get; set; }
    public bool IsCompleted { get; set; }
}

public class RecommendationRequest  
{
    public int UserId { get; set; }
    public int Count { get; set; } = 5;
    public string[] ExcludeCategories { get; set; } = [];
    public DifficultyLevel? PreferredLevel { get; set; }
}
```

### API Contract
```http
GET /api/v1/recommendations/unread?count=5&level=intermediate
Authorization: Bearer {token}

Response:
{
  "articles": [
    {
      "id": 123,
      "title": "ASP.NET Core Minimal APIs", 
      "category": "Backend",
      "level": "Intermediate",
      "estimatedReadTime": 8,
      "isNewForUser": true,
      "relevanceScore": 0.87
    }
  ],
  "totalUnread": 247,
  "lastGenerated": "2024-12-19T10:30:00Z"
}
```

### Algorithme de sélection
```csharp
public async Task<RecommendationResult> GetUnreadRecommendationsAsync(
    int userId, int count = 5)
{
    // 1. Récupérer tous les articles non-lus par l'utilisateur
    var unreadArticles = await GetUnreadArticlesAsync(userId);
    
    // 2. Calculer un score de pertinence pour chaque article
    var scoredArticles = unreadArticles
        .Select(article => new {
            Article = article,
            Score = CalculateRelevanceScore(article, userProfile)
        })
        .OrderByDescending(x => x.Score);
    
    // 3. Appliquer la diversification (max 2 articles par catégorie)
    var diversifiedSelection = ApplyDiversification(scoredArticles, count);
    
    return new RecommendationResult(diversifiedSelection);
}
```

## 📊 Métriques de succès

### Métriques utilisateur
- **Engagement** : 80% des suggestions sont ouvertes dans les 48h
- **Satisfaction** : Score de pertinence >4/5 via feedback utilisateur  
- **Découverte** : 60% des articles suggérés sont lus en entier

### Métriques techniques  
- **Performance** : Temps de réponse API <300ms (P95)
- **Précision** : 0% d'articles déjà lus dans les suggestions
- **Couverture** : 90% des articles de la base sont suggérés au moins une fois

### Métriques business
- **Rétention** : +25% de sessions utilisateur avec la feature activée
- **Découverte de contenu** : +40% d'articles lus par utilisateur/mois
- **Diversité** : Réduction de 30% de la concentration sur une seule catégorie

## 🚀 Définition de terminé

Cette user story sera considérée comme terminée quand :

- [ ] **Code** : Algorithme implémenté et testé unitairement
- [ ] **API** : Endpoint fonctionnel avec documentation Swagger
- [ ] **Interface** : Composant Blazor intégré dans l'interface principale  
- [ ] **Tests** : Scénarios automatisés passent à 100%
- [ ] **Performance** : Benchmarks respectent les critères (<500ms)
- [ ] **Validation** : Tests utilisateur confirment l'utilité (>4/5)

---

**Estimation** : 5 points  
**Assignee** : À définir  
**Sprint** : À planifier  
**Dependencies** : Système d'authentification utilisateur

*Dernière mise à jour : ${new Date().toLocaleDateString('fr-FR')}*
