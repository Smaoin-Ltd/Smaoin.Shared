namespace Smaoin.Shared.Seo;

public sealed class SeoMetaData
{
    public string Title { get; set; } = string.Empty;
    public string Description { get; set; } = string.Empty;
    public string? CanonicalUrl { get; set; }
    public string? OgImageUrl { get; set; }
    public bool NoIndex { get; set; }
    public string? OgType { get; set; } = "website";
}
