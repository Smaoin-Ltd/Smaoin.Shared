using System.Text.Json.Serialization;

namespace Smaoin.Shared.Seo.JsonLd;

public sealed class BreadcrumbSchema
{
    [JsonPropertyName("@context")] public string Context { get; } = "https://schema.org";
    [JsonPropertyName("@type")] public string Type { get; } = "BreadcrumbList";
    [JsonPropertyName("itemListElement")] public List<BreadcrumbItem> Items { get; set; } = [];
}

public sealed class BreadcrumbItem
{
    [JsonPropertyName("@type")] public string Type { get; } = "ListItem";
    [JsonPropertyName("position")] public int Position { get; set; }
    [JsonPropertyName("name")] public string? Name { get; set; }
    [JsonPropertyName("item")] public string? Url { get; set; }
}
