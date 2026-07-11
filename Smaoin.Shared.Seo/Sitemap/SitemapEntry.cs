namespace Smaoin.Shared.Seo.Sitemap;

public sealed class SitemapEntry
{
    public string Url { get; set; } = string.Empty;
    public DateTime? LastModified { get; set; }
    public string ChangeFrequency { get; set; } = "weekly";
    public double Priority { get; set; } = 0.5;
}
