using Microsoft.Extensions.DependencyInjection;

namespace Smaoin.Shared.Cdn;

public static class ServiceCollectionExtensions
{
    public static IServiceCollection AddSmaoinCdn(this IServiceCollection services)
    {
        services.AddOptions<CdnOptions>()
            .BindConfiguration(CdnOptions.SectionName)
            .ValidateDataAnnotations()
            .ValidateOnStart();
        services.AddHttpContextAccessor();
        services.AddScoped<ICdnAssetService, CdnAssetService>();
        return services;
    }
}
