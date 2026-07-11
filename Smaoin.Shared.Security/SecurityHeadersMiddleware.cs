using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Options;

namespace Smaoin.Shared.Security;

public sealed class SecurityHeadersMiddleware
{
    private readonly RequestDelegate _next;
    private readonly SecurityHeadersOptions _options;

    public SecurityHeadersMiddleware(
        RequestDelegate next,
        IOptions<SecurityHeadersOptions> options)
    {
        _next = next;
        _options = options.Value;
    }

    public async Task InvokeAsync(HttpContext context)
    {
        var headers = context.Response.Headers;

        if (_options.UseXContentTypeOptions)
            headers["X-Content-Type-Options"] = "nosniff";

        if (_options.UseXFrameOptions)
            headers["X-Frame-Options"] = "DENY";

        if (_options.UseReferrerPolicy)
            headers["Referrer-Policy"] = "strict-origin-when-cross-origin";

        if (_options.ContentSecurityPolicy is not null)
        {
            var nonce = context.RequestServices.GetRequiredService<INonceService>().Nonce;
            var csp = _options.UseNonce
                ? _options.ContentSecurityPolicy!.Replace("{nonce}", nonce)
                : _options.ContentSecurityPolicy;
            headers["Content-Security-Policy"] = csp;
        }

        if (_options.UsePermissionsPolicy)
            headers["Permissions-Policy"] = _options.PermissionsPolicy;

        if (_options.UseCrossOriginOpenerPolicy)
            headers["Cross-Origin-Opener-Policy"] = _options.CrossOriginOpenerPolicy;

        await _next(context);
    }
}
