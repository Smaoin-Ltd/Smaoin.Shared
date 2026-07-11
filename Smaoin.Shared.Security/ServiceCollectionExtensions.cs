using Microsoft.AspNetCore.Antiforgery;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.DependencyInjection;

namespace Smaoin.Shared.Security;

public static class ServiceCollectionExtensions
{
    public static IServiceCollection AddSmaoinSecurityHeaders(this IServiceCollection services)
    {
        services.AddOptions<SecurityHeadersOptions>()
            .BindConfiguration(SecurityHeadersOptions.SectionName)
            .ValidateDataAnnotations()
            .ValidateOnStart();
        services.AddHttpContextAccessor();
        services.AddSingleton<INonceProvider, NonceProvider>();
        services.AddScoped<INonceService, NonceService>();
        return services;
    }

    /// <summary>
    /// Registers antiforgery with a hardened <c>__Host-xsrf</c> cookie.
    /// The <c>__Host-</c> prefix enforces Secure + Path=/ + no Domain,
    /// preventing subdomain cookie injection attacks.
    /// </summary>
    public static IServiceCollection AddSmaoinAntiforgery(this IServiceCollection services)
    {
        services.AddAntiforgery();
        services.Configure<AntiforgeryOptions>(options =>
        {
            options.Cookie.Name        = "__Host-xsrf";
            options.Cookie.SecurePolicy = CookieSecurePolicy.Always;
        });
        return services;
    }

    /// <summary>
    /// Adds middleware that returns a <c>robots.txt</c> disallowing all crawlers
    /// on non-Production environments, preventing TST/staging content from being indexed.
    /// Must be placed early in the pipeline, before routing.
    /// </summary>
    public static IApplicationBuilder UseSmaoinRobotsBlocker(this IApplicationBuilder app)
        => app.UseMiddleware<RobotsBlockerMiddleware>();

    public static IApplicationBuilder UseSmaoinSecurityHeaders(this IApplicationBuilder app)
        => app.UseMiddleware<SecurityHeadersMiddleware>();
}
