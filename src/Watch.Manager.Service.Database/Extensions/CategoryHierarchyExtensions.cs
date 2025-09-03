namespace Watch.Manager.Service.Database.Extensions;

using Watch.Manager.Service.Database.Entities;

/// <summary>
/// Extensions pour la gestion de la hiérarchie des catégories.
/// </summary>
public static class CategoryHierarchyExtensions
{
    /// <summary>
    /// Calcule le chemin hiérarchique complet d'une catégorie.
    /// </summary>
    /// <param name="category">La catégorie.</param>
    /// <param name="allCategories">Toutes les catégories disponibles.</param>
    /// <returns>Le chemin hiérarchique (ex: "Parent/Enfant/Sous-enfant").</returns>
    public static string CalculateHierarchyPath(this Category category, IEnumerable<Category> allCategories)
    {
        var pathSegments = new List<string>();
        var currentCategory = category;

        while (currentCategory != null)
        {
            pathSegments.Insert(0, currentCategory.Name);
            currentCategory = currentCategory.ParentId.HasValue
                ? allCategories.FirstOrDefault(c => c.Id == currentCategory.ParentId.Value)
                : null;
        }

        return string.Join("/", pathSegments);
    }

    /// <summary>
    /// Calcule le niveau de profondeur dans la hiérarchie.
    /// </summary>
    /// <param name="category">La catégorie.</param>
    /// <param name="allCategories">Toutes les catégories disponibles.</param>
    /// <returns>Le niveau de profondeur (0 = racine).</returns>
    public static int CalculateHierarchyLevel(this Category category, IEnumerable<Category> allCategories)
    {
        var level = 0;
        var currentCategory = category;

        while (currentCategory?.ParentId != null)
        {
            level++;
            currentCategory = allCategories.FirstOrDefault(c => c.Id == currentCategory.ParentId.Value);
        }

        return level;
    }

    /// <summary>
    /// Obtient tous les ancêtres d'une catégorie.
    /// </summary>
    /// <param name="category">La catégorie.</param>
    /// <param name="allCategories">Toutes les catégories disponibles.</param>
    /// <returns>Liste des catégories ancêtres (du plus proche au plus éloigné).</returns>
    public static IEnumerable<Category> GetAncestors(this Category category, IEnumerable<Category> allCategories)
    {
        var ancestors = new List<Category>();
        var currentCategory = category;

        while (currentCategory?.ParentId != null)
        {
            var parent = allCategories.FirstOrDefault(c => c.Id == currentCategory.ParentId.Value);
            if (parent != null)
            {
                ancestors.Add(parent);
                currentCategory = parent;
            }
            else
            {
                break;
            }
        }

        return ancestors;
    }

    /// <summary>
    /// Obtient tous les descendants d'une catégorie de manière récursive.
    /// </summary>
    /// <param name="category">La catégorie parent.</param>
    /// <param name="allCategories">Toutes les catégories disponibles.</param>
    /// <returns>Liste de tous les descendants.</returns>
    public static IEnumerable<Category> GetAllDescendants(this Category category, IEnumerable<Category> allCategories)
    {
        var descendants = new List<Category>();
        var directChildren = allCategories.Where(c => c.ParentId == category.Id);

        foreach (var child in directChildren)
        {
            descendants.Add(child);
            descendants.AddRange(child.GetAllDescendants(allCategories));
        }

        return descendants;
    }

    /// <summary>
    /// Vérifie si une catégorie est un ancêtre d'une autre catégorie.
    /// </summary>
    /// <param name="potentialAncestor">La catégorie potentiellement ancêtre.</param>
    /// <param name="category">La catégorie à vérifier.</param>
    /// <param name="allCategories">Toutes les catégories disponibles.</param>
    /// <returns>True si potentialAncestor est un ancêtre de category.</returns>
    public static bool IsAncestorOf(this Category potentialAncestor, Category category, IEnumerable<Category> allCategories)
    {
        return category.GetAncestors(allCategories).Any(a => a.Id == potentialAncestor.Id);
    }

    /// <summary>
    /// Obtient les mots-clés effectifs d'une catégorie (incluant l'héritage).
    /// </summary>
    /// <param name="category">La catégorie.</param>
    /// <param name="allCategories">Toutes les catégories disponibles.</param>
    /// <returns>Liste des mots-clés effectifs.</returns>
    public static string[] GetEffectiveKeywords(this Category category, IEnumerable<Category> allCategories)
    {
        var keywords = new HashSet<string>(category.Keywords);

        if (category.InheritFromParent && category.ParentId.HasValue)
        {
            var ancestors = category.GetAncestors(allCategories);
            foreach (var ancestor in ancestors)
            {
                if (ancestor.InheritFromParent || ancestor.ParentId == null) // Les racines héritent toujours
                {
                    foreach (var keyword in ancestor.Keywords)
                    {
                        _ = keywords.Add(keyword);
                    }
                }
            }
        }

        return keywords.ToArray();
    }

    /// <summary>
    /// Obtient le seuil de confiance effectif d'une catégorie (incluant l'héritage).
    /// </summary>
    /// <param name="category">La catégorie.</param>
    /// <param name="allCategories">Toutes les catégories disponibles.</param>
    /// <returns>Le seuil de confiance effectif.</returns>
    public static double GetEffectiveConfidenceThreshold(this Category category, IEnumerable<Category> allCategories)
    {
        if (category.ConfidenceThreshold.HasValue)
        {
            return category.ConfidenceThreshold.Value;
        }

        if (category.InheritFromParent && category.ParentId.HasValue)
        {
            var parent = allCategories.FirstOrDefault(c => c.Id == category.ParentId.Value);
            if (parent != null)
            {
                return parent.GetEffectiveConfidenceThreshold(allCategories);
            }
        }

        return 0.7; // Valeur par défaut
    }

    /// <summary>
    /// Met à jour les chemins hiérarchiques de tous les descendants d'une catégorie.
    /// </summary>
    /// <param name="category">La catégorie parent.</param>
    /// <param name="allCategories">Toutes les catégories disponibles.</param>
    public static void UpdateDescendantsHierarchyPaths(this Category category, IEnumerable<Category> allCategories)
    {
        var descendants = category.GetAllDescendants(allCategories);
        foreach (var descendant in descendants)
        {
            descendant.HierarchyPath = descendant.CalculateHierarchyPath(allCategories);
            descendant.HierarchyLevel = descendant.CalculateHierarchyLevel(allCategories);
        }
    }

    /// <summary>
    /// Obtient une représentation en arbre hiérarchique des catégories.
    /// </summary>
    /// <param name="categories">Les catégories à organiser.</param>
    /// <returns>Les catégories racines avec leurs enfants organisés en arbre.</returns>
    public static IEnumerable<Category> ToHierarchicalTree(this IEnumerable<Category> categories)
    {
        var categoryList = categories.ToList();
        var categoryDict = categoryList.ToDictionary(c => c.Id, c => c);

        // Organiser les enfants
        foreach (var category in categoryList)
        {
            if (category.ParentId.HasValue && categoryDict.TryGetValue(category.ParentId.Value, out var parent))
            {
                if (parent.Children is not List<Category> children)
                {
                    parent.Children = new List<Category>();
                    children = (List<Category>)parent.Children;
                }
                children.Add(category);
            }
        }

        // Trier les enfants par DisplayOrder puis par nom
        foreach (var category in categoryList)
        {
            if (category.Children is List<Category> children)
            {
                children.Sort((a, b) =>
                {
                    var orderCompare = a.DisplayOrder.CompareTo(b.DisplayOrder);
                    return orderCompare != 0 ? orderCompare : string.Compare(a.Name, b.Name, StringComparison.OrdinalIgnoreCase);
                });
            }
        }

        // Retourner seulement les catégories racines, triées
        return categoryList
            .Where(c => c.ParentId == null)
            .OrderBy(c => c.DisplayOrder)
            .ThenBy(c => c.Name);
    }
}
