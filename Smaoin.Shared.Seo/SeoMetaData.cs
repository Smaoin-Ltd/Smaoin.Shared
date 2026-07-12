namespace Smaoin.Shared.Seo;

/// <summary>
/// Per-page SEO metadata. The Open Graph and Twitter/X title &amp; description are self-completing:
/// when a page does not set them explicitly they fall back to <see cref="Title"/> / <see cref="Description"/>,
/// so social and AI-search link previews never lean on the raw page &lt;title&gt;. Explicit values always win.
/// </summary>
public sealed class SeoMetaData
{
    public string Title { get; set; } = string.Empty;
    public string Description { get; set; } = string.Empty;
    public string? CanonicalUrl { get; set; }
    public string? OgImageUrl { get; set; }
    public bool NoIndex { get; set; }
    public string? OgType { get; set; } = "website";

    // og:title — explicit value if set, otherwise the page Title.
    private string? _ogTitle;
    public string? OgTitle
    {
        get => Coalesce(_ogTitle, Title);
        set => _ogTitle = value;
    }

    // og:description — explicit value if set, otherwise the page Description.
    private string? _ogDescription;
    public string? OgDescription
    {
        get => Coalesce(_ogDescription, Description);
        set => _ogDescription = value;
    }

    // twitter:title — explicit value if set, otherwise cascades through OgTitle to Title.
    private string? _twitterTitle;
    public string? TwitterTitle
    {
        get => Coalesce(_twitterTitle, OgTitle);
        set => _twitterTitle = value;
    }

    // twitter:description — explicit value if set, otherwise cascades through OgDescription to Description.
    private string? _twitterDescription;
    public string? TwitterDescription
    {
        get => Coalesce(_twitterDescription, OgDescription);
        set => _twitterDescription = value;
    }

    // twitter:card — controls how the link preview renders. Defaults to the large-image card
    // (pages carry an OgImageUrl); override per page with "summary", "app", or "player".
    public string TwitterCard { get; set; } = "summary_large_image";

    // Prefer the explicit value when present; treat null/whitespace as "unset" so a blank
    // never suppresses the fallback. Returns null only when both are blank.
    private static string? Coalesce(string? value, string? fallback)
        => !string.IsNullOrWhiteSpace(value) ? value
            : string.IsNullOrWhiteSpace(fallback) ? null : fallback;
}
