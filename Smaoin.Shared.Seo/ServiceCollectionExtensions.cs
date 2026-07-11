using Microsoft.Extensions.DependencyInjection;
using Smaoin.Shared.Seo.JsonLd;
using Smaoin.Shared.Seo.Sitemap;

namespace Smaoin.Shared.Seo;

public static class ServiceCollectionExtensions
{
    public static IServiceCollection AddSmaoinSeo(this IServiceCollection services)
    {
        services.AddOptions<SeoSettings>()
            .BindConfiguration(SeoSettings.SectionName)
            .ValidateDataAnnotations()
            .ValidateOnStart();
        services.AddSingleton<IJsonLdSerializer, JsonLdSerializer>();
        services.AddSingleton<ISitemapBuilder, SitemapBuilder>();
        return services;
    }
}
