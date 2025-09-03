namespace Watch.Manager.Service.Analyse.Abstractions;

using Watch.Manager.Service.Analyse.Models;

/// <summary>
///     Defines a service for retrieving and processing the source content of a website.
/// </summary>
/// <remarks>
///     This interface provides a method to extract the source content of a website from a specified URL.
///     Implementations of this interface may include additional processing, such as parsing or validation, depending on the
///     use case.
/// </remarks>
public interface IWebSiteService
{
    /// <summary>
    ///     Retrieves and processes the source content of the specified website.
    /// </summary>
    /// <remarks>
    ///     This method performs an asynchronous operation to fetch and process the content of the
    ///     specified website.  The caller is responsible for ensuring that the provided URL is valid and
    ///     accessible.
    /// </remarks>
    /// <param name="url">The URL of the website to retrieve. Must be a valid, absolute URI.</param>
    /// <param name="cancellationToken">A token to monitor for cancellation requests. If canceled, the operation will terminate early.</param>
    /// <returns>
    ///     A task that represents the asynchronous operation. The task result contains an <see cref="ExtractedSite" />
    ///     object  representing the processed content of the website.
    /// </returns>
    Task<ExtractedSite> GetWebSiteSource(Uri url, CancellationToken cancellationToken);
}
