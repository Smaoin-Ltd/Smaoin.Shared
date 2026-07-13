namespace Smaoin.Shared.Seo.JsonLd;

/// <summary>
/// Builds ready-to-embed schema.org JSON-LD strings. Brand-agnostic structures (FAQPage, Breadcrumbs)
/// take their data as arguments; the brand structures (Organization, WebSite, Person, Service, …) are
/// driven entirely by <see cref="SeoSettings"/> — configure <c>Seo:Organization</c> once and a site gets
/// them all. Each method returns a serialized <c>&lt;script type="application/ld+json"&gt;</c> body, or an
/// empty string when there is nothing worth emitting (e.g. no brand profile is configured).
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

    /// <summary>
    /// The primary business node (a <c>ProfessionalService</c> / LocalBusiness) from
    /// <c>Seo:Organization</c>: name, address, area served, opening hours, price range, founder and
    /// <c>sameAs</c> profiles. Empty when no organisation is configured.
    /// </summary>
    string BusinessProfile();

    /// <summary>
    /// The <c>WebSite</c> node (site-name entity, published by the organisation). Empty when no
    /// organisation is configured.
    /// </summary>
    string WebSite();

    /// <summary>
    /// The founder as a <c>Person</c> node (E-E-A-T): job title, expertise, employer and profiles.
    /// Empty when no founder is configured.
    /// </summary>
    string AuthorProfile();

    /// <summary>A <c>Service</c> node for an individual service page, provided by the organisation.</summary>
    string Service(string name, string description, string relativePath);

    /// <summary>A <c>WebApplication</c> node for a free on-site tool, provided by the organisation.</summary>
    string SoftwareApplication(string name, string description, string relativePath);

    /// <summary>
    /// An <c>Article</c> node (case study / insight) authored by the founder and published by the
    /// organisation. <paramref name="imageUrl"/> may be absolute or CDN-relative; both
    /// <paramref name="imageUrl"/> and <paramref name="datePublished"/> are omitted when empty.
    /// </summary>
    string Article(string headline, string description, string relativePath,
        string? imageUrl = null, string? datePublished = null);

    /// <summary>The resolved business profile (raw values behind <see cref="BusinessProfile"/>).</summary>
    SeoBusinessProfile ResolveBusinessProfile();

    /// <summary>The resolved founder/author profile (raw values behind <see cref="AuthorProfile"/>).</summary>
    SeoAuthorProfile ResolveAuthorProfile();
}
