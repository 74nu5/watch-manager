# US-006 : Intégration des trending topics

## 📝 Description

**En tant que** professionnel tech  
**Je veux** être informé des technologies émergentes et sujets d'actualité  
**Afin de** rester compétitif et à jour sur les tendances du marché

## 🎯 Critères d'acceptation

### Fonctionnalités principales
- [ ] **GIVEN** il y a des articles tendance cette semaine (>1000 vues/jour)  
      **WHEN** je consulte mes suggestions  
      **THEN** au moins 1 article tendance est inclus dans ma sélection

- [ ] **GIVEN** une nouvelle technologie émergente est détectée (ChatGPT, Deno, etc.)  
      **WHEN** je reçois mes recommandations  
      **THEN** je vois un article d'introduction à cette technologie

- [ ] **GIVEN** je configure ma veille sur "Innovation + Stabilité équilibrée"  
      **WHEN** le système génère ma sélection  
      **THEN** 40% sont des articles établis, 60% des nouveautés/tendances

### Détection des tendances
- [ ] **ALGORITHME TRENDING** : Détection automatique des sujets populaires (volume, vitesse)
- [ ] **TECHNOLOGIES ÉMERGENTES** : Identification des nouvelles techs par analyse sémantique
- [ ] **SAISONNALITÉ** : Prise en compte des cycles tech (conférences, releases, etc.)
- [ ] **COMMUNAUTÉ** : Intégration des signals de la communauté (GitHub stars, discussions)

### Équilibrage et personnalisation
- [ ] **PROFIL RISQUE** : Configuration early-adopter vs conservateur
- [ ] **FILTRAGE INTELLIGENT** : Éviter le hype sans substance (filtering buzzword bingo)
- [ ] **CONTEXTUALISATION** : Trending topics pertinents selon le profil utilisateur
- [ ] **HISTORIQUE** : Suivi des tendances adoptées vs ignorées par l'utilisateur

## 🎨 Maquettes

### Section trending topics
```
┌─────────────────────────────────────────────────────────┐
│ 🔥 Trending cette semaine                              │
├─────────────────────────────────────────────────────────┤
│                                                         │
│ 📈 AI Code Generation: GitHub Copilot Alternatives     │
│     🏷️ AI • Development • 📊 Intermédiaire             │
│     🔥 +2,400% lectures cette semaine                  │
│     ⏱️ 15 min • 💬 834 discussions                     │
│                                                         │
│ 🚀 Astro 4.0: The Islands Architecture Revolution     │
│     🏷️ Frontend • SSG • 📊 Avancé                     │
│     🔥 Trending #2 sur HackerNews                      │
│     ⏱️ 22 min • ⭐ 35k stars GitHub                    │
│                                                         │
│ ⚡ Bun vs Node.js: Runtime Performance Battle          │
│     🏷️ Runtime • Performance • 📊 Expert              │
│     🔥 +5,000 GitHub stars cette semaine               │
│     ⏱️ 18 min • 🎯 Match votre profil backend          │
│                                                         │
│ [📊 Voir toutes les tendances] [⚙️ Personnaliser]      │
└─────────────────────────────────────────────────────────┘
```

### Configuration profil innovation
```
┌─────────────────────────────────────────────────────────┐
│ 🎯 Mon profil d'innovation tech                        │
├─────────────────────────────────────────────────────────┤
│                                                         │
│ 🚀 Appétit pour les nouveautés                         │
│   Early Adopter ████████████████ Conservative         │
│                        ↑                               │
│              Équilibré (60% nouveau)                   │
│                                                         │
│ 🔍 Sources de tendances suivies                        │
│   ☑ GitHub Trending                                    │
│   ☑ HackerNews                                         │
│   ☑ Reddit r/programming                               │
│   ☑ Twitter tech influencers                          │
│   ☑ Conference talks & releases                       │
│   ☐ Medium trending tech                              │
│                                                         │
│ ⚡ Vitesse d'adoption souhaitée                        │
│   ☑ Immédiate (semaine de sortie)                     │
│   ☑ Rapide (premier mois)                             │
│   ☑ Modérée (après 3 mois d'adoption)                 │
│   ☐ Prudente (après 1 an de maturité)                 │
│                                                         │
│ 🎯 Domaines d'innovation prioritaires                  │
│   ☑ Intelligence Artificielle                         │
│   ☑ Frontend Frameworks                               │
│   ☑ DevOps & Cloud                                    │
│   ☐ Blockchain & Web3                                 │
│   ☑ Performance & Optimization                        │
│                                                         │
│ [💾 Sauvegarder] [🔄 Détection auto] [📊 Historique]  │
└─────────────────────────────────────────────────────────┘
```

### Dashboard tendances et insights
```
┌─────────────────────────────────────────────────────────┐
│ 📊 Tech Trends Dashboard                               │
├─────────────────────────────────────────────────────────┤
│                                                         │
│ 🏆 Top Technologies cette semaine                      │
│   1. 🤖 AI/ML Tools          ████████████ +340%       │
│   2. ⚡ Edge Computing        ██████████░░ +280%       │
│   3. 🎨 No-Code Platforms     ████████░░░░ +220%       │
│   4. 🔐 Zero Trust Security   ██████░░░░░░ +180%       │
│                                                         │
│ 🎯 Personnalisé pour vous                              │
│   • React 19 Features → Match votre expertise React    │
│   • .NET 9 Performance → Align avec vos projets       │
│   • Azure OpenAI → Tendance + votre stack Azure       │
│                                                         │
│ 📈 Votre historique tendances                          │
│   Adopté: Vite, Docker Compose, TypeScript 5.0        │
│   Ignoré: Web3, Micro-frontends, GraphQL               │
│   En watch: Astro, Bun, Tauri                         │
│                                                         │
│ [🔔 Alertes] [📊 Analytics] [💡 Suggestions]          │
└─────────────────────────────────────────────────────────┘
```

## 🧪 Scénarios de test

### Test 1 : Early adopter
```gherkin
Given je suis configuré comme "Early Adopter" (80% nouvelles techs)
And une nouvelle technologie "Bun" fait le buzz cette semaine
And j'ai un profil backend Node.js
When je demande mes suggestions quotidiennes
Then je vois un article sur Bun dans les 3 premiers
And l'article est marqué "🔥 Trending" avec les métriques
And je vois le contexte "Alternative à Node.js" 
```

### Test 2 : Développeur conservateur
```gherkin
Given je suis configuré comme "Conservative" (20% nouvelles techs)  
And il y a 10 articles trending sur des techs expérimentales
And il y a 5 articles sur des technologies matures
When je consulte mes suggestions (5 articles)
Then maximum 1 article concerne une technologie expérimentale
And cet article est bien documenté avec retours communauté
And je vois l'indication "Suffisamment mature pour adoption"
```

### Test 3 : Détection technologie émergente
```gherkin
Given une nouvelle technologie "SvelteKit 2.0" sort cette semaine
And elle gagne >10k GitHub stars en 3 jours
And j'ai un profil frontend avec intérêt Svelte
When l'algorithme évalue les trending topics  
Then SvelteKit 2.0 est automatiquement détecté comme "émergent"
And un article d'introduction est suggéré dans les 48h
And je reçois une notification "Nouvelle version de votre stack"
```

### Test 4 : Équilibrage innovations vs besoins actuels
```gherkin
Given j'ai configuré 50% trending / 50% projets actuels
And je travaille sur un projet React en production
And il y a un buzz sur "Vue 4.0 Vapor Mode"
When je reçois mes 6 suggestions quotidiennes
Then 3 articles concernent React/mon projet
And 3 articles concernent les tendances (dont Vue 4.0)
And l'équilibrage respecte mes préférences configurées
```

## 🔧 Spécifications techniques

### Modèle de trending topic
```csharp
public class TrendingTopic
{
    public int Id { get; set; }
    public string Topic { get; set; }
    public string[] Keywords { get; set; }
    public TrendingSource Source { get; set; }
    public TrendingMetrics Metrics { get; set; }
    public DateTime DetectedAt { get; set; }
    public DateTime PeakAt { get; set; }
    public TrendingStatus Status { get; set; }
    public List<Article> RelatedArticles { get; set; }
}

public class TrendingMetrics
{
    public long ViewsCount { get; set; }
    public float VelocityScore { get; set; } // Vitesse de croissance
    public int GitHubStars { get; set; }
    public int DiscussionsCount { get; set; }
    public int MentionsCount { get; set; }
    public float ViralityScore { get; set; }
    public Dictionary<string, object> SourceSpecificMetrics { get; set; }
}

public enum TrendingSource
{
    GitHub,
    HackerNews,
    Reddit,
    Twitter,
    Medium,
    ConferenceTalks,
    ProductHunt,
    DevTo
}

public enum TrendingStatus
{
    Emerging,    // Nouvelle détection
    Rising,      // En croissance
    Peak,        // Au pic
    Declining,   // En déclin
    Stable,      // Stabilisé
    Dead         // Plus tendance
}
```

### Configuration utilisateur innovation
```csharp
public class UserInnovationProfile
{
    public int UserId { get; set; }
    public InnovationAppetite Appetite { get; set; } = InnovationAppetite.Balanced;
    public float NoveltyRatio { get; set; } = 0.3f; // 30% nouveautés par défaut
    public List<TrendingSource> PreferredSources { get; set; }
    public List<string> InnovationDomains { get; set; }
    public AdoptionSpeed PreferredSpeed { get; set; }
    public int MinCommunityValidation { get; set; } = 1000; // Min stars/votes pour considérer
    public bool BuzzwordFiltering { get; set; } = true;
}

public enum InnovationAppetite
{
    Conservative = 1,  // 10% nouveautés
    Moderate = 2,      // 30% nouveautés  
    Balanced = 3,      // 50% nouveautés
    Progressive = 4,   // 70% nouveautés
    EarlyAdopter = 5   // 90% nouveautés
}

public enum AdoptionSpeed
{
    Immediate,  // Semaine de sortie
    Fast,       // Premier mois
    Moderate,   // Après 3 mois  
    Cautious    // Après 1 an
}
```

### Service de détection des tendances
```csharp
public class TrendingDetectionService
{
    public async Task<List<TrendingTopic>> DetectTrendingTopicsAsync()
    {
        var trends = new List<TrendingTopic>();
        
        // 1. Collecter les données depuis multiples sources
        var githubTrends = await _githubService.GetTrendingRepositoriesAsync();
        var hackerNewsTrends = await _hackerNewsService.GetTrendingStoriesAsync();
        var redditTrends = await _redditService.GetTrendingPostsAsync();
        
        // 2. Analyser et scorer chaque sujet
        var allTopics = await AnalyzeAndScoreTopicsAsync(
            githubTrends, hackerNewsTrends, redditTrends);
        
        // 3. Filtrer et déduplication
        var filteredTopics = await FilterAndDeduplicateAsync(allTopics);
        
        // 4. Classification et enrichissement
        foreach (var topic in filteredTopics)
        {
            topic.Status = ClassifyTrendingStatus(topic);
            topic.RelatedArticles = await FindRelatedArticlesAsync(topic);
        }
        
        return filteredTopics.OrderByDescending(t => t.Metrics.ViralityScore).ToList();
    }
    
    private async Task<float> CalculateViralityScore(TrendingTopic topic)
    {
        // Algorithme composite prenant en compte :
        // - Vitesse de croissance (dérivée du volume)
        // - Amplitude (pic absolu)
        // - Diversité des sources
        // - Engagement qualité (pas que du spam)
        
        var velocityScore = CalculateVelocity(topic.Metrics);
        var amplitudeScore = NormalizeAmplitude(topic.Metrics.ViewsCount);
        var diversityScore = CalculateSourceDiversity(topic.Source);
        var qualityScore = AssessEngagementQuality(topic.Metrics);
        
        return (velocityScore * 0.4f) + (amplitudeScore * 0.2f) + 
               (diversityScore * 0.2f) + (qualityScore * 0.2f);
    }
}
```

### Algorithme d'intégration dans les recommandations
```csharp
public class TrendingIntegrationService
{
    public async Task<List<Article>> IntegrateTrendingArticlesAsync(
        List<Article> baseRecommendations,
        UserInnovationProfile innovationProfile,
        List<TrendingTopic> activeTrends)
    {
        var trendingSlots = CalculateTrendingSlots(
            baseRecommendations.Count, innovationProfile.NoveltyRatio);
        
        var personalizedTrends = await PersonalizeTrendingTopicsAsync(
            activeTrends, innovationProfile);
        
        var trendingArticles = await SelectBestTrendingArticlesAsync(
            personalizedTrends, trendingSlots);
        
        // Remplacer les articles les moins pertinents par les trending
        var finalRecommendations = MergeTrendingWithBase(
            baseRecommendations, trendingArticles, innovationProfile);
        
        return finalRecommendations.OrderByDescending(a => a.FinalScore).ToList();
    }
    
    private async Task<List<TrendingTopic>> PersonalizeTrendingTopicsAsync(
        List<TrendingTopic> trends, UserInnovationProfile profile)
    {
        return trends.Where(trend =>
        {
            // Filtrer selon les domaines d'intérêt
            var domainMatch = trend.Keywords.Any(k => 
                profile.InnovationDomains.Contains(k, StringComparer.OrdinalIgnoreCase));
            
            // Filtrer selon la vitesse d'adoption
            var speedMatch = IsAdoptionSpeedAppropriate(trend, profile.PreferredSpeed);
            
            // Filtrer selon validation communauté
            var validationMatch = trend.Metrics.GitHubStars >= profile.MinCommunityValidation;
            
            return domainMatch && speedMatch && validationMatch;
        }).ToList();
    }
}
```

### API trending topics
```http
GET /api/v1/trending/topics?period=week&limit=10
Authorization: Bearer {token}

Response:
{
  "trends": [
    {
      "id": 1,
      "topic": "AI Code Generation",
      "keywords": ["AI", "Copilot", "CodeGen", "LLM"],
      "status": "Peak",
      "metrics": {
        "viewsCount": 125000,
        "velocityScore": 0.94,
        "githubStars": 15000,
        "viralityScore": 0.87
      },
      "relatedArticles": [
        {
          "id": 456,
          "title": "GitHub Copilot vs ChatGPT Code: Complete Comparison",
          "relevanceScore": 0.92
        }
      ]
    }
  ],
  "userRecommendations": [
    {
      "trendId": 1,
      "matchReason": "Align with your AI/Backend interests",
      "adoptionAdvice": "Early stage but promising - worth exploring"
    }
  ]
}

GET /api/v1/recommendations/with-trending?trendingRatio=0.4
Authorization: Bearer {token}
```

## 📊 Métriques de succès

### Métriques de pertinence trending
- **Précision trending** : 80% des articles marqués "trending" confirmés par les utilisateurs
- **Découverte technologique** : 40% des utilisateurs adoptent ≥1 nouvelle tech via trending
- **Timing optimal** : 70% des trending topics suggérés au moment de croissance max

### Métriques d'engagement
- **Engagement trending** : +60% temps de lecture sur articles trending vs normaux
- **Adoption suggestions** : 25% des trending topics explorés mènent à adoption
- **Partage social** : 3x plus de partages sur articles trending vs standards

### Métriques de qualité
- **Filtre anti-hype** : <5% d'articles trending jugés "pure buzzword"
- **Personnalisation** : 85% des trending suggérés alignés avec profil utilisateur
- **Équilibrage** : Respect du ratio configuré novelty/stability à ±10%

### Métriques business
- **Différenciation** : 90% des utilisateurs trouvent les trending "unique vs autres sources"
- **Compétitivité** : 80% se sentent "plus à jour technologiquement"
- **Fidélisation** : +50% de retention utilisateurs avec trending activé

## 🚀 Définition de terminé

Cette user story sera considérée comme terminée quand :

- [ ] **Détection automatique** : Algorithme de trending detection opérationnel
- [ ] **Sources multiples** : Intégration ≥3 sources (GitHub, HN, Reddit minimum)
- [ ] **Configuration profil** : Interface complète de personnalisation innovation
- [ ] **Intégration recommandations** : Trending topics intégrés dans suggestions quotidiennes
- [ ] **Filtrage qualité** : Anti-buzzword et validation communauté fonctionnels
- [ ] **Dashboard trends** : Interface de visualisation des tendances actives
- [ ] **Performance** : Détection et intégration <1s pour 100 trending topics
- [ ] **Tests complets** : Scénarios early-adopter vs conservative validés
- [ ] **Validation utilisateur** : Tests confirment la valeur ajoutée des trending

---

**Estimation** : 21 points  
**Assignee** : À définir  
**Sprint** : À planifier après US-005  
**Dependencies** : US-001 Sélection articles, US-002 Niveaux expertise, APIs externes (GitHub, HN)

*Dernière mise à jour : 14 septembre 2025*