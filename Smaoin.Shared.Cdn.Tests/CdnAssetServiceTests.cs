using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc.ViewFeatures;
using Microsoft.Extensions.Options;
using Xunit;

namespace Smaoin.Shared.Cdn.Tests;

public class CdnAssetServiceTests
{
    private static CdnAssetService Build(string baseUrl, HttpContext? context = null) =>
        new(Options.Create(new CdnOptions { BaseUrl = baseUrl }),
            new StubFileVersionProvider(),
            new StubHttpContextAccessor(context));

    [Fact]
    public void Prefixes_base_url_and_appends_version()
    {
        var service = Build("https://cdn.example.com");

        Assert.Equal("https://cdn.example.com/assets/site.css?v=abc123", service.Url("/assets/site.css"));
    }

    [Fact]
    public void Trims_trailing_slashes_from_base_url()
    {
        var service = Build("https://cdn.example.com///");

        Assert.Equal("https://cdn.example.com/assets/site.css?v=abc123", service.Url("/assets/site.css"));
    }

    [Fact]
    public void Empty_base_url_produces_a_relative_url()
    {
        var service = Build("");

        Assert.Equal("/assets/site.css?v=abc123", service.Url("/assets/site.css"));
    }

    // Returns the path unchanged with a deterministic version stamp, so the test asserts
    // the service's own base-url + composition behaviour rather than hashing.
    private sealed class StubFileVersionProvider : IFileVersionProvider
    {
        public string AddFileVersionToPath(PathString requestPathBase, string path) => path + "?v=abc123";
    }

    private sealed class StubHttpContextAccessor(HttpContext? context) : IHttpContextAccessor
    {
        public HttpContext? HttpContext { get; set; } = context;
    }
}
