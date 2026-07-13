using System.Text.Json;
using Microsoft.Extensions.Options;
using Smaoin.Shared.Seo;
using Smaoin.Shared.Seo.JsonLd;
using Xunit;

namespace Smaoin.Shared.Seo.Tests;

/// <summary>
/// Byte-parity net for the brand JSON-LD builders promoted from the corporate SeoService. The golden
/// strings in <c>TestData/brand-jsonld-golden.txt</c> were captured verbatim from that service BEFORE
/// the move (base = smaoin.co.uk, cdn = cdn.smaoin.co.uk — kept distinct so any base/cdn mix-up is
/// caught) and are loaded here unaltered. If a builder's output drifts by a single byte, these fail.
/// Structural regression risk concentrates here, so exact-string equality — not "some fields" — is the
/// assertion.
/// </summary>
public sealed class SeoBrandJsonLdTests
{
    private const string BaseUrl = "https://smaoin.co.uk";
    private const string CdnUrl  = "https://cdn.smaoin.co.uk";

    // Smaoin's brand profile — the source-of-truth values promoted from the old SeoService consts.
    // Non-ASCII characters (— em-dash, ££–£££, &, ') are written as their literal characters; the
    // serializer re-escapes them to \uXXXX in its output, exactly as the golden capture recorded.
    private static SeoSettings SmaoinSettings() => new()
    {
        SiteBaseUrl = BaseUrl,
        SiteName = "Smaoin",
        CdnBaseUrl = CdnUrl,
        DefaultApplicationCategory = "SecurityApplication",
        Organization = new SeoOrganization
        {
            Name = "Smaoin",
            Type = "ProfessionalService",
            Description = "Secure, bespoke .NET business systems and integrations for SMEs — from Elgin, Moray, across Scotland and the UK.",
            LogoPath = "/assets/images/smaoin.png",
            Address = new SeoPostalAddress
            {
                AddressLocality = "Elgin",
                AddressRegion = "Scotland",
                PostalCode = "IV30",
                AddressCountry = "GB",
            },
            AreaServed =
            [
                new SeoAreaServed { Type = "AdministrativeArea", Name = "Scotland" },
                new SeoAreaServed { Type = "Country", Name = "United Kingdom" },
            ],
            PriceRange = "££–£££",
            OpeningHours = new SeoOpeningHours
            {
                DayOfWeek = ["Monday", "Tuesday", "Wednesday", "Thursday", "Friday"],
                Opens = "09:00",
                Closes = "17:00",
            },
            MapUrl = "https://maps.app.goo.gl/3nkCEJHA8KiMaWJr7",
            SameAs =
            [
                "https://www.linkedin.com/company/smaoin",
                "https://www.facebook.com/smaoinltd",
                "https://maps.app.goo.gl/3nkCEJHA8KiMaWJr7",
                "https://github.com/Smaoin-Ltd",
            ],
            OrganizationAnchor = "organization",
            WebSiteAnchor = "website",
            Founder = new SeoFounder
            {
                Name = "Gordon Duthie",
                ProfilePath = "/about",
                ImagePath = "/assets/images/gordon-duthie.webp",
                JobTitle = "Founder & Principal Software Engineer",
                KnowsAbout = [".NET", "Microsoft Azure", "Software integration", "Application security", "Bespoke business systems"],
                SameAs = ["https://www.linkedin.com/in/gordonduthie"],
                Anchor = "gordon-duthie",
            },
        },
    };

    private static SeoJsonLd Build() => new(Options.Create(SmaoinSettings()), new JsonLdSerializer());

    private static SeoJsonLd BuildNoBrand()
        => new(Options.Create(new SeoSettings { SiteBaseUrl = BaseUrl, SiteName = "Smaoin" }), new JsonLdSerializer());

    // Golden sections keyed by the labels written during capture.
    private static readonly IReadOnlyDictionary<string, string> Golden = LoadGolden();

    private static Dictionary<string, string> LoadGolden()
    {
        var path = Path.Combine(AppContext.BaseDirectory, "TestData", "brand-jsonld-golden.txt");
        var lines = File.ReadAllLines(path);
        var map = new Dictionary<string, string>(StringComparer.Ordinal);
        for (var i = 0; i < lines.Length; i++)
        {
            var line = lines[i];
            if (!line.StartsWith("<<<", StringComparison.Ordinal) || !line.EndsWith(">>>", StringComparison.Ordinal))
                continue;
            var key = line[3..^3];
            if (key == "END") break;
            map[key] = lines[i + 1];
        }
        return map;
    }

    // --- Byte parity (exact strings captured from the pre-move corporate SeoService) ---

    [Fact]
    public void BusinessProfile_MatchesGolden() => Assert.Equal(Golden["BusinessProfile"], Build().BusinessProfile());

    [Fact]
    public void WebSite_MatchesGolden() => Assert.Equal(Golden["WebSite"], Build().WebSite());

    [Fact]
    public void AuthorProfile_MatchesGolden() => Assert.Equal(Golden["AuthorProfile"], Build().AuthorProfile());

    [Fact]
    public void Service_MatchesGolden()
        => Assert.Equal(Golden["Service"], Build().Service("Security & Hardening | Smaoin", "Lock down your APIs, data, and identity.", "/services/security-hardening"));

    [Fact]
    public void SoftwareApplication_MatchesGolden()
        => Assert.Equal(Golden["SoftwareApplication"], Build().SoftwareApplication("Website Security Scanner", "Free scan of your site's security posture.", "/security-report"));

    [Fact]
    public void Article_FullRelativeImage_MatchesGolden()
        => Assert.Equal(Golden["Article_Full_RelImg"], Build().Article("Scan Yer Van — a field case study", "How we hardened a mobile business.", "/work/scan-yer-van", "/assets/images/work/scan-yer-van.png", "2026-07-11"));

    [Fact]
    public void Article_FullAbsoluteImage_MatchesGolden()
        => Assert.Equal(Golden["Article_Full_AbsImg"], Build().Article("Absolute image article", "Desc.", "/insights/foo", "https://cdn.smaoin.co.uk/assets/images/abs.png", "2026-01-02"));

    [Fact]
    public void Article_NoImageNoDate_MatchesGolden()
        => Assert.Equal(Golden["Article_NoImg_NoDate"], Build().Article("Headline only", "Desc only", "/insights/bar"));

    // --- Resolver DTOs (raw values behind the JSON-LD) ---

    [Fact]
    public void ResolveBusinessProfile_HasAbsoluteUrls()
    {
        var p = Build().ResolveBusinessProfile();
        Assert.Equal("Smaoin", p.Name);
        Assert.Equal(BaseUrl, p.Url);
        Assert.Equal("https://cdn.smaoin.co.uk/assets/images/smaoin.png", p.LogoUrl);
        Assert.Equal("Elgin", p.AddressLocality);
        Assert.Equal("Scotland", p.AddressRegion);
        Assert.Equal("IV30", p.PostalCode);
        Assert.Equal("GB", p.AddressCountry);
    }

    [Fact]
    public void ResolveAuthorProfile_HasAbsoluteUrls()
    {
        var p = Build().ResolveAuthorProfile();
        Assert.Equal("Gordon Duthie", p.Name);
        Assert.Equal("https://smaoin.co.uk/about", p.Url);
        Assert.Equal("https://cdn.smaoin.co.uk/assets/images/gordon-duthie.webp", p.ImageUrl);
        Assert.Equal("https://www.linkedin.com/in/gordonduthie", p.SameAsSocialProfile);
    }

    // --- Behaviour without a brand profile / optional config ---

    [Fact]
    public void BrandBuilders_ReturnEmpty_WhenNoOrganisationConfigured()
    {
        var seo = BuildNoBrand();
        Assert.Equal(string.Empty, seo.BusinessProfile());
        Assert.Equal(string.Empty, seo.WebSite());
        Assert.Equal(string.Empty, seo.AuthorProfile());
        Assert.Equal(string.Empty, seo.Service("n", "d", "/x"));
        Assert.Equal(string.Empty, seo.SoftwareApplication("n", "d", "/x"));
        Assert.Equal(string.Empty, seo.Article("h", "d", "/x"));
    }

    [Fact]
    public void SoftwareApplication_OmitsApplicationCategory_WhenNotConfigured()
    {
        var settings = SmaoinSettings();
        settings.DefaultApplicationCategory = null;
        var seo = new SeoJsonLd(Options.Create(settings), new JsonLdSerializer());
        var doc = JsonDocument.Parse(seo.SoftwareApplication("Tool", "Desc", "/tool"));
        Assert.False(doc.RootElement.TryGetProperty("applicationCategory", out _));
        // The rest of the node is still well-formed.
        Assert.Equal("WebApplication", doc.RootElement.GetProperty("@type").GetString());
        Assert.Equal("0", doc.RootElement.GetProperty("offers").GetProperty("price").GetString());
    }

    [Fact]
    public void CdnBaseUrl_FallsBackToSiteBaseUrl_WhenUnset()
    {
        var settings = SmaoinSettings();
        settings.CdnBaseUrl = null; // logos should now resolve against the site base.
        var seo = new SeoJsonLd(Options.Create(settings), new JsonLdSerializer());
        Assert.Equal("https://smaoin.co.uk/assets/images/smaoin.png", seo.ResolveBusinessProfile().LogoUrl);
    }
}
