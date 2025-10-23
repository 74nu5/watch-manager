# Feature : Importation automatique et enrichissement

## 🎯 Objectif

Automatiser la collecte périodique des nouveaux articles depuis les flux RSS et enrichir automatiquement chaque article via l'IA pour extraction de métadonnées, classification par catégorie et génération d'embeddings vectoriels.

## 📋 Description

Cette feature est le cœur de l'automatisation de Watch Manager. Un service worker (background service Aspire) s'exécute périodiquement pour synchroniser tous les flux RSS actifs, détecter les nouveaux articles, et appliquer automatiquement le pipeline d'analyse IA existant (ExtractDataAi, embeddings, classification) sur chaque article importé. Le système doit gérer efficacement la charge, éviter les doublons, et maintenir un historique des synchronisations.

## 👤 User Persona

**Thomas, Développeur Full-Stack (4 ans d'expérience)**
- A configuré 20 flux RSS dans Watch Manager
- Veut que les articles arrivent automatiquement sans intervention
- Consulte Watch Manager 2x par jour pour voir les nouveautés
- S'attend à ce que les articles soient déjà classifiés et prêts à lire

## 🎯 User Stories

### [US-001 : Synchronisation automatique périodique](us-001-synchronisation-automatique.md)
**En tant que** utilisateur avec des flux RSS configurés  
**Je veux** que le système importe automatiquement les nouveaux articles  
**Afin de** ne pas avoir à déclencher manuellement la synchronisation

### [US-002 : Enrichissement IA des articles importés](us-002-enrichissement-ia-articles.md)
**En tant que** utilisateur  
**Je veux** que chaque article RSS soit analysé par l'IA à l'import  
**Afin de** bénéficier automatiquement de la classification et des métadonnées

### [US-003 : Détection et gestion des doublons](us-003-detection-gestion-doublons.md)
**En tant que** système  
**Je veux** détecter les articles déjà présents via embeddings vectoriels  
**Afin de** ne pas importer deux fois le même contenu

### [US-004 : Gestion des erreurs et retry](us-004-gestion-erreurs-retry.md)
**En tant que** système  
**Je veux** gérer les erreurs de fetch et réessayer intelligemment  
**Afin de** garantir la fiabilité de la synchronisation

### [US-005 : Monitoring et observabilité](us-005-monitoring-observabilite.md)
**En tant qu'** administrateur  
**Je veux** monitorer l'état des synchronisations via OpenTelemetry  
**Afin de** détecter et résoudre les problèmes rapidement

### [US-006 : Configuration des priorités et fréquences](us-006-configuration-priorites.md)
**En tant que** utilisateur  
**Je veux** configurer la fréquence de synchronisation par flux  
**Afin de** prioriser les sources les plus actives

## 🔧 Critères d'acceptation techniques

### Service de synchronisation
- [ ] **Background worker** : Service hébergé Aspire avec exécution périodique
- [ ] **Polling intelligent** : Respecte les fréquences configurées par flux
- [ ] **Parallélisation** : Synchronise plusieurs flux simultanément (configurable)
- [ ] **Graceful shutdown** : Termine proprement les tâches en cours lors de l'arrêt

### Pipeline d'enrichissement
- [ ] **Sanitization** : Nettoyage HTML avec SanitizeService existant
- [ ] **Extraction IA** : Appel à ExtractDataAi pour titre, résumé, tags
- [ ] **Embeddings** : Génération vectors pour tête et corps de l'article
- [ ] **Classification** : Assignment automatique aux catégories avec scores

### Dédoublonnage
- [ ] **Recherche vectorielle** : Comparaison embeddings pour détecter similarité
- [ ] **Seuil configurable** : Taux de similarité pour considérer comme doublon (ex: >95%)
- [ ] **Priorisation source** : En cas de doublon, privilégier la source de référence
- [ ] **Historique** : Tracking des doublons détectés pour reporting

### Gestion des erreurs
- [ ] **Retry policy** : Exponential backoff pour erreurs réseau (3 tentatives max)
- [ ] **Circuit breaker** : Suspend temporairement les flux en erreur répétée
- [ ] **Dead letter queue** : Articles en échec persistent pour traitement manuel
- [ ] **Alerting** : Notifications si >X% des flux échouent

## 🎨 Maquettes et UX

### Statut de synchronisation dans l'interface
```
┌─────────────────────────────────────────────────────────────┐
│ 📡 Mes flux RSS                              [+ Ajouter]     │
├─────────────────────────────────────────────────────────────┤
│ 🔄 Synchronisation en cours... (12/35 flux)                 │
│ [████████░░░░░░░░░] 34% • Temps restant: ~2 min             │
├─────────────────────────────────────────────────────────────┤
│                                                              │
│ 📂 .NET & C#                                    (5 flux)     │
│   📄 .NET Blog                                  ✅ Actif     │
│       https://devblogs.microsoft.com/dotnet/feed/            │
│       ✅ Dernière sync: Il y a 2h • 15 nouveaux articles     │
│       🔄 Prochaine sync: Dans 2h                             │
│       [Rafraîchir maintenant]                                │
│                                                              │
│   📄 Andrew Lock's Blog                         ⏳ Sync...   │
│       https://andrewlock.net/rss/                            │
│       🔄 Synchronisation en cours...                         │
│                                                              │
└─────────────────────────────────────────────────────────────┘
```

### Dashboard de monitoring (admin)
```
┌─────────────────────────────────────────────────────────────┐
│ 📊 Monitoring RSS Synchronisation                           │
├─────────────────────────────────────────────────────────────┤
│                                                              │
│ État général                                                 │
│ • 🟢 Système: Opérationnel                                  │
│ • 🔄 Dernière exécution: Il y a 5 min                       │
│ • ⏱️ Durée moyenne: 45 secondes                             │
│ • 📊 Taux de succès (24h): 98.5%                            │
│                                                              │
│ ┌──────────────────────────────────────────────────────┐    │
│ │ Articles importés (7 derniers jours)                 │    │
│ │                                                       │    │
│ │ 500 │                         ●                      │    │
│ │ 400 │                     ●       ●                  │    │
│ │ 300 │             ●   ●               ●             │    │
│ │ 200 │         ●                           ●         │    │
│ │ 100 │     ●                                   ●     │    │
│ │   0 └───┬───┬───┬───┬───┬───┬───┬───┬───┬───┬──   │    │
│ │       L   M   M   J   V   S   D                     │    │
│ └──────────────────────────────────────────────────────┘    │
│                                                              │
│ Flux en erreur (nécessitent attention)                      │
│ ❌ Old TechCrunch RSS - 404 Not Found (5 tentatives)        │
│ ⚠️ Broken Blog Feed - Timeout (3 tentatives)                │
│                                                              │
│ Performance récente                                          │
│ • ⚡ Temps moyen par flux: 1.2s                             │
│ • 📦 Articles traités (24h): 1,247                          │
│ • 🚫 Doublons détectés: 34 (2.7%)                           │
│ • 🤖 Taux succès enrichissement IA: 99.1%                   │
│                                                              │
└─────────────────────────────────────────────────────────────┘
```

### Configuration de la synchronisation
```
┌─────────────────────────────────────────────────────────────┐
│ ⚙️ Configuration de la synchronisation          [✕]         │
├─────────────────────────────────────────────────────────────┤
│                                                              │
│ Fréquence globale par défaut                                │
│ [Toutes les 4 heures ▼]                                     │
│                                                              │
│ Synchronisation parallèle                                    │
│ Nombre de flux simultanés: [█████░░░░░] 5                  │
│                                                              │
│ Enrichissement automatique                                   │
│ ☑ Activer l'analyse IA pour nouveaux articles              │
│ ☑ Générer les embeddings vectoriels                        │
│ ☑ Classification automatique par catégories                │
│ ☑ Extraction des tags et métadonnées                       │
│                                                              │
│ Gestion des doublons                                         │
│ Seuil de similarité: [███████████░] 95%                     │
│ ☑ Ignorer les articles similaires existants                │
│ ☐ Notifier lors de la détection de doublons                │
│                                                              │
│ Gestion des erreurs                                          │
│ Nombre de tentatives: [███░░░░░░░] 3                        │
│ Délai entre tentatives: [█████░░░░░] 5 minutes              │
│ ☑ Suspendre automatiquement les flux en échec répété       │
│                                                              │
│                    [Annuler]  [Enregistrer]                 │
└─────────────────────────────────────────────────────────────┘
```

### Notification de nouveaux articles
```
┌─────────────────────────────────────────────────────────────┐
│ 🔔 Nouveaux articles disponibles                            │
├─────────────────────────────────────────────────────────────┤
│                                                              │
│ 27 nouveaux articles importés depuis votre dernière visite  │
│                                                              │
│ • 📰 .NET Blog (5 articles)                                 │
│ • 📰 Azure Architecture (3 articles)                        │
│ • 📰 Scott Hanselman (2 articles)                           │
│ • ... et 17 autres                                          │
│                                                              │
│          [Voir les nouveaux articles]  [Plus tard]          │
└─────────────────────────────────────────────────────────────┘
```

## 🧪 Tests et validation

### Tests unitaires
- [ ] **Worker service** : Tests du cycle de vie et exécution périodique
- [ ] **Pipeline enrichissement** : Tests end-to-end du traitement d'un article
- [ ] **Dédoublonnage** : Tests de détection de similarité vectorielle
- [ ] **Retry logic** : Tests des politiques de retry et circuit breaker

### Tests d'intégration
- [ ] **Synchronisation complète** : Test avec 50 flux réels
- [ ] **Performance** : Import de 1000 articles en <5 minutes
- [ ] **Concurrence** : Tests avec 10 flux synchronisés simultanément
- [ ] **Résilience** : Tests avec flux en erreur et réseau instable

### Tests d'acceptance
- [ ] **Automatisation** : Nouveaux articles apparaissent sans intervention
- [ ] **Qualité IA** : >90% des articles correctement classifiés
- [ ] **Fiabilité** : >99% de disponibilité du worker sur 7 jours
- [ ] **Performance** : Latence <30s entre publication RSS et disponibilité

## 🔗 Dépendances

### Prérequis
- ✅ **Service IA** : ExtractDataAi et génération d'embeddings
- ✅ **Vector search** : Recherche de similarité dans SQL Server
- 🔄 **Flux RSS configurés** : Feature 01 - Gestion des flux
- 🔮 **Background worker Aspire** : Service hébergé pour polling
- 🔮 **Queue** : Dead letter queue pour articles en échec (optionnel)

### Intégrations
- **Watch.Manager.AppHost** : Configuration du background worker
- **Watch.Manager.Service.RssSync** : Nouveau service de synchronisation
- **Watch.Manager.Service.Analyse** : Utilisation des services existants
- **OpenTelemetry** : Métriques et traces de synchronisation

## 📊 Métriques de succès

### KPIs primaires
- **Volume d'articles** : 50+ articles importés par utilisateur par jour
- **Fraîcheur** : 95% des articles disponibles <1h après publication
- **Adoption** : 80% des utilisateurs reçoivent des articles RSS quotidiennement

### Métriques techniques
- **Taux de succès** : >99% des synchronisations réussies
- **Performance** : <2s par article (fetch + enrichissement)
- **Dédoublonnage** : <5% de doublons non détectés
- **Disponibilité worker** : >99.5% uptime

### Métriques qualité
- **Précision IA** : >90% des tags/catégories pertinents
- **Taux d'erreur** : <1% d'articles en dead letter queue
- **Latence utilisateur** : Nouveaux articles visibles en <5 minutes

## 🗓️ Planning

### Sprint 1 (2 semaines) - Worker de base
- Background worker Aspire avec polling
- Fetch et parsing des nouveaux articles RSS
- Sauvegarde basique en DB sans enrichissement
- Tests unitaires et d'intégration

### Sprint 2 (2 semaines) - Enrichissement IA
- Intégration pipeline ExtractDataAi
- Génération embeddings et classification
- Dédoublonnage par similarité vectorielle
- Tests de performance

### Sprint 3 (1 semaine) - Résilience et monitoring
- Retry policies et circuit breaker
- Métriques OpenTelemetry et dashboard
- Dead letter queue et gestion des erreurs
- Tests de charge et résilience

---

**Status** : 🔮 Planifié  
**Priority** : P1 - Haute  
**Effort** : 13 points (Large)  
**Dependencies** : Feature 01 (Gestion flux) + Service IA existant

*Dernière mise à jour : 2025-01-15*
