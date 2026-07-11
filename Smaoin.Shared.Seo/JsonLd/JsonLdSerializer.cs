using System.Text.Json;
using System.Text.Json.Serialization;

namespace Smaoin.Shared.Seo.JsonLd;

public sealed class JsonLdSerializer : IJsonLdSerializer
{
    private static readonly JsonSerializerOptions Options = new()
    {
        DefaultIgnoreCondition = JsonIgnoreCondition.WhenWritingNull,
        WriteIndented = false
    };

    public string Serialize(object schema) => JsonSerializer.Serialize(schema, Options);
}
