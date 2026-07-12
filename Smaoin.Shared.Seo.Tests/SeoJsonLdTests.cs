using System.Text.Json;
using Microsoft.Extensions.Options;
using Smaoin.Shared.Seo;
using Smaoin.Shared.Seo.JsonLd;
using Xunit;

namespace Smaoin.Shared.Seo.Tests;

public sealed class SeoJsonLdTests
{
    private const string BaseUrl = "https://smaoin.co.uk";

    private static SeoJsonLd Build(string baseUrl = BaseUrl)
        => new(Options.Create(new SeoSettings { SiteBaseUrl = baseUrl, SiteName = "Smaoin" }), new JsonLdSerializer());

    // --- FaqPage ---

    [Fact]
    public void FaqPage_EmptyInput_ReturnsEmpty()
        => Assert.Equal(string.Empty, Build().FaqPage([]));

    [Fact]
    public void FaqPage_BuildsFaqPageWithQuestions()
    {
        var doc = JsonDocument.Parse(Build().FaqPage([("Q1?", "A1."), ("Q2?", "A2.")]));
        Assert.Equal("FAQPage", doc.RootElement.GetProperty("@type").GetString());
        var main = doc.RootElement.GetProperty("mainEntity");
        Assert.Equal(2, main.GetArrayLength());
        Assert.Equal("Question", main[0].GetProperty("@type").GetString());
        Assert.Equal("Q1?", main[0].GetProperty("name").GetString());
        Assert.Equal("Answer", main[0].GetProperty("acceptedAnswer").GetProperty("@type").GetString());
        Assert.Equal("A1.", main[0].GetProperty("acceptedAnswer").GetProperty("text").GetString());
    }

    // --- Breadcrumbs ---

    [Fact]
    public void Breadcrumbs_Home_ReturnsEmpty()
        => Assert.Equal(string.Empty, Build().Breadcrumbs("https://smaoin.co.uk/"));

    [Fact]
    public void Breadcrumbs_DeepUrl_ListsHomePlusSegments()
    {
        var doc = JsonDocument.Parse(Build().Breadcrumbs("https://smaoin.co.uk/services/business-websites"));
        Assert.Equal("BreadcrumbList", doc.RootElement.GetProperty("@type").GetString());
        var items = doc.RootElement.GetProperty("itemListElement");
        Assert.Equal(3, items.GetArrayLength());
    }

    [Fact]
    public void Breadcrumbs_FirstItemIsHome_WithSequentialPositions()
    {
        var doc = JsonDocument.Parse(Build().Breadcrumbs("https://smaoin.co.uk/services/x"));
        var items = doc.RootElement.GetProperty("itemListElement");
        Assert.Equal("Home", items[0].GetProperty("name").GetString());
        Assert.Equal(BaseUrl, items[0].GetProperty("item").GetString());
        Assert.Equal(1, items[0].GetProperty("position").GetInt32());
        Assert.Equal(2, items[1].GetProperty("position").GetInt32());
        Assert.Equal(3, items[2].GetProperty("position").GetInt32());
    }

    [Fact]
    public void Breadcrumbs_TitleCasesSlugs_AndLastCrumbKeepsExactUrl()
    {
        const string url = "https://smaoin.co.uk/services/business-websites";
        var doc = JsonDocument.Parse(Build().Breadcrumbs(url));
        var items = doc.RootElement.GetProperty("itemListElement");
        Assert.Equal("Business Websites", items[2].GetProperty("name").GetString());
        Assert.Equal(url, items[2].GetProperty("item").GetString());
    }
}
