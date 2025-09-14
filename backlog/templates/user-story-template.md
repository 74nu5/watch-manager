# US-XXX : [TITRE_USER_STORY]

## 📝 Description

**En tant que** [persona/rôle utilisateur]  
**Je veux** [action/fonctionnalité souhaitée]  
**Afin de** [bénéfice/valeur pour l'utilisateur]

## 🎯 Critères d'acceptation

### Fonctionnalités principales
- [ ] **GIVEN** [condition initiale]  
      **WHEN** [action utilisateur]  
      **THEN** [résultat attendu]

- [ ] **GIVEN** [condition initiale]  
      **WHEN** [action utilisateur]  
      **THEN** [résultat attendu]

- [ ] **GIVEN** [condition initiale]  
      **WHEN** [action utilisateur]  
      **THEN** [résultat attendu]

### Comportements attendus
- [ ] **[Comportement 1]** : [Description du comportement]
- [ ] **[Comportement 2]** : [Description du comportement]
- [ ] **[Comportement 3]** : [Description du comportement]

### Critères techniques
- [ ] **[Critère technique 1]** : [Spécification avec métrique]
- [ ] **[Critère technique 2]** : [Spécification avec métrique]
- [ ] **[Critère technique 3]** : [Spécification]

## 🎨 Maquettes

### [Interface principale]
```
┌─────────────────────────────────────────────────────────┐
│ [Description de l'interface utilisateur]               │
├─────────────────────────────────────────────────────────┤
│ [Mockup textuel de l'interface]                        │
│ [Éléments, boutons, inputs, etc.]                      │
│ [Layout et organisation]                               │
└─────────────────────────────────────────────────────────┘
```

### [État d'erreur/edge case]
```
┌─────────────────────────────────────────────────────────┐
│ [Mockup des états d'erreur ou cas limites]            │
└─────────────────────────────────────────────────────────┘
```

## 🧪 Scénarios de test

### Test 1 : [Nom du scénario]
```gherkin
Given [condition initiale détaillée]
And [condition supplémentaire]
When [action utilisateur spécifique]
Then [résultat attendu précis]
And [vérification supplémentaire]
```

### Test 2 : [Nom du scénario]
```gherkin
Given [condition initiale]
And [données de test spécifiques]
When [action utilisateur]
Then [résultat attendu]
And [métrique à vérifier]
```

### Test 3 : [Cas limite/erreur]
```gherkin  
Given [condition d'erreur]
When [action qui provoque l'erreur]
Then [gestion d'erreur attendue]
And [message/comportement approprié]
```

## 🔧 Spécifications techniques

### Modèle de données
```csharp
public class [NomModel]
{
    public int Id { get; set; }
    public string [Propriété1] { get; set; }
    public DateTime [Propriété2] { get; set; }
    // Autres propriétés...
}
```

### API Contract
```http
[METHOD] /api/v1/[endpoint]?[params]
Authorization: Bearer {token}

Request Body:
{
  "[propriété1]": "[valeur]",
  "[propriété2]": "[valeur]"
}

Response:
{
  "[propriété1]": "[valeur]",
  "[propriété2]": "[valeur]",
  "meta": {
    "total": 42,
    "timestamp": "2024-12-19T10:30:00Z"
  }
}
```

### Algorithme/Logique métier
```csharp
public async Task<[ReturnType]> [MethodName]Async([Parameters])
{
    // 1. [Étape 1 de l'algorithme]
    
    // 2. [Étape 2 de l'algorithme]
    
    // 3. [Étape 3 avec logique métier]
    
    return [result];
}
```

## 📊 Métriques de succès

### Métriques utilisateur
- **[Métrique engagement 1]** : [Valeur cible] ([Description])
- **[Métrique satisfaction 1]** : [Valeur cible] ([Méthode de mesure])
- **[Métrique usage 1]** : [Valeur cible] ([Fréquence])

### Métriques techniques  
- **[Performance 1]** : [Valeur cible] ([Conditions])
- **[Qualité 1]** : [Valeur cible] ([Méthode de mesure])
- **[Fiabilité 1]** : [Valeur cible] ([Conditions])

### Métriques business
- **[Impact business 1]** : [Valeur cible] ([Période])
- **[Adoption 1]** : [Valeur cible] ([Définition])

## 🚀 Définition de terminé

Cette user story sera considérée comme terminée quand :

- [ ] **Code** : [Critère de code complet]
- [ ] **Tests** : [Critère de couverture/qualité tests]
- [ ] **Performance** : [Critère de performance respecté]
- [ ] **Documentation** : [Critère de documentation]
- [ ] **UX** : [Critère d'expérience utilisateur]
- [ ] **Intégration** : [Critère d'intégration système]

---

**Estimation** : [X] points  
**Assignee** : [À définir/Nom développeur]  
**Sprint** : [À planifier/Sprint X]  
**Dependencies** : [Liste des dépendances]

*Dernière mise à jour : [Date]*
