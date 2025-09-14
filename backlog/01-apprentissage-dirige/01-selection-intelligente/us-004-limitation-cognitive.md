# US-004 : Limitation cognitive intelligente

## 📝 Description

**En tant que** professionnel avec un temps limité  
**Je veux** que le système limite le nombre de nouveaux concepts par session  
**Afin de** pouvoir assimiler efficacement les informations sans surcharge

## 🎯 Critères d'acceptation

### Fonctionnalités principales
- [ ] **GIVEN** je configure ma capacité d'apprentissage à "Modérée" (2 nouveaux concepts/session)  
      **WHEN** le système génère des suggestions  
      **THEN** maximum 2 articles contiennent des technologies que je ne connais pas

- [ ] **GIVEN** j'ai déjà lu un article sur un nouveau framework aujourd'hui  
      **WHEN** je demande de nouvelles suggestions  
      **THEN** les autres articles proposés utilisent des technologies familières

- [ ] **GIVEN** j'ai 15 minutes de temps disponible  
      **WHEN** je consulte mes suggestions  
      **THEN** la charge cognitive totale estimée ne dépasse pas ma limite configurée

### Comportements adaptatifs
- [ ] **DÉTECTION AUTO** : Identifier automatiquement les "nouveaux concepts" via analyse sémantique
- [ ] **PROGRESSION** : Augmenter graduellement la charge cognitive selon les succès d'apprentissage
- [ ] **CONTEXTE TEMPS** : Adapter selon le moment de la journée (moins de nouveauté en fin de journée)
- [ ] **FEEDBACK** : Permettre de signaler "trop complexe" ou "trop simple"

### Critères techniques
- [ ] **SCORING COMPLEXITÉ** : Algorithme de calcul de charge cognitive par article
- [ ] **PROFIL UTILISATEUR** : Stockage des préférences et capacités d'apprentissage
- [ ] **HISTORIQUE** : Tracking des nouveaux concepts introduits par période
- [ ] **PERFORMANCE** : Calcul de charge cognitive en <200ms

## 🎨 Maquettes

### Configuration de la charge cognitive
```
┌─────────────────────────────────────────────────────────┐
│ 🧠 Gestion de la charge cognitive                      │
├─────────────────────────────────────────────────────────┤
│                                                         │
│ 🎯 Ma capacité d'apprentissage                         │
│                                                         │
│ ○ Faible      (1 nouveau concept par session)          │
│ ●Modérée     (2-3 nouveaux concepts par session)      │
│ ○ Élevée      (4+ nouveaux concepts par session)       │
│ ○ Variable    (Adapte selon le contexte)               │
│                                                         │
│ ⏰ Temps disponible par session                        │
│ [████████░░] 20 minutes                                │
│                                                         │
│ 🧩 Types de nouveauté acceptés                         │
│ ☑ Nouveaux frameworks/librairies                      │
│ ☑ Nouveaux concepts architecturaux                    │
│ ☐ Nouveaux langages de programmation                  │
│ ☑ Nouvelles pratiques/méthodologies                   │
│                                                         │
│ 📊 Adaptation intelligente                             │
│ ☑ Réduire la complexité en fin de journée             │
│ ☑ Augmenter graduellement selon mes progrès           │
│ ☐ Mode focus : que des sujets familiers               │
│                                                         │
│ [💾 Sauvegarder] [📊 Voir historique] [🔄 Test]       │
└─────────────────────────────────────────────────────────┘
```

### Suggestions avec indicateurs de charge
```
┌─────────────────────────────────────────────────────────┐
│ 🎯 Votre sélection adaptée (Charge: Modérée 🟡)       │
├─────────────────────────────────────────────────────────┤
│                                                         │
│ 📄 React Query vs SWR: Performance Comparison          │
│     🧩 Familier • React, État global • ⏱️ 8 min       │
│     💡 Concepts connus, lecture fluide                 │
│                                                         │
│ 📄 Introduction to Deno 2.0 🆕                        │
│     🧩 Nouveauté • Runtime, TypeScript • ⏱️ 12 min    │
│     💡 1er nouveau concept de la session               │
│                                                         │
│ 📄 Advanced TypeScript Generics Patterns               │
│     🧩 Familier • TypeScript, Patterns • ⏱️ 15 min    │
│     💡 Approfondissement de vos connaissances          │
│                                                         │
│ 📄 Micro-frontends with Module Federation 🆕           │
│     🧩 Nouveauté • Architecture, Webpack • ⏱️ 18 min  │
│     💡 2ème nouveau concept - limite atteinte          │
│                                                         │
│ ⚠️ Articles filtrés : 3 articles trop complexes        │
│   pour votre capacité actuelle                         │
│                                                         │
│ 📊 Charge cognitive: 2/3 nouveaux concepts             │
│ ⏱️ Temps estimé: 53 min (dans votre limite)           │
│                                                         │
│ [🔄 Nouvelles suggestions] [📈 Augmenter limite]       │
│ [📋 Voir articles filtrés] [⚙️ Ajuster]              │
└─────────────────────────────────────────────────────────┘
```

## 🧪 Scénarios de test

### Test 1 : Limitation basique
```gherkin
Given je configure ma capacité à "2 nouveaux concepts par session"
And il y a 20 articles disponibles dont 10 avec des nouveautés
When le système génère 5 suggestions
Then maximum 2 articles contiennent des technologies inconnues
And les 3 autres articles utilisent mes technologies familières
And la charge cognitive totale respecte ma limite
```

### Test 2 : Adaptation temporelle
```gherkin
Given je consulte des suggestions à 17h (fin de journée)
And j'ai la configuration "Réduire complexité en fin de journée" activée
When les suggestions sont générées
Then la charge cognitive est réduite de 50% par rapport au matin
And plus d'articles "Familier" sont privilégiés
```

### Test 3 : Progression adaptative
```gherkin
Given j'ai réussi à assimiler 5 nouveaux concepts cette semaine
And mon score de compréhension moyen est >80%
When le système réévalue ma capacité
Then ma limite est augmentée progressivement (+1 concept)
And je reçois une notification de progression
```

## 🔧 Spécifications techniques

### Modèle de charge cognitive
```csharp
public class CognitiveLoadCalculator
{
    public async Task<CognitiveLoad> CalculateArticleLoadAsync(
        Article article, UserKnowledgeProfile userProfile)
    {
        var concepts = await ExtractConceptsAsync(article);
        var unknownConcepts = concepts
            .Where(c => !userProfile.IsKnownConcept(c))
            .ToList();
        
        return new CognitiveLoad
        {
            ArticleId = article.Id,
            TotalConcepts = concepts.Count,
            UnknownConcepts = unknownConcepts.Count,
            ComplexityScore = CalculateComplexityScore(article, unknownConcepts),
            EstimatedProcessingTime = EstimateProcessingTime(article, unknownConcepts),
            IsWithinUserCapacity = IsWithinCapacity(unknownConcepts.Count, userProfile)
        };
    }
    
    private double CalculateComplexityScore(Article article, List<Concept> unknownConcepts)
    {
        var baseComplexity = article.DifficultyLevel switch
        {
            ExpertiseLevel.Beginner => 1.0,
            ExpertiseLevel.Intermediate => 2.0,
            ExpertiseLevel.Advanced => 3.0,
            ExpertiseLevel.Expert => 4.0,
            _ => 2.0
        };
        
        var noveltyPenalty = unknownConcepts.Count * 0.5;
        var conceptComplexity = unknownConcepts.Sum(c => c.ComplexityWeight);
        
        return baseComplexity + noveltyPenalty + conceptComplexity;
    }
}
```

### Profil de capacité utilisateur
```csharp
public class UserCognitiveProfile
{
    public int UserId { get; set; }
    public CognitiveCapacity Capacity { get; set; }
    public int MaxNewConceptsPerSession { get; set; }
    public TimeSpan MaxSessionDuration { get; set; }
    public List<string> KnownTechnologies { get; set; } = new();
    public List<ConceptCategory> AcceptedNoveltyTypes { get; set; } = new();
    public bool ReduceComplexityInEvening { get; set; } = true;
    public bool EnableProgressiveAdaptation { get; set; } = true;
    public double CurrentCapacityMultiplier { get; set; } = 1.0;
    public DateTime LastCapacityUpdate { get; set; }
}

public enum CognitiveCapacity
{
    Low = 1,      // 1 nouveau concept
    Moderate = 2, // 2-3 nouveaux concepts  
    High = 3,     // 4+ nouveaux concepts
    Variable = 4  // Adaptatif selon contexte
}
```

### Algorithme de sélection adaptative
```csharp
public class CognitiveLoadAwareSelector
{
    public async Task<List<Article>> SelectArticlesAsync(
        List<Article> candidates, UserCognitiveProfile profile)
    {
        var articlesWithLoad = new List<(Article Article, CognitiveLoad Load)>();
        
        // 1. Calculer la charge cognitive pour chaque article
        foreach (var article in candidates)
        {
            var load = await _cognitiveCalculator
                .CalculateArticleLoadAsync(article, profile);
            articlesWithLoad.Add((article, load));
        }
        
        // 2. Séparer articles familiers vs nouveaux
        var familiarArticles = articlesWithLoad
            .Where(x => x.Load.UnknownConcepts == 0)
            .OrderByDescending(x => x.Article.RelevanceScore)
            .ToList();
            
        var novelArticles = articlesWithLoad
            .Where(x => x.Load.UnknownConcepts > 0)
            .OrderBy(x => x.Load.ComplexityScore)
            .ThenByDescending(x => x.Article.RelevanceScore)
            .ToList();
        
        // 3. Composer la sélection finale
        return ComposeBalancedSelection(familiarArticles, novelArticles, profile);
    }
    
    private List<Article> ComposeBalancedSelection(
        List<(Article, CognitiveLoad)> familiar,
        List<(Article, CognitiveLoad)> novel,
        UserCognitiveProfile profile)
    {
        var selection = new List<Article>();
        var newConceptsCount = 0;
        var totalTime = TimeSpan.Zero;
        
        // Ajouter des articles nouveaux jusqu'à la limite
        foreach (var (article, load) in novel)
        {
            if (newConceptsCount + load.UnknownConcepts <= profile.MaxNewConceptsPerSession
                && totalTime + load.EstimatedProcessingTime <= profile.MaxSessionDuration)
            {
                selection.Add(article);
                newConceptsCount += load.UnknownConcepts;
                totalTime += load.EstimatedProcessingTime;
            }
        }
        
        // Compléter avec des articles familiers
        foreach (var (article, load) in familiar)
        {
            if (selection.Count >= 5) break;
            if (totalTime + load.EstimatedProcessingTime <= profile.MaxSessionDuration)
            {
                selection.Add(article);
                totalTime += load.EstimatedProcessingTime;
            }
        }
        
        return selection;
    }
}
```

## 📊 Métriques de succès

### Métriques de charge cognitive
- **Respect des limites** : 95% des sessions respectent la capacité configurée
- **Satisfaction cognitive** : Score de "surcharge" <2/10 dans les enquêtes
- **Complétion** : 80% des articles suggérés lus en entier (vs abandon)

### Métriques d'apprentissage
- **Assimilation** : 75% des nouveaux concepts sont réutilisés dans la semaine
- **Progression** : 40% des utilisateurs voient leur capacité augmenter sur 3 mois
- **Rétention** : 85% de rétention des concepts à 7 jours (via quiz)

### Métriques techniques
- **Performance** : Calcul de charge cognitive <200ms
- **Précision** : 90% de précision dans la détection des nouveaux concepts
- **Adaptation** : Ajustement de capacité en temps réel selon le contexte

## 🚀 Définition de terminé

Cette user story sera considérée comme terminée quand :

- [ ] **Algorithme** : Calcul de charge cognitive fonctionnel et précis
- [ ] **Configuration** : Interface complète pour définir capacités et préférences
- [ ] **Adaptation** : Système d'ajustement automatique selon les progrès
- [ ] **Visualisation** : Indicateurs clairs de charge cognitive dans les suggestions
- [ ] **Tests** : Scénarios de limitation cognitive couverts à 100%
- [ ] **Performance** : Benchmarks de calcul respectent les critères
- [ ] **Validation** : Tests utilisateur confirment la réduction de surcharge

---

**Estimation** : 8 points  
**Assignee** : À définir  
**Sprint** : À planifier après US-003  
**Dependencies** : US-001, US-002 (profils utilisateur)

*Dernière mise à jour : ${new Date().toLocaleDateString('fr-FR')}*
