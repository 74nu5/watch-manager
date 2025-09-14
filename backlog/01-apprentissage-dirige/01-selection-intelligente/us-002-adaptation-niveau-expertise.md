# US-002 : Adaptation au niveau d'expertise

## 📝 Description

**En tant que** développeur avec un niveau technique spécifique  
**Je veux** que les articles suggérés correspondent à mon niveau de compétence  
**Afin de** ne pas perdre de temps sur des contenus trop basiques ou trop avancés

## 🎯 Critères d'acceptation

### Fonctionnalités principales
- [ ] **GIVEN** je suis un développeur junior  
      **WHEN** je reçois des suggestions d'articles  
      **THEN** 80% des articles sont de niveau "Débutant" ou "Intermédiaire"

- [ ] **GIVEN** je suis un expert en React  
      **WHEN** je vois des articles React suggérés  
      **THEN** ils sont de niveau "Avancé" ou "Expert" uniquement

- [ ] **GIVEN** je configure mon profil avec plusieurs niveaux d'expertise  
      **WHEN** les suggestions sont générées  
      **THEN** chaque article respecte mon niveau déclaré pour sa catégorie

### Comportements adaptatifs
- [ ] **APPRENTISSAGE** : Si je lis souvent des articles "Avancés", le système adapte mon profil
- [ ] **PROGRESSION** : Suggestions progressives d'articles légèrement plus complexes
- [ ] **FEEDBACK** : Possibilité de signaler qu'un article est "trop facile" ou "trop difficile"
- [ ] **MULTI-DOMAINE** : Niveaux différents par domaine technique (ex: Expert React, Junior Docker)

### Critères techniques
- [ ] **CLASSIFICATION** : Chaque article a un niveau défini automatiquement ou manuellement
- [ ] **PROFIL UTILISATEUR** : Stockage des niveaux par domaine/technologie
- [ ] **ÉVOLUTION** : Ajustement automatique basé sur le comportement de lecture
- [ ] **OVERRIDE** : Possibilité de forcer un niveau différent temporairement

## 🎨 Maquettes

### Configuration du profil d'expertise
```
┌─────────────────────────────────────────────────────────┐
│ 👤 Mon profil d'expertise technique                    │
├─────────────────────────────────────────────────────────┤
│                                                         │
│ 🎯 Frontend                                            │
│   React        ████████░░ Expert    [📈 Progresser]    │
│   Vue.js       ██████░░░░ Avancé                       │
│   Angular      ███░░░░░░░ Débutant                     │
│                                                         │
│ ⚙️ Backend                                              │
│   .NET         ██████████ Expert                       │
│   Node.js      ████████░░ Avancé                       │
│   Python       █████░░░░░ Intermédiaire               │
│                                                         │
│ 🐳 DevOps                                              │
│   Docker       ███████░░░ Avancé                       │
│   Kubernetes   ██░░░░░░░░ Débutant  [🎯 Objectif 2025] │
│                                                         │
│ [💾 Sauvegarder] [🔄 Détection auto] [📊 Voir stats]  │
└─────────────────────────────────────────────────────────┘
```

### Suggestions adaptées avec niveaux
```
┌─────────────────────────────────────────────────────────┐
│ 🎯 Suggestions adaptées à votre niveau                 │
├─────────────────────────────────────────────────────────┤
│                                                         │
│ 📄 Advanced React Patterns with Suspense               │
│     🏷️ React • Patterns • 📊 Expert                   │
│     💡 Parfait pour votre niveau React                 │
│     ⏱️ 20 min • 🔥 Tendance                            │
│                                                         │
│ 📄 Docker Multi-stage Builds Best Practices            │
│     🏷️ Docker • DevOps • 📊 Avancé                    │
│     💡 Correspond à votre niveau Docker                │
│     ⏱️ 12 min • 🎯 Recommandé                          │
│                                                         │
│ 📄 Kubernetes Basics: Pods and Services                │
│     🏷️ Kubernetes • 📊 Débutant                       │
│     💡 Parfait pour débuter en Kubernetes              │
│     ⏱️ 15 min • 🌱 Apprentissage                       │
│                                                         │
│ [👍 Niveaux corrects] [👎 Ajuster] [⚙️ Configurer]    │
└─────────────────────────────────────────────────────────┘
```

## 🧪 Scénarios de test

### Test 1 : Développeur junior
```gherkin
Given je suis un développeur junior (profil configuré)
And mon niveau React est "Débutant"  
And mon niveau .NET est "Intermédiaire"
When je demande des suggestions (5 articles)
Then au moins 3 articles sont de niveau "Débutant" ou "Intermédiaire"
And aucun article "Expert" n'est suggéré
And je vois des indicateurs de niveau sur chaque article
```

### Test 2 : Expert multi-domaines
```gherkin
Given je suis expert en React et .NET
And débutant en DevOps
When je reçois des suggestions
Then les articles React/NET sont de niveau "Avancé" ou "Expert"
And les articles DevOps sont de niveau "Débutant"
And la distribution respecte mes niveaux déclarés
```

### Test 3 : Progression automatique
```gherkin
Given je suis "Intermédiaire" en Vue.js
And j'ai lu 10 articles "Avancés" en Vue.js avec succès
When le système réévalue mon profil
Then mon niveau Vue.js passe automatiquement à "Avancé"
And les prochaines suggestions reflètent ce changement
```

## 🔧 Spécifications techniques

### Modèle de profil utilisateur
```csharp
public class UserExpertiseProfile
{
    public int UserId { get; set; }
    public List<DomainExpertise> Domains { get; set; }
    public DateTime LastUpdated { get; set; }
    public bool AutoLevelingEnabled { get; set; } = true;
}

public class DomainExpertise  
{
    public string Domain { get; set; } // "React", ".NET", "Docker"
    public ExpertiseLevel Level { get; set; }
    public float Confidence { get; set; } // 0-1, confiance du système
    public DateTime LastAssessed { get; set; }
    public List<ProgressionMilestone> Milestones { get; set; }
}

public enum ExpertiseLevel
{
    Beginner = 1,    // Débutant
    Intermediate = 2, // Intermédiaire  
    Advanced = 3,     // Avancé
    Expert = 4        // Expert
}
```

### Classification automatique des articles
```csharp
public class ArticleDifficultyClassifier
{
    public async Task<ExpertiseLevel> ClassifyArticleAsync(Article article)
    {
        var features = ExtractDifficultyFeatures(article);
        
        // Analyse de complexité basée sur :
        // - Vocabulaire technique utilisé
        // - Présence de concepts avancés  
        // - Structure et profondeur du contenu
        // - Prérequis implicites
        
        var score = await _mlModel.PredictDifficultyAsync(features);
        return MapScoreToLevel(score);
    }
    
    private DifficultyFeatures ExtractDifficultyFeatures(Article article)
    {
        return new DifficultyFeatures
        {
            TechnicalTermsDensity = CalculateTermsDensity(article.Content),
            ConceptComplexity = AnalyzeConceptDepth(article.Content),
            PrerequisitesCount = DetectPrerequisites(article.Content),
            CodeComplexity = AnalyzeCodeSamples(article.Content)
        };
    }
}
```

### Algorithme d'adaptation niveau
```csharp
public class ExpertiseAdaptationService
{
    public async Task<List<Article>> FilterByExpertiseAsync(
        List<Article> articles, UserExpertiseProfile profile)
    {
        var filteredArticles = new List<Article>();
        
        foreach (var article in articles)
        {
            var domain = ExtractPrimaryDomain(article);
            var userLevel = profile.GetLevelForDomain(domain);
            
            // Règle d'adaptation : niveau ±1 acceptable
            if (IsLevelAppropriate(article.DifficultyLevel, userLevel))
            {
                article.RelevanceScore = CalculateLevelRelevance(
                    article.DifficultyLevel, userLevel);
                filteredArticles.Add(article);
            }
        }
        
        return filteredArticles.OrderByDescending(a => a.RelevanceScore).ToList();
    }
    
    private bool IsLevelAppropriate(ExpertiseLevel articleLevel, ExpertiseLevel userLevel)
    {
        // Accepter articles de niveau -1 à +1 par rapport au niveau utilisateur
        var levelDiff = Math.Abs((int)articleLevel - (int)userLevel);
        return levelDiff <= 1;
    }
}
```

## 📊 Métriques de succès

### Métriques d'adaptation
- **Précision niveau** : 85% des articles suggérés jugés "de bon niveau"
- **Engagement** : +30% de temps de lecture sur articles adaptés vs non-adaptés
- **Complétion** : 70% des articles adaptés lus jusqu'au bout

### Métriques de progression
- **Détection auto** : 60% des changements de niveau détectés automatiquement
- **Progression utilisateur** : 20% des utilisateurs progressent d'un niveau par trimestre
- **Satisfaction** : Score de pertinence >4.2/5 pour les suggestions par niveau

### Métriques techniques
- **Classification** : 80% de précision sur la classification automatique des niveaux
- **Performance** : Filtrage par expertise <100ms pour 1000 articles
- **Évolution profil** : Mise à jour des niveaux en temps réel

## 🚀 Définition de terminé

Cette user story sera considérée comme terminée quand :

- [ ] **Profil expertise** : Interface de configuration complète et intuitive
- [ ] **Classification auto** : Algorithme de classification des articles par niveau
- [ ] **Filtrage adaptatif** : Suggestions respectent les niveaux configurés
- [ ] **Progression auto** : Détection et ajustement automatique des niveaux
- [ ] **Tests** : Couverture complète des scénarios d'adaptation
- [ ] **Performance** : Benchmarks respectent les critères de performance
- [ ] **UX** : Tests utilisateur confirment l'utilité de l'adaptation

---

**Estimation** : 8 points  
**Assignee** : À définir  
**Sprint** : À planifier après US-001  
**Dependencies** : US-001 Sélection articles non consultés

*Dernière mise à jour : ${new Date().toLocaleDateString('fr-FR')}*
