namespace Smaoin.Shared.Seo.JsonLd;

/// <summary>
/// Builds ready-to-embed schema.org JSON-LD strings for common, brand-agnostic structures.
/// Each method returns a serialized <c>&lt;script type="application/ld+json"&gt;</c> body, or an
/// empty string when there is nothing worth emitting. Site-specific/brand JSON-LD (Organization,
/// Person, Service, …) stays in the consuming site until promoted here.
/// </summary>
public interface ISeoJsonLd
{
    /// <summary>
    /// FAQPage from question/answer pairs, or an empty string when there are no items. The answers
    /// must also appear visibly on the page (Google requirement).
    /// </summary>
    string FaqPage(IEnumerable<(string Question, string Answer)> items);

    /// <summary>
    /// BreadcrumbList derived from a URL (the site base comes from <c>Seo:SiteBaseUrl</c>), or an
    /// empty string for the home page — a single "Home" crumb is not worth emitting. Path segments
    /// are title-cased (e.g. "business-websites" → "Business Websites"); the final crumb keeps the
    /// exact URL passed in.
    /// </summary>
    string Breadcrumbs(string currentUrl);
}
