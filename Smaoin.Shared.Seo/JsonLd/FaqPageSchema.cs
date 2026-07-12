using System.Text.Json.Serialization;

namespace Smaoin.Shared.Seo.JsonLd;

/// <summary>schema.org FAQPage — a list of questions with visible answers, for FAQ rich results.</summary>
public sealed class FaqPageSchema
{
    [JsonPropertyName("@context")] public string Context { get; } = "https://schema.org";
    [JsonPropertyName("@type")] public string Type { get; } = "FAQPage";
    [JsonPropertyName("mainEntity")] public List<FaqQuestion> MainEntity { get; set; } = [];
}

public sealed class FaqQuestion
{
    [JsonPropertyName("@type")] public string Type { get; } = "Question";
    [JsonPropertyName("name")] public string? Name { get; set; }
    [JsonPropertyName("acceptedAnswer")] public FaqAnswer? AcceptedAnswer { get; set; }
}

public sealed class FaqAnswer
{
    [JsonPropertyName("@type")] public string Type { get; } = "Answer";
    [JsonPropertyName("text")] public string? Text { get; set; }
}
