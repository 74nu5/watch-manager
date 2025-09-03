namespace Watch.Manager.Web.Services.Models;

/// <summary>
/// Types de vues disponibles pour afficher les catégories.
/// </summary>
public enum CategoryViewType
{
    /// <summary>
    /// Vue en grille (affichage par cartes).
    /// </summary>
    Grid,

    /// <summary>
    /// Vue en liste (affichage tabulaire).
    /// </summary>
    List,

    /// <summary>
    /// Vue en arbre hiérarchique.
    /// </summary>
    Tree
}
