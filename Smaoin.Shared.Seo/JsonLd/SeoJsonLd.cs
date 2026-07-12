using Microsoft.Extensions.Options;

namespace Smaoin.Shared.Seo.JsonLd;

/// <inheritdoc />
public sealed class SeoJsonLd(IOptions<SeoSettings> settings, IJsonLdSerializer serializer) : ISeoJsonLd
{
    private readonly string _baseUrl = settings.Value.SiteBaseUrl.TrimEnd('/');

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

    /// <summary>Turns a URL slug like "business-websites" into "Business Websites".</summary>
    private static string Titleise(string segment)
    {
        var words = segment.Replace('-', ' ').Replace('_', ' ')
            .Split(' ', StringSplitOptions.RemoveEmptyEntries);
        return string.Join(' ', words.Select(w => char.ToUpperInvariant(w[0]) + w[1..]));
    }
}
