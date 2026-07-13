using Microsoft.Extensions.Options;

namespace Smaoin.Shared.Seo.JsonLd;

/// <inheritdoc />
public sealed class SeoJsonLd(IOptions<SeoSettings> settings, IJsonLdSerializer serializer) : ISeoJsonLd
{
    private readonly SeoSettings _settings = settings.Value;
    private readonly string _baseUrl = settings.Value.SiteBaseUrl.TrimEnd('/');
    // Assets (logos, images) resolve against the CDN; falls back to the site base when unset — matches
    // the previous per-site behaviour exactly.
    private readonly string _cdnBaseUrl = settings.Value.CdnBaseUrl?.TrimEnd('/') ?? settings.Value.SiteBaseUrl.TrimEnd('/');

    // Stable @id anchors so the Organization / WebSite / Person nodes cross-reference by id instead of
    // duplicating the same entity across pages.
    private string OrgId(SeoOrganization org) => $"{_baseUrl}/#{org.OrganizationAnchor}";
    private string SiteId(SeoOrganization org) => $"{_baseUrl}/#{org.WebSiteAnchor}";
    private string PersonId(SeoOrganization org) => $"{_baseUrl}/#{org.Founder?.Anchor}";

    public string FaqPage(IEnumerable<(string Question, string Answer)> items)
    {
        var list = items?.ToArray() ?? [];
        if (list.Length == 0) return string.Empty;

        var schema = new FaqPageSchema
        {
            MainEntity = list.Select(qa => new FaqQuestion
            {
                Name = qa.Question,
                AcceptedAnswer = new FaqAnswer { Text = qa.Answer },
            }).ToList(),
        };
        return serializer.Serialize(schema);
    }

    public string Breadcrumbs(string currentUrl)
    {
        var items = new List<BreadcrumbItem>
        {
            new() { Position = 1, Name = "Home", Url = _baseUrl },
        };

        var path = Uri.TryCreate(currentUrl, UriKind.Absolute, out var uri)
            ? uri.AbsolutePath.Trim('/')
            : currentUrl.Trim('/');

        if (path.Length > 0)
        {
            var segments   = path.Split('/', StringSplitOptions.RemoveEmptyEntries);
            var cumulative = _baseUrl;
            for (var i = 0; i < segments.Length; i++)
            {
                cumulative += "/" + segments[i];
                items.Add(new BreadcrumbItem
                {
                    Position = i + 2,
                    Name     = Titleise(segments[i]),
                    // Last crumb keeps the exact URL passed in (preserves any query/fragment).
                    Url      = i == segments.Length - 1 ? currentUrl : cumulative,
                });
            }
        }

        if (items.Count <= 1) return string.Empty; // Home only — no useful trail.

        return serializer.Serialize(new BreadcrumbSchema { Items = items });
    }

    public SeoBusinessProfile ResolveBusinessProfile()
    {
        var org  = _settings.Organization;
        var addr = org?.Address;
        return new SeoBusinessProfile
        {
            Name            = org?.Name ?? string.Empty,
            Url             = _baseUrl,
            LogoUrl         = _cdnBaseUrl + org?.LogoPath,
            AddressLocality = addr?.AddressLocality ?? string.Empty,
            AddressRegion   = addr?.AddressRegion ?? string.Empty,
            PostalCode      = addr?.PostalCode ?? string.Empty,
            AddressCountry  = addr?.AddressCountry ?? string.Empty,
        };
    }

    public SeoAuthorProfile ResolveAuthorProfile()
    {
        var founder = _settings.Organization?.Founder;
        return new SeoAuthorProfile
        {
            Name                = founder?.Name ?? string.Empty,
            Url                 = _baseUrl + founder?.ProfilePath,
            ImageUrl            = string.IsNullOrWhiteSpace(founder?.ImagePath) ? null : _cdnBaseUrl + founder!.ImagePath,
            SameAsSocialProfile = founder?.SameAs.FirstOrDefault(),
        };
    }

    public string BusinessProfile()
    {
        var org = _settings.Organization;
        if (org is null) return string.Empty;

        var profile = ResolveBusinessProfile();

        // Dictionary keys are preserved verbatim in JSON — required for @context / @type.
        var schema = new Dictionary<string, object>
        {
            ["@context"]    = "https://schema.org",
            ["@type"]       = org.Type,
            ["@id"]         = OrgId(org),
            ["name"]        = profile.Name,
            ["url"]         = profile.Url,
            ["description"] = org.Description ?? string.Empty,
            ["logo"]        = new Dictionary<string, object>
            {
                ["@type"] = "ImageObject",
                ["url"]   = profile.LogoUrl,
            },
            ["image"]       = profile.LogoUrl,
            ["address"]     = new Dictionary<string, object>
            {
                ["@type"]           = "PostalAddress",
                ["addressLocality"] = profile.AddressLocality,
                ["addressRegion"]   = profile.AddressRegion,
                ["postalCode"]      = profile.PostalCode,
                ["addressCountry"]  = profile.AddressCountry,
            },
            ["areaServed"]  = org.AreaServed
                .Select(a => new Dictionary<string, object> { ["@type"] = a.Type, ["name"] = a.Name })
                .ToArray<object>(),
            ["founder"]     = new Dictionary<string, object>
            {
                ["@type"] = "Person",
                ["@id"]   = PersonId(org),
                ["name"]  = org.Founder?.Name ?? string.Empty,
                ["url"]   = _baseUrl + org.Founder?.ProfilePath,
            },
            ["sameAs"]      = org.SameAs.ToArray(),
            ["hasMap"]      = org.MapUrl ?? string.Empty,
            ["priceRange"]  = org.PriceRange ?? string.Empty,
            ["openingHoursSpecification"] = new Dictionary<string, object>
            {
                ["@type"]     = "OpeningHoursSpecification",
                ["dayOfWeek"] = org.OpeningHours?.DayOfWeek.ToArray() ?? [],
                ["opens"]     = org.OpeningHours?.Opens ?? string.Empty,
                ["closes"]    = org.OpeningHours?.Closes ?? string.Empty,
            },
        };

        return serializer.Serialize(schema);
    }

    public string WebSite()
    {
        var org = _settings.Organization;
        if (org is null) return string.Empty;

        var schema = new Dictionary<string, object>
        {
            ["@context"]  = "https://schema.org",
            ["@type"]     = "WebSite",
            ["@id"]       = SiteId(org),
            ["name"]      = org.Name,
            ["url"]       = _baseUrl,
            ["publisher"] = new Dictionary<string, object> { ["@id"] = OrgId(org) },
            // No potentialAction/SearchAction: a site with no on-site search endpoint must not
            // advertise one.
        };
        return serializer.Serialize(schema);
    }

    public string AuthorProfile()
    {
        var org     = _settings.Organization;
        var founder = org?.Founder;
        if (org is null || founder is null) return string.Empty;

        var person = ResolveAuthorProfile();

        var schema = new Dictionary<string, object>
        {
            ["@context"]   = "https://schema.org",
            ["@type"]      = "Person",
            ["@id"]        = PersonId(org),
            ["name"]       = person.Name,
            ["url"]        = person.Url,
            ["jobTitle"]   = founder.JobTitle ?? string.Empty,
            ["worksFor"]   = new Dictionary<string, object> { ["@id"] = OrgId(org) },
            ["knowsAbout"] = founder.KnowsAbout.ToArray(),
            ["sameAs"]     = founder.SameAs.ToArray(),
        };
        if (!string.IsNullOrWhiteSpace(person.ImageUrl))
            schema["image"] = person.ImageUrl!;

        return serializer.Serialize(schema);
    }

    public string Service(string name, string description, string relativePath)
    {
        var org = _settings.Organization;
        if (org is null) return string.Empty;

        var schema = new Dictionary<string, object>
        {
            ["@context"]    = "https://schema.org",
            ["@type"]       = "Service",
            ["name"]        = name,
            ["description"] = description,
            ["url"]         = _baseUrl + relativePath,
            ["provider"]    = new Dictionary<string, object>
            {
                ["@type"] = org.Type,
                ["name"]  = org.Name,
                ["url"]   = _baseUrl,
            },
            ["areaServed"]  = new Dictionary<string, object>
            {
                ["@type"] = "Country",
                ["name"]  = org.Address?.AddressCountry ?? string.Empty,
            },
        };
        return serializer.Serialize(schema);
    }

    public string SoftwareApplication(string name, string description, string relativePath)
    {
        var org = _settings.Organization;
        if (org is null) return string.Empty;

        var schema = new Dictionary<string, object>
        {
            ["@context"]    = "https://schema.org",
            ["@type"]       = "WebApplication",
            ["name"]        = name,
            ["url"]         = _baseUrl + relativePath,
            ["description"] = description,
        };
        if (!string.IsNullOrWhiteSpace(_settings.DefaultApplicationCategory))
            schema["applicationCategory"] = _settings.DefaultApplicationCategory!;
        schema["operatingSystem"] = "Web browser";
        schema["offers"] = new Dictionary<string, object>
        {
            ["@type"]         = "Offer",
            ["price"]         = "0",
            ["priceCurrency"] = "GBP",
        };
        schema["provider"] = new Dictionary<string, object> { ["@id"] = OrgId(org) };

        return serializer.Serialize(schema);
    }

    public string Article(string headline, string description, string relativePath,
        string? imageUrl = null, string? datePublished = null)
    {
        var org = _settings.Organization;
        if (org is null) return string.Empty;

        var schema = new Dictionary<string, object>
        {
            ["@context"]    = "https://schema.org",
            ["@type"]       = "Article",
            ["headline"]    = headline,
            ["description"] = description,
            ["url"]         = _baseUrl + relativePath,
            ["author"]      = new Dictionary<string, object>
            {
                ["@type"] = "Person",
                ["@id"]   = PersonId(org),
                ["name"]  = org.Founder?.Name ?? string.Empty,
                ["url"]   = _baseUrl + org.Founder?.ProfilePath,
            },
            ["publisher"]   = new Dictionary<string, object>
            {
                ["@type"] = "Organization",
                ["@id"]   = OrgId(org),
                ["name"]  = org.Name,
                ["logo"]  = new Dictionary<string, object>
                {
                    ["@type"] = "ImageObject",
                    ["url"]   = _cdnBaseUrl + org.LogoPath,
                },
            },
        };
        if (!string.IsNullOrWhiteSpace(imageUrl))
            schema["image"] = imageUrl.StartsWith("http", StringComparison.OrdinalIgnoreCase)
                ? imageUrl : _cdnBaseUrl + imageUrl;
        if (!string.IsNullOrWhiteSpace(datePublished))
            schema["datePublished"] = datePublished!;

        return serializer.Serialize(schema);
    }

    /// <summary>Turns a URL slug like "business-websites" into "Business Websites".</summary>
    private static string Titleise(string segment)
    {
        var words = segment.Replace('-', ' ').Replace('_', ' ')
            .Split(' ', StringSplitOptions.RemoveEmptyEntries);
        return string.Join(' ', words.Select(w => char.ToUpperInvariant(w[0]) + w[1..]));
    }
}
