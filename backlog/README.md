# Backlog Watch Manager

## Structure du backlog

Cette arborescence organise les fonctionnalités de la roadmap Watch Manager en Epic, Features et User Stories.

### Organisation

```
backlog/
├── README.md                           # Ce fichier
├── 01-apprentissage-dirige/           # Epic : Apprentissage dirigé  
│   ├── epic.md                        # Description de l'epic
│   ├── 01-selection-intelligente/     # Feature : Sélection intelligente de contenus
│   ├── 02-filtrage-competences/       # Feature : Filtrage par domaine de compétence
│   ├── 03-exercices-ia/               # Feature : Génération d'exercices pratiques par IA
│   ├── 04-suivi-progression/          # Feature : Suivi de progression avancé
│   ├── 05-randomisation/              # Feature : Mécanismes de randomisation intelligente
│   └── 06-personnalisation/           # Feature : Personnalisation cognitive
└── templates/                         # Templates pour épics, features et user stories
    ├── epic-template.md
    ├── feature-template.md
    └── user-story-template.md
```

### Conventions de nommage

- **Epic** : `[numéro]-[nom-kebab-case]/`
- **Feature** : `[numéro]-[nom-kebab-case]/`
- **User Story** : `us-[numéro]-[nom-kebab-case].md`

### Statuts

- 🔮 **Planifié** : Fonctionnalité identifiée et spécifiée
- 🔄 **En cours** : Développement actif
- ✅ **Terminé** : Implémentée et testée
- ⏸️ **En pause** : Développement suspendu
- ❌ **Annulé** : Fonctionnalité abandonnée

### Priorisation

- **P0** : Critique - Bloquant pour la release
- **P1** : Haute - Important pour la valeur utilisateur
- **P2** : Moyenne - Amélioration notable
- **P3** : Basse - Nice to have

---

*Dernière mise à jour : ${new Date().toLocaleDateString('fr-FR')}*
