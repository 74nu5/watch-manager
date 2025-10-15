# Epic : Intégration de flux RSS

## 🎯 Vision

Permettre aux utilisateurs de centraliser leur veille technique en intégrant automatiquement des flux RSS de sources fiables, transformant Watch Manager en agrégateur intelligent avec enrichissement IA.

## 📋 Description

L'intégration de flux RSS constitue une évolution majeure de Watch Manager, permettant l'automatisation de la collecte de contenus. Au lieu d'ajouter manuellement les articles un par un, les utilisateurs pourront s'abonner à leurs blogs et sites techniques préférés, avec synchronisation automatique, détection des nouveaux articles, et enrichissement IA de chaque entrée pour classification et extraction de métadonnées.

## 🎯 Objectifs business

- **Croissance du contenu** : Augmenter le volume d'articles analysés de 10x via l'automatisation
- **Engagement utilisateur** : Réduire de 80% l'effort manuel de curation de contenus
- **Qualité de veille** : Garantir une couverture complète des sources techniques majeures
- **Rétention** : Transformer Watch Manager en outil quotidien incontournable de veille

## 👥 Personas cibles

### 👨‍💻 Développeur actif en veille
- **Besoin** : Suivre 20+ blogs techniques sans effort manuel
- **Défi** : Trop de sources à consulter individuellement
- **Valeur** : Agrégation automatique avec IA pour filtrage et priorisation

### 👩‍💻 Tech Lead
- **Besoin** : Maintenir une veille exhaustive pour son équipe
- **Défi** : Identifier les contenus pertinents parmi le bruit
- **Valeur** : Curation automatique et partage de sélections thématiques

### 🏢 Organisation technique
- **Besoin** : Créer une base de connaissances partagée
- **Défi** : Centraliser les sources dispersées de l'équipe
- **Valeur** : Hub de veille collaboratif avec flux partagés

## 📊 Critères d'acceptation epic

- [ ] **Gestion des abonnements** : CRUD complet pour flux RSS avec import OPML
- [ ] **Synchronisation automatique** : Polling périodique configurable des flux
- [ ] **Enrichissement IA** : Analyse automatique de chaque article importé (titre, résumé, tags, catégories)
- [ ] **Interface de lecture** : Vue agrégée avec filtres par source/date/catégorie
- [ ] **Dédoublonnage intelligent** : Détection des articles déjà présents via embeddings vectoriels

## 🏗️ Features incluses

1. **[Gestion des flux RSS](01-gestion-flux-rss/)** - P0
2. **[Lecture et affichage des flux](02-lecture-affichage/)** - P0
3. **[Importation automatique et enrichissement](03-importation-enrichissement/)** - P1

## 🔗 Dépendances

### Prérequis techniques
- ✅ **Service IA** : ExtractDataAi pour analyse des articles
- ✅ **Embeddings vectoriels** : Système existant pour dédoublonnage
- ✅ **Base de données** : SQL Server avec tables articles et catégories
- 🔮 **Service de synchronisation** : Background worker Aspire pour polling périodique
- 🔮 **Parser RSS** : Bibliothèque .NET pour lecture des flux (System.ServiceModel.Syndication ou CodeHollow.FeedReader)

### Intégrations
- **Watch.Manager.Service.Analyse** : Extension pour traitement batch des articles RSS
- **Watch.Manager.ApiService** : Nouveaux endpoints pour CRUD flux RSS
- **Watch.Manager.Service.Database** : Nouvelles entités RssFeed et RssFeedItem
- **Watch.Manager.AppHost** : Service worker pour synchronisation périodique

## 📈 Métriques de succès

### Métriques d'engagement
- **Flux actifs par utilisateur** : Moyenne de 10+ flux RSS
- **Articles importés quotidiennement** : 50+ par utilisateur actif
- **Temps gagné** : Réduction de 80% du temps de collecte manuelle

### Métriques techniques
- **Taux de synchronisation** : 99% de disponibilité du worker
- **Temps de traitement** : <5s par article (fetch + analyse IA)
- **Précision du dédoublonnage** : >95% de détection des doublons

### Métriques qualité
- **Taux d'erreur parsing** : <2% des flux
- **Qualité des métadonnées** : >85% de tags/catégories pertinents
- **Satisfaction utilisateur** : NPS >8/10 pour la feature

## 🗓️ Timeline

- **Q1 2026** : Gestion des flux + Synchronisation basique (MVP)
- **Q2 2026** : Interface de lecture + Enrichissement IA
- **Q3 2026** : Import OPML + Optimisations performances

## 🚀 MVP Definition

Le MVP de l'intégration flux RSS inclut :
1. **CRUD flux RSS** : Ajouter/supprimer des flux avec URL et nom
2. **Synchronisation manuelle** : Bouton "Rafraîchir" pour importer les articles
3. **Import basique** : Parser RSS + sauvegarde dans DB (titre, lien, date)
4. **Affichage simple** : Liste des articles importés avec lien vers source

## 🎯 Success Criteria

L'epic sera considérée comme réussie si :
- **80% des utilisateurs actifs** utilisent au moins 5 flux RSS
- **Volume de contenu** : 10x plus d'articles analysés qu'avant la feature
- **Adoption** : 90% des nouveaux articles proviennent des flux RSS vs ajout manuel
- **Qualité** : Taux de satisfaction >85% sur la pertinence des articles importés

---

**Status** : 🔮 Planifié  
**Priority** : P0 - Critique  
**Effort** : 21 points (Large)  
**Risk** : Medium - Complexité du parsing RSS multi-formats et charge du worker de synchronisation

*Dernière mise à jour : 2025-01-15*
