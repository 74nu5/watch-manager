# Feature : Lecture et affichage des flux

## 🎯 Objectif

Créer une interface de lecture unifiée permettant de consulter tous les articles importés depuis les flux RSS, avec filtres par source, catégorie, date et statut de lecture, dans une expérience utilisateur fluide et intuitive.

## 📋 Description

Cette feature transforme Watch Manager en un véritable agrégateur RSS moderne. Elle offre une vue consolidée de tous les articles importés depuis les flux, avec des fonctionnalités de tri, filtrage, recherche et marquage comme lu/non-lu. L'interface doit être comparable aux agrégateurs populaires comme Feedly ou Inoreader, tout en conservant l'identité visuelle de Watch Manager basée sur FluentUI.

## 👤 User Persona

**Sophie, Tech Lead (8 ans d'expérience)**
- Suit 35 flux RSS quotidiennement
- Consulte sa veille le matin (20 min) et à midi (15 min)
- Veut une vue "tout en un" puis filtrer par catégorie
- Marque systématiquement les articles lus pour suivre sa progression

## 🎯 User Stories

### [US-001 : Vue agrégée de tous les articles](us-001-vue-agregee-articles.md)
**En tant que** utilisateur avec plusieurs flux RSS  
**Je veux** voir une liste unifiée de tous les nouveaux articles  
**Afin de** consulter ma veille depuis un seul endroit

### [US-002 : Filtrage par source et catégorie](us-002-filtrage-source-categorie.md)
**En tant que** utilisateur  
**Je veux** filtrer les articles par flux source ou catégorie  
**Afin de** me concentrer sur des thématiques spécifiques

### [US-003 : Marquage lu/non-lu et archivage](us-003-marquage-lu-archivage.md)
**En tant que** lecteur assidu  
**Je veux** marquer des articles comme lus et les archiver  
**Afin de** suivre ma progression et ne pas revoir les mêmes contenus

### [US-004 : Aperçu et lecture rapide](us-004-apercu-lecture-rapide.md)
**En tant que** utilisateur pressé  
**Je veux** prévisualiser les articles sans ouvrir le lien complet  
**Afin de** décider rapidement si l'article m'intéresse

### [US-005 : Actions groupées sur articles](us-005-actions-groupees.md)
**En tant que** utilisateur avec beaucoup de contenu  
**Je veux** effectuer des actions en masse (marquer tous comme lus, archiver)  
**Afin de** gérer efficacement ma liste d'articles

## 🔧 Critères d'acceptation techniques

### Interface de lecture
- [ ] **Vue liste** : Affichage compact avec titre, source, date, extrait
- [ ] **Vue carte** : Affichage enrichi avec image, tags, estimation temps de lecture
- [ ] **Pagination** : Chargement progressif avec scroll infini ou pagination classique
- [ ] **Performance** : Rendu de 100 articles en <500ms

### Filtres et recherche
- [ ] **Filtres combinés** : Source + Catégorie + Date + Statut (lu/non-lu)
- [ ] **Recherche full-text** : Dans titre et description des articles
- [ ] **Tri** : Par date (récent/ancien), par source, par popularité
- [ ] **Persistance** : Sauvegarde des filtres actifs entre sessions

### Gestion des statuts
- [ ] **Marquage automatique** : Article marqué lu après X secondes de lecture
- [ ] **Marquage manuel** : Bouton explicite pour marquer lu/non-lu
- [ ] **Archivage** : Déplacement des articles lus hors de la vue principale
- [ ] **Synchronisation** : Statuts persistés en temps réel

## 🎨 Maquettes et UX

### Vue principale - Liste des articles
```
┌─────────────────────────────────────────────────────────────┐
│ 📰 Flux RSS                                                  │
├─────────────────────────────────────────────────────────────┤
│ [🔍 Rechercher...] [Source ▼] [Catégorie ▼] [Date ▼]       │
│ ⚪ Tous  ⚫ Non lus  ⚪ Lus  ⚪ Archivés                       │
│                                                              │
│ 127 articles non lus • Dernier: Il y a 15 min               │
│ [Tout marquer comme lu] [Actualiser]                        │
├─────────────────────────────────────────────────────────────┤
│                                                              │
│ 📄 What's New in .NET 9 Performance                         │
│     🔗 .NET Blog • ⏱️ 8 min • 🕐 Il y a 2h                  │
│     🏷️ .NET • Performance • C#                             │
│     Lorem ipsum performance improvements in .NET 9...       │
│     [Lire plus] [📌 Sauvegarder] [✓ Marquer lu]           │
│                                                              │
│ ─────────────────────────────────────────────────────────── │
│                                                              │
│ 📄 Microservices Patterns for Azure                         │
│     🔗 Azure Architecture • ⏱️ 12 min • 🕐 Il y a 4h       │
│     🏷️ Azure • Microservices • Architecture               │
│     Best practices for designing microservices on Azure...  │
│     [Lire plus] [📌 Sauvegarder] [✓ Marquer lu]           │
│                                                              │
│ ─────────────────────────────────────────────────────────── │
│                                                              │
│ 📄 Docker Multi-Stage Build Optimization                    │
│     🔗 Docker Blog • ⏱️ 6 min • 🕐 Il y a 6h               │
│     🏷️ Docker • DevOps • CI/CD                            │
│     Learn how to optimize your Docker builds with...        │
│     [Lire plus] [📌 Sauvegarder] [✓ Marquer lu]           │
│                                                              │
│ ... [25 autres articles]                                    │
│                                                              │
│                    [Charger plus]                           │
└─────────────────────────────────────────────────────────────┘
```

### Vue carte avec image
```
┌─────────────────────────────────────────────────────────────┐
│ 📰 Flux RSS - Vue carte                                     │
├─────────────────────────────────────────────────────────────┤
│                                                              │
│ ┌────────────┬──────────────────────────────────────────┐   │
│ │ [Image]    │ What's New in .NET 9 Performance        │   │
│ │  (300x200) │ 🔗 .NET Blog • ⏱️ 8 min • 🕐 2h         │   │
│ │            │ 🏷️ .NET • Performance • C#              │   │
│ │            │                                          │   │
│ │            │ Major performance improvements including │   │
│ │            │ JSON serialization, LINQ optimizations...│   │
│ │            │                                          │   │
│ │            │ [Lire l'article] [✓] [📌] [🗑️]          │   │
│ └────────────┴──────────────────────────────────────────┘   │
│                                                              │
│ ┌────────────┬──────────────────────────────────────────┐   │
│ │ [Image]    │ Microservices Patterns for Azure        │   │
│ │  (300x200) │ 🔗 Azure Architecture • ⏱️ 12 min • 4h  │   │
│ │            │ ...                                      │   │
│ └────────────┴──────────────────────────────────────────┘   │
│                                                              │
└─────────────────────────────────────────────────────────────┘
```

### Panneau de lecture (détail article)
```
┌─────────────────────────────────────────────────────────────┐
│ [← Retour aux articles]                           [✕]       │
├─────────────────────────────────────────────────────────────┤
│                                                              │
│ What's New in .NET 9 Performance                            │
│                                                              │
│ 🔗 .NET Blog • 📅 13 janvier 2025 • ⏱️ 8 min de lecture   │
│ 🏷️ .NET • Performance • C#                                │
│                                                              │
│ [🔗 Ouvrir dans le navigateur] [📌 Sauvegarder]            │
│ [✓ Marquer comme lu] [🗑️ Archiver]                         │
│                                                              │
│ ──────────────────────────────────────────────────────────  │
│                                                              │
│ [Contenu de l'article formaté en Markdown]                 │
│                                                              │
│ .NET 9 introduces significant performance improvements      │
│ across the entire stack. Here are the highlights...         │
│                                                              │
│ ## JSON Serialization                                       │
│                                                              │
│ The new System.Text.Json enhancements provide up to 40%     │
│ faster serialization for complex objects...                 │
│                                                              │
│ ```csharp                                                   │
│ var options = new JsonSerializerOptions                     │
│ {                                                            │
│     TypeInfoResolver = ...                                  │
│ };                                                           │
│ ```                                                          │
│                                                              │
│ ... [reste du contenu] ...                                  │
│                                                              │
│ ──────────────────────────────────────────────────────────  │
│                                                              │
│ 🤖 Analyse IA:                                              │
│ • Niveau: Intermédiaire                                     │
│ • Catégories suggérées: Performance, .NET, C#               │
│ • Résumé: Améliorations de performance dans .NET 9...       │
│                                                              │
└─────────────────────────────────────────────────────────────┘
```

### Filtres avancés
```
┌─────────────────────────────────────────────────────────────┐
│ 🔍 Filtres avancés                                [✕]       │
├─────────────────────────────────────────────────────────────┤
│                                                              │
│ Source                                                       │
│ ☑ .NET Blog (15)                                            │
│ ☑ Azure Architecture (8)                                    │
│ ☐ Docker Blog (12)                                          │
│ ☑ Scott Hanselman (3)                                       │
│ ... [Voir tous les flux]                                    │
│                                                              │
│ Catégorie                                                    │
│ ☑ .NET & C# (23)                                            │
│ ☐ DevOps (15)                                               │
│ ☑ Architecture (12)                                         │
│ ☐ Cloud (18)                                                │
│                                                              │
│ Date de publication                                          │
│ ⚫ Dernières 24h  ⚪ Cette semaine  ⚪ Ce mois  ⚪ Tout      │
│                                                              │
│ Statut                                                       │
│ ⚫ Non lus uniquement  ⚪ Tous  ⚪ Lus  ⚪ Archivés           │
│                                                              │
│ Temps de lecture estimé                                     │
│ [░░░░░░██░░░░░░] 3-10 minutes                               │
│                                                              │
│          [Réinitialiser] [Appliquer les filtres]           │
└─────────────────────────────────────────────────────────────┘
```

## 🧪 Tests et validation

### Tests unitaires
- [ ] **Composants Blazor** : Tests des vues liste et carte
- [ ] **Filtres** : Tests de combinaisons de filtres multiples
- [ ] **Pagination** : Tests de chargement progressif
- [ ] **Marquage lu/non-lu** : Tests de persistance des statuts

### Tests d'intégration
- [ ] **API** : Tests des endpoints de récupération d'articles
- [ ] **Performance** : Load testing avec 10k+ articles
- [ ] **Temps réel** : Tests de synchronisation des statuts
- [ ] **Recherche** : Tests full-text sur grands volumes

### Tests d'acceptance utilisateur
- [ ] **UX** : Navigation fluide entre articles (target: <200ms)
- [ ] **Filtres** : Application instantanée des filtres (<500ms)
- [ ] **Recherche** : Résultats pertinents en <1s
- [ ] **Satisfaction** : >85% trouvent l'interface intuitive

## 🔗 Dépendances

### Prérequis
- ✅ **Articles en DB** : Entités Article et métadonnées
- 🔄 **Flux RSS importés** : Feature 01 - Gestion des flux
- 🔮 **Nouvelles propriétés** : IsRead, IsArchived, ReadAt sur Article
- 🔮 **Index full-text** : Pour recherche rapide

### Intégrations
- **Watch.Manager.Service.Database** : Extension entité Article pour statuts
- **Watch.Manager.ApiService** : Endpoints GET /api/v1/articles avec filtres
- **Watch.Manager.Web** : Nouveaux composants RssFeedReader et ArticleList
- **Search** : Intégration avec recherche sémantique existante

## 📊 Métriques de succès

### KPIs primaires
- **Taux d'utilisation** : >80% des utilisateurs avec flux consultent quotidiennement
- **Articles lus par session** : Moyenne de 10+ articles
- **Temps de session** : 20-30 minutes en moyenne

### Métriques techniques
- **Performance affichage** : <500ms pour charger 100 articles
- **Performance filtres** : <200ms pour appliquer des filtres
- **Performance recherche** : <1s pour recherche full-text

### Métriques UX
- **Taux de rebond** : <10% abandonnent avant de lire un article
- **Utilisation filtres** : >60% des utilisateurs utilisent les filtres
- **Satisfaction** : >90% trouvent la lecture confortable

## 🗓️ Planning

### Sprint 1 (2 semaines) - Vue de base et filtres
- Composant ArticleList avec pagination
- Filtres par source et catégorie
- API endpoints avec filtres
- Tests unitaires

### Sprint 2 (2 semaines) - Marquage et recherche
- Marquage lu/non-lu + archivage
- Recherche full-text
- Actions groupées
- Vue détail article

### Sprint 3 (1 semaine) - Vue carte et optimisations
- Vue carte avec images
- Optimisations performance
- UX polish et animations
- Tests d'acceptance

---

**Status** : 🔮 Planifié  
**Priority** : P0 - Critique  
**Effort** : 8 points (Large)  
**Dependencies** : Feature 01 - Gestion des flux RSS

*Dernière mise à jour : 2025-01-15*
