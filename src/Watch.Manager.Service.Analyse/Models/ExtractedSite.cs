namespace Watch.Manager.Service.Analyse.Models;

/// <summary>
///     Represents a site extracted from a source, including its head, body, and thumbnail.
/// </summary>
/// <remarks>
///     This record encapsulates the key components of an extracted site, such as its head content, body
///     content,  and a thumbnail URI. It is immutable except for the <see cref="ThumbnailBase64" /> property, which can be
///     modified.
/// </remarks>
/// <param name="Head">The head content of the extracted site, typically containing metadata and links to stylesheets or scripts.</param>
/// <param name="Body">The body content of the extracted site, representing the main content and structure of the webpage.</param>
/// <param name="Thumbnail">The URI of the thumbnail image associated with the extracted site, which can be used for preview purposes.</param>
public sealed record ExtractedSite(string Head, string Body, Uri Thumbnail)
{
    /// <summary>
    ///     Gets or sets the Base64-encoded representation of the thumbnail image.
    /// </summary>
    public string ThumbnailBase64 { get; set; } = string.Empty;
}
