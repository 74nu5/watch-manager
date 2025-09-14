namespace Watch.Manager.Web.Services.Models;

/// <summary>
///     Available view types for displaying categories.
/// </summary>
public enum CategoryViewType
{
    /// <summary>
    ///     Grid view (display as cards).
    /// </summary>
    Grid,

    /// <summary>
    ///     List view (tabular display).
    /// </summary>
    List,

    /// <summary>
    ///     Tree view (hierarchical display).
    /// </summary>
    Tree,
}
