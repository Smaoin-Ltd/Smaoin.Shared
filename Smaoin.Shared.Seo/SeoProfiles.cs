namespace Smaoin.Shared.Seo;

/// <summary>
/// The resolved business profile — absolute URLs computed from <see cref="SeoSettings"/>. This is the
/// raw data the Organization/LocalBusiness JSON-LD is built from, exposed for callers that want the
/// values without the serialized JSON-LD.
/// </summary>
public sealed class SeoBusinessProfile
{
    public string Name { get; set; } = string.Empty;
    public string Url { get; set; } = string.Empty;
    public string LogoUrl { get; set; } = string.Empty;
    public string AddressLocality { get; set; } = string.Empty;
    public string AddressRegion { get; set; } = string.Empty;
    public string PostalCode { get; set; } = string.Empty;
    public string AddressCountry { get; set; } = string.Empty;
}

/// <summary>
/// The resolved author/founder profile — absolute URLs computed from <see cref="SeoSettings"/>. The raw
/// data behind the Person JSON-LD, exposed for callers that want the values without the JSON-LD.
/// </summary>
public sealed class SeoAuthorProfile
{
    public string Name { get; set; } = string.Empty;
    public string Url { get; set; } = string.Empty;
    public string? ImageUrl { get; set; }
    public string? SameAsSocialProfile { get; set; }
}
