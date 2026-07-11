using System.Text;
using System.Xml;

namespace Smaoin.Shared.Seo.Sitemap;

public sealed class SitemapBuilder : ISitemapBuilder
{
    public string Build(IEnumerable<SitemapEntry> entries)
    {
        var sb = new StringBuilder();
        var settings = new XmlWriterSettings { Encoding = Encoding.UTF8, Indent = true };

        using (var writer = XmlWriter.Create(sb, settings))
        {
            writer.WriteStartDocument();
            writer.WriteStartElement("urlset", "http://www.sitemaps.org/schemas/sitemap/0.9");

            foreach (var entry in entries)
            {
                writer.WriteStartElement("url");
                writer.WriteElementString("loc", entry.Url);
                if (entry.LastModified.HasValue)
                    writer.WriteElementString("lastmod", entry.LastModified.Value.ToString("yyyy-MM-dd"));
                writer.WriteElementString("changefreq", entry.ChangeFrequency);
                writer.WriteElementString("priority", entry.Priority.ToString("F1"));
                writer.WriteEndElement();
            }

            writer.WriteEndElement();
        }

        return sb.ToString();
    }
}
