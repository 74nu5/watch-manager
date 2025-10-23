# Feature : Gestion des flux RSS

## 🎯 Objectif

Développer une interface complète de gestion des abonnements aux flux RSS permettant aux utilisateurs d'ajouter, modifier, supprimer et organiser leurs sources de veille technique avec validation et test de connectivité.

## 📋 Description

Cette feature constitue la fondation de l'intégration RSS dans Watch Manager. Elle permet aux utilisateurs de gérer leurs abonnements aux flux RSS de manière intuitive, avec validation automatique des URLs, détection du format du flux, et organisation par catégories. Le système doit supporter les formats RSS 2.0, Atom et RSS 1.0, et offrir des fonctionnalités avancées comme l'import/export OPML pour faciliter la migration depuis d'autres agrégateurs.

## 👤 User Persona

**Marc, Développeur Backend (5 ans d'expérience)**
- Suit 25+ blogs techniques sur .NET, Azure, Architecture
- Utilisait Feedly mais cherche une solution avec IA
- Veut importer ses abonnements existants via OPML

## 🎯 User Stories

### [US-001 : Ajout d'un flux RSS](us-001-ajout-flux-rss.md)
**En tant que** utilisateur de Watch Manager  
**Je veux** ajouter un flux RSS en saisissant son URL  
**Afin de** suivre automatiquement les nouveaux articles de cette source

### [US-002 : Validation et test de flux](us-002-validation-test-flux.md)
**En tant que** utilisateur ajoutant un flux  
**Je veux** que le système valide et teste la connectivité du flux  
**Afin de** m'assurer qu'il est accessible et au bon format

### [US-003 : Gestion des flux (liste, modification, suppression)](us-003-gestion-flux.md)
**En tant que** utilisateur avec plusieurs flux  
**Je veux** voir la liste de mes abonnements et pouvoir les modifier ou supprimer  
**Afin de** maintenir mes sources de veille à jour

### [US-004 : Import/Export OPML](us-004-import-export-opml.md)
**En tant que** utilisateur migrant depuis un autre agrégateur  
**Je veux** importer mes abonnements via un fichier OPML  
**Afin de** ne pas avoir à ressaisir tous mes flux manuellement

### [US-005 : Organisation par catégories](us-005-organisation-categories.md)
**En tant que** utilisateur avec de nombreux flux  
**Je veux** organiser mes flux par catégories (ex: .NET, DevOps, Architecture)  
**Afin de** structurer ma veille par thématique

### [US-006 : Configuration de la synchronisation](us-006-configuration-synchronisation.md)
**En tant que** utilisateur soucieux de performances  
**Je veux** configurer la fréquence de synchronisation par flux  
**Afin de** optimiser la charge et prioriser les sources actives

## 🔧 Critères d'acceptation techniques

### CRUD des flux RSS
- [ ] **Création** : Validation URL + parsing automatique des métadonnées du flux
- [ ] **Lecture** : Liste paginée avec recherche et filtres par catégorie
- [ ] **Mise à jour** : Modification nom, catégorie, fréquence de sync
- [ ] **Suppression** : Soft delete avec option de suppression des articles associés

### Validation et parsing
- [ ] **Support multi-formats** : RSS 2.0, Atom 1.0, RSS 1.0 (RDF)
- [ ] **Test de connectivité** : Timeout de 10s avec retry
- [ ] **Extraction métadonnées** : Titre du flux, description, logo/favicon
- [ ] **Détection automatique** : Auto-découverte du flux depuis une URL de page web

### Import/Export OPML
- [ ] **Import** : Parser OPML 2.0 avec préservation des catégories
- [ ] **Validation** : Vérification de chaque flux avant import
- [ ] **Export** : Génération OPML avec métadonnées complètes
- [ ] **Gestion des erreurs** : Rapport détaillé des flux en échec

## 🎨 Maquettes et UX

### Interface de gestion des flux
```
┌─────────────────────────────────────────────────────────────┐
│ 📡 Mes flux RSS                              [+ Ajouter]     │
├─────────────────────────────────────────────────────────────┤
│ [Rechercher...] [🗂️ Toutes catégories ▼] [⚙️ Paramètres]  │
├─────────────────────────────────────────────────────────────┤
│                                                              │
│ 📂 .NET & C#                                    (5 flux)     │
│   📄 .NET Blog                                  ✅ Actif     │
│       https://devblogs.microsoft.com/dotnet/feed/            │
│       🔄 Dernière sync: Il y a 2h • 15 nouveaux articles     │
│       [Rafraîchir] [✏️ Modifier] [🗑️]                       │
│                                                              │
│   📄 Andrew Lock's Blog                         ✅ Actif     │
│       https://andrewlock.net/rss/                            │
│       🔄 Dernière sync: Il y a 5h • 3 nouveaux articles      │
│       [Rafraîchir] [✏️ Modifier] [🗑️]                       │
│                                                              │
│ 📂 DevOps & Cloud                               (3 flux)     │
│   📄 Azure Updates                              ⚠️ Erreur    │
│       https://azure.microsoft.com/updates/feed/              │
│       ❌ Erreur 404: Flux introuvable                        │
│       [Retester] [✏️ Modifier] [🗑️]                         │
│                                                              │
└─────────────────────────────────────────────────────────────┘
```

### Dialogue d'ajout de flux
```
┌─────────────────────────────────────────────────────────────┐
│ ➕ Ajouter un flux RSS                            [✕]       │
├─────────────────────────────────────────────────────────────┤
│                                                              │
│ URL du flux RSS *                                            │
│ [https://devblogs.microsoft.com/dotnet/feed/            ]    │
│ 🔍 [Tester le flux]                                         │
│                                                              │
│ ✅ Flux valide détecté!                                     │
│ 📰 Titre: .NET Blog                                         │
│ 📝 Description: The official .NET team blog...              │
│ 📊 25 articles trouvés                                      │
│                                                              │
│ Nom du flux (optionnel)                                     │
│ [.NET Blog Official                                     ]    │
│                                                              │
│ Catégorie                                                    │
│ [.NET & C# ▼] ou [➕ Créer une catégorie]                  │
│                                                              │
│ Fréquence de synchronisation                                │
│ ⚪ Toutes les heures  ⚫ Toutes les 4h  ⚪ Quotidien         │
│                                                              │
│               [Annuler]  [Ajouter le flux]                  │
└─────────────────────────────────────────────────────────────┘
```

### Import OPML
```
┌─────────────────────────────────────────────────────────────┐
│ 📥 Importer des flux (OPML)                       [✕]       │
├─────────────────────────────────────────────────────────────┤
│                                                              │
│ Sélectionnez votre fichier OPML                             │
│ [📁 Parcourir...] feedly-export.opml                        │
│                                                              │
│ Options d'import                                             │
│ ☑ Préserver les catégories existantes                      │
│ ☑ Valider chaque flux avant import                         │
│ ☐ Fusionner avec les flux existants                        │
│                                                              │
│ 📊 Aperçu du fichier:                                       │
│ • 47 flux détectés                                          │
│ • 8 catégories                                              │
│ • 12 flux déjà présents (seront ignorés)                   │
│                                                              │
│               [Annuler]  [Lancer l'import]                  │
└─────────────────────────────────────────────────────────────┘

┌─────────────────────────────────────────────────────────────┐
│ Import en cours... (25/47)                                  │
│ [████████████░░░░░░░░░░░░░] 53%                            │
│                                                              │
│ ✅ .NET Blog - Importé avec succès                          │
│ ✅ Scott Hanselman's Blog - Importé avec succès             │
│ ❌ Old TechCrunch RSS - Erreur 404                          │
│ ✅ Martin Fowler - Importé avec succès                      │
│ ...                                                          │
└─────────────────────────────────────────────────────────────┘
```

## 🧪 Tests et validation

### Tests unitaires
- [ ] **Parser RSS** : Tests sur RSS 2.0, Atom, RSS 1.0 avec fixtures réelles
- [ ] **Validation URL** : Tests des formats valides/invalides
- [ ] **Parser OPML** : Tests avec fichiers OPML de Feedly, Inoreader, NetNewsWire
- [ ] **Métadonnées** : Extraction correcte du titre, description, favicon

### Tests d'intégration
- [ ] **API endpoints** : Tests CRUD complets des flux
- [ ] **Connectivité réseau** : Tests avec mocks de réponses HTTP
- [ ] **Base de données** : Tests de persistance et relations
- [ ] **Worker sync** : Tests de déclenchement manuel et automatique

### Tests d'acceptance utilisateur
- [ ] **Ajout flux** : 90% de succès sur les flux RSS publics populaires
- [ ] **Import OPML** : Validation avec exports de 3+ agrégateurs différents
- [ ] **Performance** : <2s pour ajouter un flux, <30s pour import de 50 flux
- [ ] **UX** : Tests utilisateurs sur 5+ personnes avec mesure de satisfaction

## 🔗 Dépendances

### Prérequis
- ✅ **Base de données** : Tables articles et catégories existantes
- 🔮 **Nouvelles entités** : RssFeed, RssFeedItem, RssFeedCategory
- 🔮 **Bibliothèque RSS** : CodeHollow.FeedReader ou System.ServiceModel.Syndication
- 🔮 **HTTP Client** : Configuration avec retry policies et timeout

### Intégrations
- **Watch.Manager.Service.Database** : Nouvelles entités et DbContext
- **Watch.Manager.ApiService** : Endpoints /api/v1/rss-feeds
- **Watch.Manager.Web** : Composants Blazor de gestion des flux
- **Watch.Manager.Service.Rss** : Nouveau service pour parsing et validation

## 📊 Métriques de succès

### KPIs primaires
- **Nombre de flux par utilisateur** : Moyenne de 10+ flux actifs
- **Taux d'adoption** : 70% des utilisateurs utilisent au moins 1 flux
- **Taux de succès d'ajout** : >95% des URLs valides importées correctement

### Métriques techniques
- **Temps d'ajout** : <2s pour validation + import
- **Taux d'erreur parsing** : <2% sur les flux standards
- **Performance import OPML** : <1s par flux en moyenne

### Métriques qualité
- **Satisfaction utilisateur** : >85% trouvent l'interface intuitive
- **Taux de complétion import OPML** : >90% des flux importés avec succès
- **Taux de rétention** : 80% des flux ajoutés restent actifs après 1 mois

## 🗓️ Planning

### Sprint 1 (2 semaines) - CRUD et parsing basique
- Entités RssFeed et migrations DB
- Service de parsing RSS (RSS 2.0 + Atom)
- API endpoints CRUD
- Tests unitaires du parser

### Sprint 2 (2 semaines) - Interface et validation
- Composants Blazor de gestion des flux
- Validation et test de connectivité
- Gestion des catégories
- UX/UI polish

### Sprint 3 (1 semaine) - Import/Export OPML
- Parser OPML
- Interface d'import avec preview
- Export OPML
- Tests d'intégration complets

---

**Status** : 🔮 Planifié  
**Priority** : P0 - Critique  
**Effort** : 8 points (Large)  
**Dependencies** : Aucune - Feature de base

*Dernière mise à jour : 2025-01-15*
