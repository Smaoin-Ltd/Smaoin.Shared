using Smaoin.Shared.Seo.Sitemap;
using Xunit;

namespace Smaoin.Shared.Seo.Tests;

public sealed class SitemapBuilderTests
{
    private readonly SitemapBuilder _sut = new();

    [Fact]
    public void Build_ContainsUrlEntries()
    {
        var entries = new[]
        {
            new SitemapEntry { Url = "https://example.com/", Priority = 1.0 },
            new SitemapEntry { Url = "https://example.com/about", Priority = 0.8 }
        };

        var xml = _sut.Build(entries);

        Assert.Contains("https://example.com/", xml);
        Assert.Contains("https://example.com/about", xml);
    }
}
