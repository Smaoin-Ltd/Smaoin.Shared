using System.ComponentModel.DataAnnotations;

namespace Smaoin.Shared.Seo;

public sealed class SeoSettings
{
    public const string SectionName = "Seo";

    [Required] public string SiteBaseUrl { get; set; } = string.Empty;
    [Required] public string SiteName { get; set; } = string.Empty;
    public string? DefaultOgImageUrl { get; set; }
    public string? TwitterHandle { get; set; }
}
