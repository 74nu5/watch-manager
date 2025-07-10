namespace Watch.Manager.Service.Analyse.Models;

public sealed record ExtractedSite(string Head, string Body, Uri Thumbnail)
{
    public string ThumbnailBase64 { get; set; }
}
