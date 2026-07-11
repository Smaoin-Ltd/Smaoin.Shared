namespace Smaoin.Shared.Cdn;

public sealed class CdnOptions
{
    public const string SectionName = "Cdn";

    // Empty string means relative URLs (development). Set to CDN origin in deployed environments.
    public string BaseUrl { get; set; } = string.Empty;
}
