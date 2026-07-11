using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Options;

namespace Smaoin.Shared.Captcha;

public static class ServiceCollectionExtensions
{
    public static IServiceCollection AddSmaoinTurnstile(this IServiceCollection services)
    {
        services.AddOptions<TurnstileOptions>()
            .BindConfiguration(TurnstileOptions.SectionName)
            .ValidateDataAnnotations()
            .ValidateOnStart();
        // SEC-26: cap the siteverify call so a slow/hung Cloudflare response can't pin the
        // abuse-gating request for the default 100s (DoS amplifier).
        services.AddHttpClient(TurnstileVerificationService.HttpClientName)
            .ConfigureHttpClient((sp, client) =>
                client.Timeout = System.TimeSpan.FromSeconds(
                    sp.GetRequiredService<IOptions<TurnstileOptions>>().Value.TimeoutSeconds));
        services.AddTransient<ICaptchaVerificationService, TurnstileVerificationService>();
        return services;
    }
}
