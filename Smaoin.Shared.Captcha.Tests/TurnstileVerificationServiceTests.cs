using System.Net;
using System.Text;
using Microsoft.Extensions.Logging.Abstractions;
using Microsoft.Extensions.Options;
using Xunit;

namespace Smaoin.Shared.Captcha.Tests;

public class TurnstileVerificationServiceTests
{
    private static TurnstileVerificationService Build(string json, TurnstileOptions options) =>
        new(new StubHttpClientFactory(json), Options.Create(options),
            NullLogger<TurnstileVerificationService>.Instance);

    private static TurnstileOptions Opts(Action<TurnstileOptions>? cfg = null)
    {
        var o = new TurnstileOptions { SiteKey = "sk", SecretKey = "secret" };
        cfg?.Invoke(o);
        return o;
    }

    [Fact]
    public async Task Rejects_empty_token()
    {
        var svc = Build("""{"success":true}""", Opts());
        Assert.False(await svc.ValidateAsync(""));
    }

    [Fact]
    public async Task Accepts_success_when_no_binding_configured()
    {
        var svc = Build("""{"success":true,"hostname":"anywhere.example"}""", Opts());
        Assert.True(await svc.ValidateAsync("t"));
    }

    [Fact]
    public async Task Rejects_when_success_false()
    {
        var svc = Build("""{"success":false,"error-codes":["invalid-input-response"]}""", Opts());
        Assert.False(await svc.ValidateAsync("t"));
    }

    [Fact]
    public async Task Rejects_hostname_mismatch()
    {
        var svc = Build("""{"success":true,"hostname":"evil.example"}""",
            Opts(o => o.ExpectedHostnames = new[] { "tst.example.com" }));
        Assert.False(await svc.ValidateAsync("t"));
    }

    [Fact]
    public async Task Accepts_hostname_match_case_insensitive()
    {
        var svc = Build("""{"success":true,"hostname":"TST.example.com"}""",
            Opts(o => o.ExpectedHostnames = new[] { "tst.example.com" }));
        Assert.True(await svc.ValidateAsync("t"));
    }

    [Fact]
    public async Task Rejects_action_mismatch()
    {
        var svc = Build("""{"success":true,"hostname":"h","action":"login"}""",
            Opts(o => { o.ExpectedHostnames = new[] { "h" }; o.ExpectedAction = "scan"; }));
        Assert.False(await svc.ValidateAsync("t"));
    }

    [Fact]
    public async Task Accepts_action_match()
    {
        var svc = Build("""{"success":true,"hostname":"h","action":"scan"}""",
            Opts(o => { o.ExpectedHostnames = new[] { "h" }; o.ExpectedAction = "scan"; }));
        Assert.True(await svc.ValidateAsync("t"));
    }

    private sealed class StubHttpClientFactory(string json) : IHttpClientFactory
    {
        public HttpClient CreateClient(string name) => new(new StubHandler(json));
    }

    private sealed class StubHandler(string json) : HttpMessageHandler
    {
        protected override Task<HttpResponseMessage> SendAsync(HttpRequestMessage request, CancellationToken cancellationToken)
            => Task.FromResult(new HttpResponseMessage(HttpStatusCode.OK)
            {
                Content = new StringContent(json, Encoding.UTF8, "application/json")
            });
    }
}
