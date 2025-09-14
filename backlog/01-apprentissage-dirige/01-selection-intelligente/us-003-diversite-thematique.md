# US-003 : Diversité thématique garantie

## 📝 Description

**En tant que** utilisateur en veille technique  
**Je veux** que les suggestions couvrent différents domaines techniques  
**Afin de** maintenir une culture générale tech équilibrée

## 🎯 Critères d'acceptation

### Fonctionnalités principales
- [ ] **GIVEN** je demande 5 suggestions d'articles  
      **WHEN** le système génère la liste  
      **THEN** au maximum 2 articles proviennent de la même catégorie

- [ ] **GIVEN** j'ai lu principalement des articles Frontend cette semaine  
      **WHEN** je reçois de nouvelles suggestions  
      **THEN** au moins 60% des articles appartiennent à d'autres domaines

- [ ] **GIVEN** il y a 10 catégories disponibles en base  
      **WHEN** je reçois 5 suggestions  
      **THEN** elles couvrent au minimum 3 catégories différentes

### Comportements de diversification
- [ ] **ROTATION** : Éviter les doublons de catégories sur plusieurs sessions consécutives
- [ ] **ÉQUILIBRAGE** : Prioriser les domaines moins consultés récemment
- [ ] **DÉCOUVERTE** : Inclure systématiquement au moins 1 article d'un domaine peu exploré
- [ ] **PRÉFÉRENCES** : Respecter les exclusions explicites (ex: "Pas de mobile")

### Critères techniques
- [ ] **ALGORITHME** : Coefficient de diversité Gini < 0.6 sur les suggestions
- [ ] **CONFIGURATION** : Possibilité de définir des quotas par catégorie
- [ ] **MESURE** : Tracking de la distribution thématique par utilisateur
- [ ] **PERFORMANCE** : Diversification calculée en <100ms

## 🎨 Maquettes

### Suggestions avec diversité visualisée
```
┌─────────────────────────────────────────────────────────┐
│ 🎯 Votre sélection diversifiée                         │
├─────────────────────────────────────────────────────────┤
│                                                         │
│ 🎨 Frontend                                            │
│ 📄 React Server Components: The Complete Guide         │
│     ⏱️ 12 min • 🏷️ React, SSR • 📊 Avancé             │
│                                                         │
│ ⚙️ Backend                                              │
│ 📄 Building Microservices with .NET 8                  │
│     ⏱️ 15 min • 🏷️ .NET, Architecture • 📊 Expert     │
│                                                         │
│ 🐳 DevOps                                              │
│ 📄 Kubernetes Security Best Practices                  │
│     ⏱️ 10 min • 🏷️ K8s, Security • 📊 Intermédiaire  │
│                                                         │
│ 📊 Data                                                │
│ 📄 Introduction to Vector Databases                    │
│     ⏱️ 8 min • 🏷️ Database, AI • 📊 Débutant          │
│                                                         │
│ 🔒 Security                                            │
│ 📄 OAuth 2.0 vs OpenID Connect Explained              │
│     ⏱️ 6 min • 🏷️ Auth, Standards • 📊 Intermédiaire │
│                                                         │
│ 📈 Distribution: Frontend(1) Backend(1) DevOps(1)      │
│                 Data(1) Security(1)                    │
│                                                         │
│ [🔄 Nouvelle sélection] [⚙️ Ajuster diversité]        │
└─────────────────────────────────────────────────────────┘
```

### Configuration de la diversité
```
┌─────────────────────────────────────────────────────────┐
│ ⚙️ Configuration de la diversité                       │
├─────────────────────────────────────────────────────────┤
│                                                         │
│ 🎯 Quotas par domaine (sur 5 articles)                │
│                                                         │
│ Frontend      ▓▓░░░ 2 max   [📈 Souvent lu]           │
│ Backend       ▓▓░░░ 2 max   [📈 Souvent lu]           │
│ DevOps        ▓░░░░ 1 max   [📊 Peu lu]               │
│ Mobile        ░░░░░ 0       [❌ Exclu]                 │
│ Data          ▓░░░░ 1 max   [🆕 À découvrir]          │
│ Security      ▓░░░░ 1 max   [📊 Peu lu]               │
│ IA/ML         ▓░░░░ 1 max   [🆕 À découvrir]          │
│                                                         │
│ 🎲 Mode découverte                                     │
│ ☑ Forcer 1 article de domaine non-exploré             │
│ ☑ Éviter >2 articles de même catégorie                │
│ ☐ Prioriser les lacunes détectées                     │
│                                                         │
│ [💾 Sauvegarder] [🔄 Réinitialiser] [📊 Voir stats]  │
└─────────────────────────────────────────────────────────┘
```

## 🧪 Scénarios de test

### Test 1 : Diversification basique
```gherkin
Given il y a 100 articles répartis sur 6 catégories
And je demande 5 suggestions
When l'algorithme génère la sélection
Then aucune catégorie n'a plus de 2 articles représentés
And au moins 3 catégories différentes sont présentes
And le coefficient de Gini de distribution est < 0.6
```

### Test 2 : Utilisateur spécialisé
```gherkin
Given j'ai lu 90% d'articles "Frontend" le mois dernier
And seulement 10% d'articles autres domaines
When je reçois 5 nouvelles suggestions
Then maximum 2 articles Frontend sont proposés
And au moins 3 articles proviennent d'autres domaines
And je vois un indicateur "Équilibrage recommandé"
```

### Test 3 : Configuration personnalisée
```gherkin
Given j'ai exclu la catégorie "Mobile" de mes préférences
And j'ai défini quota max 1 pour "DevOps"
When le système génère des suggestions
Then aucun article "Mobile" n'est proposé
And maximum 1 article "DevOps" est inclus
And la diversification respecte mes contraintes
```

## 🔧 Spécifications techniques

### Algorithme de diversification
```csharp
public class DiversificationService
{
    public async Task<List<Article>> DiversifySelectionAsync(
        List<Article> candidateArticles, 
        DiversificationConfig config)
    {
        var diversifiedSelection = new List<Article>();
        var categoryQuotas = InitializeCategoryQuotas(config);
        
        // 1. Trier les articles par score de pertinence
        var sortedArticles = candidateArticles
            .OrderByDescending(a => a.RelevanceScore)
            .ToList();
        
        // 2. Sélectionner en respectant les quotas de diversité
        foreach (var article in sortedArticles)
        {
            if (CanAddArticle(article, categoryQuotas, diversifiedSelection))
            {
                diversifiedSelection.Add(article);
                UpdateQuotas(categoryQuotas, article.Category);
                
                if (diversifiedSelection.Count >= config.MaxArticles)
                    break;
            }
        }
        
        // 3. Vérifier les critères de diversité
        ValidateDiversityMetrics(diversifiedSelection, config);
        
        return diversifiedSelection;
    }
    
    private bool CanAddArticle(Article article, 
        Dictionary<string, int> quotas, List<Article> selection)
    {
        var categoryCount = selection.Count(a => a.Category == article.Category);
        var maxForCategory = quotas.GetValueOrDefault(article.Category, 2);
        
        return categoryCount < maxForCategory;
    }
}
```

### Métriques de diversité
```csharp
public class DiversityMetrics
{
    public static double CalculateGiniCoefficient(List<Article> articles)
    {
        var categoryDistribution = articles
            .GroupBy(a => a.Category)
            .Select(g => (double)g.Count())
            .OrderBy(count => count)
            .ToArray();
        
        if (categoryDistribution.Length <= 1) return 0;
        
        double sum = 0;
        for (int i = 0; i < categoryDistribution.Length; i++)
        {
            sum += (2 * (i + 1) - categoryDistribution.Length - 1) 
                   * categoryDistribution[i];
        }
        
        return sum / (categoryDistribution.Length * categoryDistribution.Sum());
    }
    
    public static DiversityReport GenerateReport(List<Article> articles)
    {
        return new DiversityReport
        {
            TotalCategories = articles.Select(a => a.Category).Distinct().Count(),
            GiniCoefficient = CalculateGiniCoefficient(articles),
            CategoryDistribution = articles
                .GroupBy(a => a.Category)
                .ToDictionary(g => g.Key, g => g.Count()),
            DiversityScore = CalculateDiversityScore(articles)
        };
    }
}
```

### Configuration utilisateur
```csharp
public class DiversificationConfig
{
    public int MaxArticles { get; set; } = 5;
    public Dictionary<string, int> CategoryQuotas { get; set; } = new();
    public List<string> ExcludedCategories { get; set; } = new();
    public bool ForceNewDomainExploration { get; set; } = true;
    public double MinDiversityScore { get; set; } = 0.6;
    public bool PreventCategoryDomination { get; set; } = true;
}
```

## 📊 Métriques de succès

### Métriques de diversité
- **Distribution équilibrée** : Coefficient Gini < 0.6 sur 80% des suggestions
- **Couverture thématique** : Moyenne de 4+ catégories sur 5 articles suggérés
- **Exploration** : 30% des utilisateurs découvrent 1+ nouveau domaine/mois

### Métriques d'engagement
- **Temps de lecture** : +25% sur articles de domaines diversifiés
- **Satisfaction diversité** : >4/5 dans les enquêtes utilisateur
- **Réduction spécialisation** : -40% d'utilisateurs concentrés sur 1 seul domaine

### Métriques techniques
- **Performance** : Algorithme de diversification <100ms
- **Qualité** : 90% des sélections respectent les quotas configurés
- **Cohérence** : Diversité maintenue sur 10 sessions consécutives

## 🚀 Définition de terminé

Cette user story sera considérée comme terminée quand :

- [ ] **Algorithme** : Diversification fonctionnelle avec métriques Gini
- [ ] **Configuration** : Interface utilisateur pour définir quotas et exclusions
- [ ] **Visualisation** : Affichage de la distribution thématique
- [ ] **Tests** : Scénarios de diversification couverts à 100%
- [ ] **Performance** : Benchmarks respectent les critères (<100ms)
- [ ] **UX** : Tests utilisateur confirment l'amélioration de la découverte

---

**Estimation** : 5 points  
**Assignee** : À définir  
**Sprint** : À planifier après US-002  
**Dependencies** : US-001, US-002

*Dernière mise à jour : ${new Date().toLocaleDateString('fr-FR')}*
