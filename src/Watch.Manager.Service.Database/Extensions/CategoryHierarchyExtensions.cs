namespace Watch.Manager.Service.Database.Extensions;

using Watch.Manager.Service.Database.Entities;

/// <summary>
///     Extensions for managing category hierarchy.
/// </summary>
internal static class CategoryHierarchyExtensions
{
    /// <summary>
    ///     Calculates the full hierarchical path of a category.
    /// </summary>
    /// <param name="category">The category.</param>
    /// <param name="allCategories">All available categories.</param>
    /// <returns>The hierarchical path (e.g., "Parent/Child/Sub-child").</returns>
    public static string CalculateHierarchyPath(this Category category, IEnumerable<Category> allCategories)
    {
        var pathSegments = new List<string>();
        var currentCategory = category;

        var categories = allCategories.ToList();

        while (currentCategory != null)
        {
            pathSegments.Insert(0, currentCategory.Name);
            currentCategory = currentCategory.ParentId.HasValue
                                      ? categories.FirstOrDefault(c => c.Id == currentCategory.ParentId.Value)
                                      : null;
        }

        return string.Join("/", pathSegments);
    }

    /// <summary>
    ///     Calculates the depth level in the hierarchy.
    /// </summary>
    /// <param name="category">The category.</param>
    /// <param name="allCategories">All available categories.</param>
    /// <returns>The depth level (0 = root).</returns>
    public static int CalculateHierarchyLevel(this Category category, IEnumerable<Category> allCategories)
    {
        var level = 0;
        var currentCategory = category;

        var categories = allCategories.ToList();

        while (currentCategory?.ParentId != null)
        {
            level++;
            currentCategory = categories.FirstOrDefault(c => c.Id == currentCategory.ParentId.Value);
        }

        return level;
    }

    /// <summary>
    ///     Gets all ancestors of a category.
    /// </summary>
    /// <param name="category">The category.</param>
    /// <param name="allCategories">All available categories.</param>
    /// <returns>List of ancestor categories (from closest to farthest).</returns>
    public static IEnumerable<Category> GetAncestors(this Category category, IEnumerable<Category> allCategories)
    {
        var ancestors = new List<Category>();
        var currentCategory = category;
        var categories = allCategories.ToList();

        while (currentCategory.ParentId != null)
        {
            var parent = categories.FirstOrDefault(c => c.Id == currentCategory.ParentId.Value);

            if (parent != null)
            {
                ancestors.Add(parent);
                currentCategory = parent;
                continue;
            }

            break;
        }

        return ancestors;
    }

    /// <summary>
    ///     Gets all descendants of a category recursively.
    /// </summary>
    /// <param name="category">The parent category.</param>
    /// <param name="allCategories">All available categories.</param>
    /// <returns>List of all descendant categories.</returns>
    public static IEnumerable<Category> GetAllDescendants(this Category category, IEnumerable<Category> allCategories)
    {
        var descendants = new List<Category>();
        var categories = allCategories.ToList();
        var directChildren = categories.Where(c => c.ParentId == category.Id);

        foreach (var child in directChildren)
        {
            descendants.Add(child);
            descendants.AddRange(child.GetAllDescendants(categories));
        }

        return descendants;
    }

    /// <summary>
    ///     Checks if a category is an ancestor of another category.
    /// </summary>
    /// <param name="potentialAncestor">The potential ancestor category.</param>
    /// <param name="category">The category to check.</param>
    /// <param name="allCategories">All available categories.</param>
    /// <returns>True if <paramref name="potentialAncestor" /> is an ancestor of <paramref name="category" />.</returns>
    public static bool IsAncestorOf(this Category potentialAncestor, Category category, IEnumerable<Category> allCategories)
        => category.GetAncestors(allCategories).Any(a => a.Id == potentialAncestor.Id);

    /// <summary>
    ///     Gets the effective keywords of a category (including inheritance).
    /// </summary>
    /// <param name="category">The category.</param>
    /// <param name="allCategories">All available categories.</param>
    /// <returns>List of effective keywords.</returns>
    public static string[] GetEffectiveKeywords(this Category category, IEnumerable<Category> allCategories)
    {
        var keywords = new HashSet<string>(category.Keywords);

        if (category is { InheritFromParent: true, ParentId: not null })
        {
            var ancestors = category.GetAncestors(allCategories);

            foreach (var ancestor in ancestors)
            {
                // Roots always inherit
                if (ancestor.InheritFromParent || ancestor.ParentId == null)
                {
                    foreach (var keyword in ancestor.Keywords)
                        _ = keywords.Add(keyword);
                }
            }
        }

        return [.. keywords];
    }

    /// <summary>
    ///     Gets the effective confidence threshold of a category (including inheritance).
    /// </summary>
    /// <param name="category">The category.</param>
    /// <param name="allCategories">All available categories.</param>
    /// <returns>The effective confidence threshold.</returns>
    public static double GetEffectiveConfidenceThreshold(this Category category, IEnumerable<Category> allCategories)
    {
        if (category.ConfidenceThreshold.HasValue)
            return category.ConfidenceThreshold.Value;

        if (category is { InheritFromParent: true, ParentId: not null })
        {
            var categories = allCategories.ToList();
            var parent = categories.FirstOrDefault(c => c.Id == category.ParentId.Value);
            if (parent != null)
                return parent.GetEffectiveConfidenceThreshold(categories);
        }

        return 0.7; // Default value
    }

    /// <summary>
    ///     Updates the hierarchical paths of all descendants of a category.
    /// </summary>
    /// <param name="category">The parent category.</param>
    /// <param name="allCategories">All available categories.</param>
    public static void UpdateDescendantsHierarchyPaths(this Category category, IEnumerable<Category> allCategories)
    {
        var categories = allCategories.ToList();
        var descendants = category.GetAllDescendants(categories);

        foreach (var descendant in descendants)
        {
            descendant.HierarchyPath = descendant.CalculateHierarchyPath(categories);
            descendant.HierarchyLevel = descendant.CalculateHierarchyLevel(categories);
        }
    }

    /// <summary>
    ///     Gets a hierarchical tree representation of the categories.
    /// </summary>
    /// <param name="categories">The categories to organize.</param>
    /// <returns>The root categories with their children organized as a tree.</returns>
    public static IEnumerable<Category> ToHierarchicalTree(this IEnumerable<Category> categories)
    {
        var categoryList = categories.ToList();
        var categoryDict = categoryList.ToDictionary(c => c.Id, c => c);

        // Organize children
        foreach (var category in categoryList)
        {
            if (!category.ParentId.HasValue || !categoryDict.TryGetValue(category.ParentId.Value, out var parent))
                continue;

            if (parent.Children is not List<Category> children)
            {
                parent.Children = [];
                children = (List<Category>)parent.Children;
            }

            children.Add(category);
        }

        // Sort children by DisplayOrder then by name
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

        // Return only root categories, sorted
        return categoryList
              .Where(c => c.ParentId == null)
              .OrderBy(c => c.DisplayOrder)
              .ThenBy(c => c.Name);
    }
}
