using System.ComponentModel.DataAnnotations;

namespace Smaoin.Shared.Seo;

public sealed class SeoSettings
{
    public const string SectionName = "Seo";

    [Required] public string SiteBaseUrl { get; set; } = string.Empty;
    [Required] public string SiteName { get; set; } = string.Empty;
    public string? DefaultOgImageUrl { get; set; }
    public string? TwitterHandle { get; set; }

    /// <summary>
    /// Absolute base URL for static assets (logos, article images). Falls back to
    /// <see cref="SiteBaseUrl"/> when unset. Trailing slashes are trimmed.
    /// </summary>
    public string? CdnBaseUrl { get; set; }

    /// <summary>
    /// schema.org <c>applicationCategory</c> used for <c>WebApplication</c> (SoftwareApplication)
    /// JSON-LD, e.g. "SecurityApplication". Omitted from the output when null/empty.
    /// </summary>
    public string? DefaultApplicationCategory { get; set; }

    /// <summary>
    /// Brand profile powering the Organization / WebSite / Person / Service / … JSON-LD builders.
    /// When null, those builders return an empty string (nothing brand-specific to emit).
    /// </summary>
    public SeoOrganization? Organization { get; set; }
}
