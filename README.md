# Watch Manager

**L'outil d'accompagnement intelligent pour la veille technique**

Une application .NET moderne qui révolutionne la façon dont vous gérez votre veille technologique grâce à l'intelligence artificielle. Watch Manager vous aide à rester à jour, résoudre vos problèmes techniques et planifier votre apprentissage de manière intelligente.

## 🎯 Vision

Watch Manager aspire à devenir votre **compagnon de veille technique**, transformant la collecte d'informations en un processus d'apprentissage structuré et intelligent, adapté à vos besoins et objectifs professionnels.

## 🌟 Fonctionnalités actuelles

### ✅ Gestion intelligente d'articles
- **Analyse automatique** : Extraction et résumé du contenu web par IA
- **Tagging automatique** : Attribution intelligente de mots-clés
- **Embeddings vectoriels** : Recherche sémantique avancée (1536 dimensions)
- **Catégorisation** : Organisation des articles par thématiques

### 🔄 En développement
- **Interface de gestion des catégories** : Organisation hiérarchique et filtres avancés
- **Classification automatique** : Amélioration de l'IA de catégorisation

## 🚀 Fonctionnalités à venir

### 🔍 Accompagnement à la résolution de problèmes
Vous avez un problème technique ? Watch Manager analysera votre base d'articles pour vous proposer des solutions déjà documentées dans votre veille.

### 🎲 Apprentissage dirigé
Envie de découvrir de nouveaux sujets ? L'IA sélectionnera un article aléatoire et vous proposera des exercices pratiques pour approfondir le sujet.

### 📅 Planification intelligente de veille
Élaborez un plan de veille personnalisé avec des sujets à approfondir, configurable par fréquence (quotidienne, hebdomadaire, mensuelle).

> 📋 **Voir la [Roadmap complète](Roadmap.md) pour plus de détails**

## 🏗️ Architecture

Le projet suit une architecture microservices moderne avec les composants suivants :

### Services principaux

- **Watch.Manager.ApiService** : API RESTful pour la gestion des articles et catégories
- **Watch.Manager.Web** : Interface utilisateur Blazor Server
- **Watch.Manager.Service.Analyse** : Service d'analyse IA pour l'extraction de données
- **Watch.Manager.Service.Database** : Couche d'accès aux données avec Entity Framework
- **Watch.Manager.AppHost** : Orchestrateur Aspire pour le déploiement

### Services utilitaires

- **Watch.Manager.ServiceDefaults** : Configuration par défaut partagée
- **Watch.Manager.Common** : Modèles et utilitaires communs
- **Watch.Manager.Web.Services** : Services spécifiques à l'interface web
- **Watch.Manager.Service.Migrations** : Gestion des migrations de base de données

## 🚀 Technologies utilisées

- **.NET 9** : Framework principal
- **ASP.NET Core** : API et services web
- **Blazor Server** : Interface utilisateur interactive
- **Entity Framework Core** : ORM pour l'accès aux données
- **SQL Server** : Base de données principale
- **Aspire** : Orchestration et observabilité
- **FluentUI** : Composants d'interface utilisateur
- **Microsoft Extensions AI** : Intégration IA
- **Scalar** : Documentation API automatique

## 📋 Prérequis

- .NET 9 SDK
- Docker Desktop (pour SQL Server)
- Visual Studio 2025 ou VS Code avec extension C#

## ⚡ Installation et démarrage rapide

1. **Cloner le repository**
   ```bash
   git clone https://github.com/74nu5/watch-manager.git
   cd watch-manager
   ```

2. **Restaurer les packages NuGet**
   ```bash
   dotnet restore
   ```

3. **Démarrer l'application avec Aspire**
   ```bash
   dotnet run --project src/Watch.Manager.AppHost
   ```

   L'application sera accessible via le tableau de bord Aspire qui ouvrira automatiquement dans votre navigateur.

## 🔧 Configuration

### Base de données

Le projet utilise SQL Server avec la configuration suivante :
- **Serveur** : localhost:1434
- **Base de données** : articlesdb
- **Mot de passe** : Password1234 (configurable via les paramètres Aspire)

### Variables d'environnement

Les principales configurations peuvent être modifiées via les fichiers `appsettings.json` :

```json
{
  "ConnectionStrings": {
    "DefaultConnection": "Server=localhost,1434;Database=articlesdb;..."
  }
}
```

## 📡 API Endpoints

### Articles
- `POST /v1/articles` : Analyser et sauvegarder un nouvel article
- `GET /v1/articles` : Récupérer la liste des articles
- `GET /v1/articles/{id}/thumbnail` : Récupérer la miniature d'un article
- `GET /v1/articles/tags` : Récupérer tous les tags disponibles

### Catégories
- `GET /v1/categories` : Lister toutes les catégories
- `GET /v1/categories/{id}` : Récupérer une catégorie spécifique
- `POST /v1/categories` : Créer une nouvelle catégorie
- `PUT /v1/categories/{id}` : Mettre à jour une catégorie
- `DELETE /v1/categories/{id}` : Supprimer une catégorie

## 🧪 Tests

Exécuter les tests unitaires :

```bash
dotnet test
```

## 📦 Déploiement

### Local avec Docker

L'application utilise Aspire pour orchestrer tous les services et dépendances automatiquement.

### Production

Le projet inclut des configurations pour le déploiement sur Azure :
- Configuration CORS pour Azure App Service
- Profils de publication inclus

## 🤝 Contribution

1. Fork le projet
2. Créer une branche pour votre fonctionnalité (`git checkout -b feature/nouvelle-fonctionnalite`)
3. Commiter vos changements (`git commit -am 'Ajout d'une nouvelle fonctionnalité'`)
4. Pusher vers la branche (`git push origin feature/nouvelle-fonctionnalite`)
5. Créer une Pull Request

## 📄 License

Ce projet est sous licence [MIT](LICENSE).

## 👥 Auteurs

- **74nu5** - Développeur principal

## 🆘 Support

Pour obtenir de l'aide ou signaler un problème :
- Créer une [issue](https://github.com/74nu5/watch-manager/issues)
- Consulter la [documentation](https://github.com/74nu5/watch-manager/wiki)

## 🔮 Roadmap

### 🎯 Objectifs principaux
- **Résolution assistée** : Recherche IA dans vos articles pour résoudre des problèmes techniques
- **Apprentissage dirigé** : Sélection aléatoire de sujets avec exercices générés par IA  
- **Planification de veille** : Plans personnalisés quotidiens, hebdomadaires ou mensuels
- **Collaboration** : Partage et recommandations entre équipes
- **Application mobile** : Veille en mobilité avec synchronisation

📋 **[Voir la roadmap détaillée](Roadmap.md)**

---

*Développé avec ❤️ en utilisant .NET et Aspire*
