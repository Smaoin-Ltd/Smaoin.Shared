using System.Text.Json.Serialization;

namespace Smaoin.Shared.Seo.JsonLd;

public sealed class OrganizationSchema
{
    [JsonPropertyName("@context")] public string Context { get; } = "https://schema.org";
    [JsonPropertyName("@type")] public string Type { get; } = "Organization";
    [JsonPropertyName("name")] public string? Name { get; set; }
    [JsonPropertyName("url")] public string? Url { get; set; }
    [JsonPropertyName("logo")] public string? Logo { get; set; }
    [JsonPropertyName("telephone")] public string? Telephone { get; set; }
    [JsonPropertyName("email")] public string? Email { get; set; }
    [JsonPropertyName("address")] public PostalAddressSchema? Address { get; set; }
}

public sealed class PostalAddressSchema
{
    [JsonPropertyName("@type")] public string Type { get; } = "PostalAddress";
    [JsonPropertyName("streetAddress")] public string? StreetAddress { get; set; }
    [JsonPropertyName("addressLocality")] public string? AddressLocality { get; set; }
    [JsonPropertyName("postalCode")] public string? PostalCode { get; set; }
    [JsonPropertyName("addressCountry")] public string? AddressCountry { get; set; }
}
