# US-005 : Contextualisation professionnelle

## 📝 Description

**En tant que** développeur avec des projets en cours  
**Je veux** que les suggestions priorisent les technologies liées à mes projets actuels  
**Afin de** que ma veille soit directement applicable à mon travail

## 🎯 Critères d'acceptation

### Fonctionnalités principales
- [ ] **GIVEN** j'ai configuré mes projets actuels (React + .NET)  
      **WHEN** je reçois des suggestions d'articles  
      **THEN** 60% des articles concernent React ou .NET

- [ ] **GIVEN** j'indique travailler sur une migration vers microservices  
      **WHEN** le système génère mes recommandations  
      **THEN** il privilégie les articles sur microservices, Docker, et API design

- [ ] **GIVEN** mon projet utilise Azure et DevOps  
      **WHEN** je consulte ma sélection quotidienne  
      **THEN** je vois des articles Azure DevOps, CI/CD, et cloud architecture

### Gestion contextuelle
- [ ] **PROJETS MULTIPLES** : Gestion de plusieurs projets avec technologies différentes
- [ ] **PONDÉRATION** : Possibilité de définir l'importance relative de chaque projet
- [ ] **TEMPORALITÉ** : Prise en compte des deadlines pour prioriser les besoins urgents
- [ ] **ÉVOLUTION** : Adaptation des suggestions selon l'avancement du projet

### Critères techniques
- [ ] **MATCHING SÉMANTIQUE** : Reconnaissance des technologies liées (ex: React → Next.js, Redux)
- [ ] **SCORE CONTEXTUEL** : Boost de pertinence pour les articles project-relevant
- [ ] **EQUILIBRAGE** : Maintien d'une diversité minimale même avec focus projet
- [ ] **HISTORIQUE** : Suivi de l'efficacité des suggestions contextuelles

## 🎨 Maquettes

### Configuration des projets actuels
```
┌─────────────────────────────────────────────────────────┐
│ 💼 Mes projets en cours                                │
├─────────────────────────────────────────────────────────┤
│                                                         │
│ 🚀 E-commerce Platform (Priority: High) 🔴            │
│   Technologies: React, .NET Core, Azure SQL           │
│   Deadline: Mars 2024                                  │
│   Focus: Performance, Security, Scalability           │
│   [✏️ Modifier] [📊 Stats] [🗑️ Archiver]             │
│                                                         │
│ 📱 Mobile App MVP (Priority: Medium) 🟡               │
│   Technologies: React Native, Firebase                 │
│   Deadline: Avril 2024                                 │
│   Focus: UX, Offline sync, Push notifications         │
│   [✏️ Modifier] [📊 Stats]                            │
│                                                         │
│ [➕ Ajouter un projet] [⚙️ Configuration avancée]      │
└─────────────────────────────────────────────────────────┘
```

### Suggestions contextualisées
```
┌─────────────────────────────────────────────────────────┐
│ 🎯 Suggestions pour vos projets                        │
├─────────────────────────────────────────────────────────┤
│                                                         │
│ 🚀 Pour E-commerce Platform                           │
│                                                         │
│ 📄 ASP.NET Core Performance Optimization               │
│     🏷️ .NET • Performance • 📊 Avancé                 │
│     💼 Parfait pour votre projet e-commerce            │
│     ⏱️ 18 min • 🎯 Haute priorité                      │
│                                                         │
│ 📄 React Query vs SWR: Data Fetching Comparison        │
│     🏷️ React • Data • 📊 Intermédiaire                │
│     💼 Utile pour optimiser les requêtes               │
│     ⏱️ 12 min • 🎯 Recommandé                          │
│                                                         │
│ 📱 Pour Mobile App MVP                                │
│                                                         │
│ 📄 React Native Offline-First Architecture             │
│     🏷️ React Native • Offline • 📊 Avancé             │
│     💼 Répond à votre besoin offline sync              │
│     ⏱️ 25 min • 🎯 Très pertinent                      │
│                                                         │
│ [💼 Mode projet] [🌍 Mode général] [⚙️ Ajuster focus] │
└─────────────────────────────────────────────────────────┘
```

### Configuration focus projet
```
┌─────────────────────────────────────────────────────────┐
│ ⚙️ Personnaliser la contextualisation                  │
├─────────────────────────────────────────────────────────┤
│                                                         │
│ Focus sur les projets actuels                          │
│ ████████████████████████ 80% 💼 Max contexte          │
│                                                         │
│ Diversité thématique garantie                          │
│ █████████ 20% 🌍 Exploration générale                  │
│                                                         │
│ 📋 Priorité des contextes                              │
│ 1. 🚀 E-commerce Platform                              │
│ 2. 📱 Mobile App MVP                                   │
│ 3. 🎓 Formation personnelle                            │
│                                                         │
│ ⏰ Adaptation temporelle                               │
│ ☑ Booster les articles urgents (deadline < 30j)       │
│ ☑ Archiver auto les projets terminés                  │
│ ☑ Suggérer des projets d'apprentissage                │
│                                                         │
│ [💾 Sauvegarder] [🔄 Réinitialiser]                   │
└─────────────────────────────────────────────────────────┘
```

## 🧪 Scénarios de test

### Test 1 : Développeur mono-projet
```gherkin
Given je travaille uniquement sur un projet React/Node.js
And j'ai configuré ce projet avec priorité "High"
And je configure le focus à 80%
When je demande des suggestions (5 articles)
Then au moins 4 articles concernent React, Node.js ou JavaScript
And les articles ont un score de contextualisation élevé
And je vois l'indication "Parfait pour votre projet" sur les articles pertinents
```

### Test 2 : Développeur multi-projets
```gherkin
Given j'ai 2 projets : "E-commerce (.NET)" priorité High et "Mobile (React Native)" priorité Medium
And la deadline e-commerce est dans 2 semaines
When je consulte mes suggestions
Then 60% des articles concernent .NET/Azure (projet prioritaire)
And 25% des articles concernent React Native  
And 15% sont des articles de diversité générale
And les articles .NET sont marqués comme "Urgents"
```

### Test 3 : Évolution du contexte projet
```gherkin
Given j'ai un projet "API Migration" avec deadline dans 3 mois
And je lis régulièrement les articles microservices suggérés
When la deadline approche (< 1 mois)
Then le score de priorité des articles microservices augmente
And je vois plus d'articles pratiques vs théoriques
And les articles "Getting Started" disparaissent au profit des "Advanced"
```

## 🔧 Spécifications techniques

### Modèle projet utilisateur
```csharp
public class UserProject
{
    public int Id { get; set; }
    public int UserId { get; set; }
    public string Name { get; set; }
    public string Description { get; set; }
    public List<string> Technologies { get; set; }
    public List<string> FocusAreas { get; set; } // Performance, Security, etc.
    public ProjectPriority Priority { get; set; }
    public DateTime? Deadline { get; set; }
    public ProjectStatus Status { get; set; }
    public DateTime CreatedAt { get; set; }
    public DateTime UpdatedAt { get; set; }
}

public enum ProjectPriority
{
    Low = 1,
    Medium = 2, 
    High = 3,
    Critical = 4
}

public enum ProjectStatus
{
    Planning,
    Active,
    OnHold,
    Completed,
    Archived
}
```

### Configuration contextualisation
```csharp
public class ContextualizationSettings
{
    public int UserId { get; set; }
    public float ProjectFocusRatio { get; set; } = 0.7f; // 70% articles projet
    public float DiversityRatio { get; set; } = 0.3f;   // 30% diversité
    public bool UrgencyBoostEnabled { get; set; } = true;
    public int UrgencyThresholdDays { get; set; } = 30;
    public bool AutoArchiveCompleted { get; set; } = true;
    public List<int> ProjectPriorityOrder { get; set; }
}
```

### Algorithme de contextualisation
```csharp
public class ProjectContextualizationService
{
    public async Task<List<Article>> ContextualizeRecommendationsAsync(
        List<Article> baseRecommendations, 
        List<UserProject> activeProjects,
        ContextualizationSettings settings)
    {
        var contextualizedArticles = new List<Article>();
        
        // 1. Calculer le score contextuel pour chaque article
        foreach (var article in baseRecommendations)
        {
            var contextScore = CalculateProjectRelevance(article, activeProjects);
            var urgencyBoost = CalculateUrgencyBoost(article, activeProjects, settings);
            
            article.ContextualScore = contextScore + urgencyBoost;
            article.ProjectRelevance = GetRelevantProjects(article, activeProjects);
        }
        
        // 2. Sélectionner articles selon ratio projet/diversité
        var projectArticles = SelectProjectRelevantArticles(
            baseRecommendations, settings.ProjectFocusRatio);
        var diversityArticles = SelectDiversityArticles(
            baseRecommendations, settings.DiversityRatio);
            
        return CombineAndRank(projectArticles, diversityArticles);
    }
    
    private float CalculateProjectRelevance(Article article, List<UserProject> projects)
    {
        float maxRelevance = 0f;
        
        foreach (var project in projects.Where(p => p.Status == ProjectStatus.Active))
        {
            var techMatch = CalculateTechnologyMatch(article.Tags, project.Technologies);
            var focusMatch = CalculateFocusMatch(article.Content, project.FocusAreas);
            var priorityWeight = GetPriorityWeight(project.Priority);
            
            var projectRelevance = (techMatch + focusMatch) * priorityWeight;
            maxRelevance = Math.Max(maxRelevance, projectRelevance);
        }
        
        return maxRelevance;
    }
    
    private float CalculateUrgencyBoost(Article article, List<UserProject> projects, 
        ContextualizationSettings settings)
    {
        if (!settings.UrgencyBoostEnabled) return 0f;
        
        var urgentProjects = projects.Where(p => 
            p.Deadline.HasValue && 
            (p.Deadline.Value - DateTime.Now).TotalDays <= settings.UrgencyThresholdDays);
            
        return urgentProjects.Any(p => IsRelevantToProject(article, p)) ? 0.3f : 0f;
    }
}
```

### API de gestion des projets
```http
POST /api/v1/user/projects
Content-Type: application/json

{
  "name": "E-commerce Platform",
  "description": "Migration vers microservices",
  "technologies": ["ASP.NET Core", "React", "Azure", "Docker"],
  "focusAreas": ["Performance", "Scalability", "Security"],
  "priority": "High",
  "deadline": "2024-03-15T00:00:00Z"
}

GET /api/v1/recommendations/contextualized?focusRatio=0.8
Authorization: Bearer {token}

Response:
{
  "recommendations": [
    {
      "article": { /* article details */ },
      "contextualScore": 0.92,
      "relevantProjects": ["E-commerce Platform"],
      "urgencyLevel": "High",
      "reasonText": "Parfait pour votre projet e-commerce - deadline dans 14 jours"
    }
  ],
  "contextSummary": {
    "projectFocusedCount": 4,
    "diversityCount": 1,
    "urgentCount": 2
  }
}
```

## 📊 Métriques de succès

### Métriques de pertinence
- **Applicabilité directe** : 75% des articles contextualisés utilisés dans les projets
- **Satisfaction** : Score pertinence >4.3/5 pour suggestions contextualisées
- **Gain de temps** : 40% réduction du temps de recherche d'informations projet

### Métriques d'adoption  
- **Configuration** : 60% des utilisateurs configurent au moins 1 projet
- **Utilisation** : 80% des utilisateurs activent le mode contextualisation
- **Engagement** : +45% de temps de lecture sur articles contextualisés

### Métriques business
- **Productivité** : 25% amélioration perçue de l'efficacité projet (enquête)
- **Rétention** : +35% de sessions repeat avec feature activée
- **Découverte contextuelle** : 3x plus d'articles sauvegardés pour usage ultérieur

### Métriques techniques
- **Précision matching** : 85% des articles marqués "pertinents" le sont réellement
- **Performance** : Contextualisation <200ms pour 500 articles
- **Couverture** : 95% des technologies populaires reconnues automatiquement

## 🚀 Définition de terminé

Cette user story sera considérée comme terminée quand :

- [ ] **Gestion projets** : CRUD complet des projets utilisateur
- [ ] **Algorithme contextuel** : Score de pertinence projet fonctionnel
- [ ] **Interface configuration** : Écrans de configuration projets et paramètres
- [ ] **Suggestions adaptées** : Recommandations priorisent effectivement les projets
- [ ] **Gestion urgence** : Boost automatique selon deadlines
- [ ] **Tests complets** : Couverture des scénarios mono/multi-projets
- [ ] **Performance** : Benchmarks respectent les critères (<200ms)
- [ ] **Validation utilisateur** : Tests confirment l'utilité de la contextualisation

---

**Estimation** : 13 points  
**Assignee** : À définir  
**Sprint** : À planifier après US-002  
**Dependencies** : US-002 Adaptation niveau expertise, Authentification utilisateur

*Dernière mise à jour : 14 septembre 2025*