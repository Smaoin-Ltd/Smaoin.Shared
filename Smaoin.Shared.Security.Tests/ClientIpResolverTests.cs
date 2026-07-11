using System.Net;
using Microsoft.AspNetCore.Http;
using Xunit;

namespace Smaoin.Shared.Security.Tests;

public class ClientIpResolverTests
{
    private static HeaderDictionary Headers(params (string Key, string Value)[] pairs)
    {
        var h = new HeaderDictionary();
        foreach (var (key, value) in pairs)
            h[key] = value;
        return h;
    }

    [Fact]
    public void Prefers_X_Real_IP_over_lower_priority_headers()
    {
        var ip = ClientIpResolver.Resolve(
            Headers(("X-Real-IP", "203.0.113.7"), ("CF-Connecting-IP", "198.51.100.1")),
            IPAddress.Parse("10.0.0.1"));

        Assert.Equal("203.0.113.7", ip);
    }

    [Fact]
    public void Takes_leftmost_parseable_hop_from_forwarded_for()
    {
        var ip = ClientIpResolver.Resolve(
            Headers(("X-Forwarded-For", "203.0.113.9, 70.41.3.18, 150.172.238.178")),
            peer: null);

        Assert.Equal("203.0.113.9", ip);
    }

    [Fact]
    public void Skips_unresolved_reverse_proxy_placeholder()
    {
        var ip = ClientIpResolver.Resolve(
            Headers(("X-Real-IP", "{http.request.client_ip}"), ("CF-Connecting-IP", "198.51.100.4")),
            peer: null);

        Assert.Equal("198.51.100.4", ip);
    }

    [Fact]
    public void Ignores_spoofed_non_ip_and_falls_back_to_peer()
    {
        var ip = ClientIpResolver.Resolve(
            Headers(("X-Real-IP", "definitely-not-an-ip")),
            IPAddress.Parse("192.0.2.9"));

        Assert.Equal("192.0.2.9", ip);
    }

    [Fact]
    public void Falls_back_to_peer_when_no_headers_present()
    {
        var ip = ClientIpResolver.Resolve(Headers(), IPAddress.Parse("192.0.2.55"));

        Assert.Equal("192.0.2.55", ip);
    }

    [Fact]
    public void Returns_null_when_nothing_resolvable()
    {
        Assert.Null(ClientIpResolver.Resolve(Headers(), peer: null));
    }
}
