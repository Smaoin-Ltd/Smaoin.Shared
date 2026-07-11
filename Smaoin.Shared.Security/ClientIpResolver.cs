using System.Net;
using Microsoft.AspNetCore.Http;

namespace Smaoin.Shared.Security;

/// <summary>
/// Resolves the real client IP for a request that arrives behind Cloudflare and a reverse proxy
/// (e.g. Caddy). Returns the first candidate header value that ACTUALLY parses as an IP address,
/// then falls back to the transport connection peer.
/// <para>
/// Header values are never relayed unvalidated: an unresolved reverse-proxy placeholder (such as a
/// literal <c>{http.request.client_ip}</c>) or any spoofed non-IP string is skipped. Callers can
/// therefore use the result directly as a rate-limiting / abuse key, for geo/tax lookups, or in
/// logs without a bad value poisoning a shared bucket or 4xx-ing a downstream API.
/// </para>
/// </summary>
public static class ClientIpResolver
{
    /// <summary>
    /// Default header priority: the reverse-proxy-set real IP, then Cloudflare's connecting IP,
    /// then the left-most <c>X-Forwarded-For</c> hop. Only trust these when the origin is reachable
    /// solely via the proxy / Cloudflare — otherwise a client could forge them.
    /// </summary>
    public static readonly string[] DefaultHeaderPriority = ["X-Real-IP", "CF-Connecting-IP", "X-Forwarded-For"];

    /// <summary>
    /// Resolves the client IP for the current request, or <c>null</c> when none can be determined.
    /// </summary>
    public static string? Resolve(HttpContext context, IReadOnlyList<string>? headerPriority = null)
    {
        ArgumentNullException.ThrowIfNull(context);
        return Resolve(context.Request.Headers, context.Connection.RemoteIpAddress, headerPriority);
    }

    /// <summary>
    /// Header/peer overload — resolves without an <see cref="HttpContext"/> so it is trivially unit-testable.
    /// </summary>
    public static string? Resolve(IHeaderDictionary headers, IPAddress? peer, IReadOnlyList<string>? headerPriority = null)
    {
        ArgumentNullException.ThrowIfNull(headers);

        foreach (var header in headerPriority ?? DefaultHeaderPriority)
        {
            // X-Forwarded-For may carry a comma-separated chain — take the left-most parseable hop.
            var raw = headers[header].ToString();
            foreach (var candidate in raw.Split(',', StringSplitOptions.RemoveEmptyEntries | StringSplitOptions.TrimEntries))
            {
                if (IPAddress.TryParse(candidate, out var ip))
                    return ip.ToString();
            }
        }

        return peer?.ToString();
    }
}

/// <summary>Convenience access to <see cref="ClientIpResolver"/> from an <see cref="HttpContext"/>.</summary>
public static class HttpContextClientIpExtensions
{
    /// <summary>
    /// Returns the validated real client IP (see <see cref="ClientIpResolver"/>), or <c>null</c>.
    /// </summary>
    public static string? GetClientIp(this HttpContext context, IReadOnlyList<string>? headerPriority = null)
        => ClientIpResolver.Resolve(context, headerPriority);
}
