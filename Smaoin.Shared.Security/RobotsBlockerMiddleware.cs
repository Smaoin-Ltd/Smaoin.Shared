using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Hosting;

namespace Smaoin.Shared.Security;

/// <summary>
/// Returns a <c>robots.txt</c> that disallows all crawlers on non-Production environments.
/// Prevents TST / staging content from appearing in search results.
/// Pairs with per-page <c>noindex</c> meta tags for belt-and-suspenders protection.
/// </summary>
public sealed class RobotsBlockerMiddleware
{
    private readonly RequestDelegate    _next;
    private readonly IWebHostEnvironment _env;

    public RobotsBlockerMiddleware(RequestDelegate next, IWebHostEnvironment env)
    {
        _next = next;
        _env  = env;
    }

    public async Task InvokeAsync(HttpContext context)
    {
        if (context.Request.Path == "/robots.txt" && !_env.IsProduction())
        {
            context.Response.StatusCode  = 200;
            context.Response.ContentType = "text/plain";
            await context.Response.WriteAsync("User-agent: *\nDisallow: /\n");
            return;
        }

        await _next(context);
    }
}
