using Xunit;

namespace Smaoin.Shared.Seo.Tests;

public sealed class SeoMetaDataTests
{
    [Fact]
    public void OgAndTwitter_FallBackToTitleAndDescription_WhenNotSet()
    {
        var seo = new SeoMetaData { Title = "Home", Description = "Welcome" };

        Assert.Equal("Home", seo.OgTitle);
        Assert.Equal("Welcome", seo.OgDescription);
        Assert.Equal("Home", seo.TwitterTitle);
        Assert.Equal("Welcome", seo.TwitterDescription);
    }

    [Fact]
    public void ExplicitOgValues_Win_AndTwitterCascadesThroughThem()
    {
        var seo = new SeoMetaData
        {
            Title = "Home",
            Description = "Welcome",
            OgTitle = "OG Home",
            OgDescription = "OG Welcome"
        };

        Assert.Equal("OG Home", seo.OgTitle);
        Assert.Equal("OG Welcome", seo.OgDescription);
        // Twitter not set explicitly → cascades through the OG values, not straight to Title/Description.
        Assert.Equal("OG Home", seo.TwitterTitle);
        Assert.Equal("OG Welcome", seo.TwitterDescription);
    }

    [Fact]
    public void ExplicitTwitterValues_Win_OverOgAndTitle()
    {
        var seo = new SeoMetaData
        {
            Title = "Home",
            Description = "Welcome",
            OgTitle = "OG Home",
            TwitterTitle = "Tweet Home",
            TwitterDescription = "Tweet Welcome"
        };

        Assert.Equal("Tweet Home", seo.TwitterTitle);
        Assert.Equal("Tweet Welcome", seo.TwitterDescription);
    }

    [Theory]
    [InlineData("")]
    [InlineData("   ")]
    public void BlankExplicitValue_DoesNotSuppressFallback(string blank)
    {
        var seo = new SeoMetaData
        {
            Title = "Home",
            Description = "Welcome",
            OgTitle = blank,
            OgDescription = blank
        };

        Assert.Equal("Home", seo.OgTitle);
        Assert.Equal("Welcome", seo.OgDescription);
    }

    [Fact]
    public void AllBlank_ReturnsNull()
    {
        var seo = new SeoMetaData();

        Assert.Null(seo.OgTitle);
        Assert.Null(seo.OgDescription);
        Assert.Null(seo.TwitterTitle);
        Assert.Null(seo.TwitterDescription);
    }

    [Fact]
    public void TwitterCard_DefaultsToLargeImage_AndIsOverridable()
    {
        Assert.Equal("summary_large_image", new SeoMetaData().TwitterCard);
        Assert.Equal("summary", new SeoMetaData { TwitterCard = "summary" }.TwitterCard);
    }
}
