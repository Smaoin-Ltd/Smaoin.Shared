namespace Smaoin.Shared.Seo;

/// <summary>
/// Config-bound brand profile for a site's organisation and its founder. Drives the brand JSON-LD
/// builders (<c>ProfessionalService</c>, <c>WebSite</c>, <c>Person</c>, <c>Service</c>, <c>Article</c>,
/// <c>WebApplication</c>) so a new site configures this once and gets them all. Bound from the
/// <c>Seo:Organization</c> configuration section.
/// </summary>
public sealed class SeoOrganization
{
    /// <summary>Display name, e.g. "Smaoin".</summary>
    public string Name { get; set; } = string.Empty;

    /// <summary>schema.org type for the primary business node, e.g. "ProfessionalService".</summary>
    public string Type { get; set; } = "ProfessionalService";

    /// <summary>One-line business description used as the JSON-LD <c>description</c>.</summary>
    public string? Description { get; set; }

    /// <summary>CDN-relative logo path (prefixed with <see cref="SeoSettings.CdnBaseUrl"/>), e.g. "/assets/images/logo.png".</summary>
    public string? LogoPath { get; set; }

    /// <summary>Postal address for the <c>PostalAddress</c> node.</summary>
    public SeoPostalAddress? Address { get; set; }

    /// <summary>Areas served, emitted as an array of typed area nodes (AdministrativeArea, Country, …).</summary>
    public List<SeoAreaServed> AreaServed { get; set; } = [];

    /// <summary>schema.org <c>priceRange</c>, e.g. "££–£££".</summary>
    public string? PriceRange { get; set; }

    /// <summary>Trading hours for the <c>OpeningHoursSpecification</c> node.</summary>
    public SeoOpeningHours? OpeningHours { get; set; }

    /// <summary>Map / Google Business Profile URL, emitted as <c>hasMap</c>.</summary>
    public string? MapUrl { get; set; }

    /// <summary>Verified social/profile URLs, emitted as the organisation's <c>sameAs</c> array.</summary>
    public List<string> SameAs { get; set; } = [];

    /// <summary>Fragment for the Organization <c>@id</c> anchor: <c>{SiteBaseUrl}/#{OrganizationAnchor}</c>.</summary>
    public string OrganizationAnchor { get; set; } = "organization";

    /// <summary>Fragment for the WebSite <c>@id</c> anchor: <c>{SiteBaseUrl}/#{WebSiteAnchor}</c>.</summary>
    public string WebSiteAnchor { get; set; } = "website";

    /// <summary>The founder / principal, used for the Person node and Article authorship (E-E-A-T).</summary>
    public SeoFounder? Founder { get; set; }
}

/// <summary>schema.org <c>PostalAddress</c> fields.</summary>
public sealed class SeoPostalAddress
{
    public string? AddressLocality { get; set; }
    public string? AddressRegion { get; set; }
    public string? PostalCode { get; set; }
    public string? AddressCountry { get; set; }
}

/// <summary>A single schema.org area-served node (a <c>@type</c> such as AdministrativeArea/Country and its name).</summary>
public sealed class SeoAreaServed
{
    public string Type { get; set; } = string.Empty;
    public string Name { get; set; } = string.Empty;
}

/// <summary>schema.org <c>OpeningHoursSpecification</c> fields.</summary>
public sealed class SeoOpeningHours
{
    public List<string> DayOfWeek { get; set; } = [];
    public string? Opens { get; set; }
    public string? Closes { get; set; }
}

/// <summary>The founder / principal profile used for the Person node and Article authorship.</summary>
public sealed class SeoFounder
{
    /// <summary>Full name, e.g. "Gordon Duthie".</summary>
    public string Name { get; set; } = string.Empty;

    /// <summary>Site-relative profile path (prefixed with <see cref="SeoSettings.SiteBaseUrl"/>), e.g. "/about".</summary>
    public string? ProfilePath { get; set; }

    /// <summary>CDN-relative headshot path (prefixed with <see cref="SeoSettings.CdnBaseUrl"/>), e.g. "/assets/images/founder.webp".</summary>
    public string? ImagePath { get; set; }

    /// <summary>Job title, e.g. "Founder &amp; Principal Software Engineer".</summary>
    public string? JobTitle { get; set; }

    /// <summary>Expertise topics, emitted as <c>knowsAbout</c>.</summary>
    public List<string> KnowsAbout { get; set; } = [];

    /// <summary>Verified personal profile URLs, emitted as the founder's <c>sameAs</c> array.</summary>
    public List<string> SameAs { get; set; } = [];

    /// <summary>Fragment for the Person <c>@id</c> anchor: <c>{SiteBaseUrl}/#{Anchor}</c>, e.g. "gordon-duthie".</summary>
    public string Anchor { get; set; } = "person";
}
