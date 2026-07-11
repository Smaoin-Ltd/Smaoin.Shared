namespace Smaoin.Shared.Seo.Sitemap;

public interface ISitemapBuilder
{
    string Build(IEnumerable<SitemapEntry> entries);
}
