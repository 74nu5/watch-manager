# Epic : Apprentissage dirigé

## 🎯 Vision

Permettre aux utilisateurs d'apprendre de manière dirigée et intelligente en proposant des contenus adaptés à leur niveau, leurs lacunes et leurs objectifs professionnels.

## 📋 Description

L'apprentissage dirigé constitue le cœur de l'accompagnement intelligent de Watch Manager. Cette epic vise à transformer la veille passive en un processus d'apprentissage actif et personnalisé, où l'IA guide l'utilisateur vers les contenus les plus pertinents pour son développement professionnel.

## 🎯 Objectifs business

- **Engagement** : Augmenter le temps passé quotidiennement sur la veille de 15 à 45 minutes
- **Efficacité** : Réduire de 60% le temps de recherche de solutions techniques
- **Apprentissage** : Faire découvrir 3 nouveaux sujets techniques par mois minimum
- **Rétention** : Améliorer la mémorisation des concepts via l'apprentissage adaptatif

## 👥 Personas cibles

### 👨‍💻 Développeur Junior
- **Besoin** : Apprentissage structuré et progressif
- **Défi** : Éviter la surcharge cognitive
- **Valeur** : Parcours d'apprentissage guidé avec prérequis

### 👩‍💻 Développeur Senior
- **Besoin** : Veille sur technologies émergentes
- **Défi** : Manque de temps pour exploration
- **Valeur** : Découverte ciblée et sérendipité contrôlée

### 🏗️ Architecte technique
- **Besoin** : Vision transversale et tendances
- **Défi** : Maintien de l'expertise multi-domaines
- **Valeur** : Synthèse des interconnexions technologiques

## 📊 Critères d'acceptation epic

- [ ] **Algorithmes de recommandation** fonctionnels avec scoring de pertinence
- [ ] **Profils d'apprentissage** configurables par utilisateur
- [ ] **Métriques de progression** mesurables et visualisées
- [ ] **Système de gamification** engageant sans être intrusif
- [ ] **Intelligence prédictive** pour anticiper les besoins d'apprentissage

## 🏗️ Features incluses

1. **[Sélection intelligente de contenus](01-selection-intelligente/)** - P0
2. **[Filtrage par domaine de compétence](02-filtrage-competences/)** - P1  
3. **[Génération d'exercices pratiques par IA](03-exercices-ia/)** - P1
4. **[Suivi de progression avancé](04-suivi-progression/)** - P0
5. **[Mécanismes de randomisation intelligente](05-randomisation/)** - P2
6. **[Personnalisation cognitive](06-personnalisation/)** - P2

## 🔗 Dépendances

### Prérequis techniques
- ✅ Système d'embeddings vectoriels existant
- ✅ Classification automatique par IA  
- ✅ API de gestion des articles et catégories
- 🔄 Système d'authentification utilisateur
- 🔮 Profils utilisateur étendus

### Intégrations
- **Service IA** : Extension des capacités d'analyse existantes
- **Base de données** : Nouvelles tables pour profils et progression
- **API** : Nouveaux endpoints pour recommandations et métriques
- **Interface** : Nouveaux composants Blazor pour tableaux de bord

## 📈 Métriques de succès

### Métriques d'engagement
- **Temps de session moyen** : > 30 minutes
- **Fréquence d'utilisation** : 5+ jours par semaine
- **Taux de complétion des sessions** : > 80%

### Métriques d'apprentissage  
- **Nouveaux sujets explorés** : 3+ par mois
- **Score de rétention** : Mesure via quiz automatiques
- **Progression des compétences** : Évaluation qualitative

### Métriques techniques
- **Précision des recommandations** : > 75% de pertinence
- **Temps de réponse des algorithmes** : < 500ms
- **Adoption des fonctionnalités** : > 60% des utilisateurs actifs

## 🗓️ Timeline

- **Q1 2026** : Sélection intelligente + Suivi progression (MVP)
- **Q2 2026** : Filtrage compétences + Exercices IA
- **Q3 2026** : Randomisation + Personnalisation cognitive
- **Q4 2026** : Optimisations et intelligence prédictive

## 🚀 MVP Definition

Le MVP de l'apprentissage dirigé inclut :
1. **Algorithme de base** pour sélectionner des articles non consultés
2. **Filtrage simple** par tags et catégories existants  
3. **Métriques basiques** : articles lus, temps passé, sujets couverts
4. **Interface minimale** : bouton "Surprise moi !" avec 3 articles suggérés

## 🎯 Success Criteria

L'epic sera considérée comme réussie si :
- Les utilisateurs passent **2x plus de temps** en veille active qu'avant
- **80% des recommandations** sont jugées pertinentes par les utilisateurs  
- **50% des nouveaux sujets** découverts sont réutilisés dans des projets
- Le **Net Promoter Score** pour la fonctionnalité dépasse 7/10

---

**Status** : 🔮 Planifié  
**Priority** : P0 - Critique  
**Effort** : 13 points (Large)  
**Risk** : Medium - Complexité algorithmique et UX  

*Dernière mise à jour : ${new Date().toLocaleDateString('fr-FR')}*
