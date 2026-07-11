using System.Net.Http.Json;
using System.Text.Json.Serialization;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;

namespace Smaoin.Shared.Captcha;

public sealed class TurnstileVerificationService : ICaptchaVerificationService
{
    internal const string HttpClientName = "Smaoin.Shared.Captcha.Turnstile";
    private const string VerifyUrl = "https://challenges.cloudflare.com/turnstile/v0/siteverify";

    private readonly IHttpClientFactory _factory;
    private readonly TurnstileOptions _options;
    private readonly ILogger<TurnstileVerificationService> _logger;

    public TurnstileVerificationService(
        IHttpClientFactory factory,
        IOptions<TurnstileOptions> options,
        ILogger<TurnstileVerificationService> logger)
    {
        _factory = factory;
        _options = options.Value;
        _logger = logger;
    }

    public async Task<bool> ValidateAsync(string token, CancellationToken cancellationToken = default)
    {
        if (string.IsNullOrWhiteSpace(token))
            return false;

        var payload = new Dictionary<string, string>
        {
            ["secret"] = _options.SecretKey,
            ["response"] = token
        };

        var http = _factory.CreateClient(HttpClientName);

        TurnstileResponse? result;
        try
        {
            var response = await http.PostAsync(
                VerifyUrl,
                new FormUrlEncodedContent(payload),
                cancellationToken);

            result = await response.Content.ReadFromJsonAsync<TurnstileResponse>(cancellationToken: cancellationToken);
        }
        catch (Exception ex)
        {
            // Includes TaskCanceledException from the HTTP timeout (SEC-26) — fail closed.
            _logger.LogError(ex, "Turnstile verification failed");
            return false;
        }

        if (result is null || !result.Success)
        {
            if (result?.ErrorCodes is { Length: > 0 } errors)
                _logger.LogWarning("Turnstile rejected token: {Errors}", string.Join(",", errors));
            return false;
        }

        // SEC-26: a valid token is only accepted if it was solved on one of THIS site's
        // hostnames and (optionally) for the expected action. Both checks are skipped when
        // not configured, so this is backward-compatible.
        if (_options.ExpectedHostnames.Length > 0 &&
            !_options.ExpectedHostnames.Any(h => string.Equals(h, result.Hostname, StringComparison.OrdinalIgnoreCase)))
        {
            _logger.LogWarning("Turnstile hostname mismatch: got '{Got}', expected one of [{Expected}]",
                result.Hostname, string.Join(", ", _options.ExpectedHostnames));
            return false;
        }

        if (!string.IsNullOrEmpty(_options.ExpectedAction) &&
            !string.Equals(_options.ExpectedAction, result.Action, StringComparison.Ordinal))
        {
            _logger.LogWarning("Turnstile action mismatch: got '{Got}', expected '{Expected}'",
                result.Action, _options.ExpectedAction);
            return false;
        }

        return true;
    }

    private sealed record TurnstileResponse
    {
        [JsonPropertyName("success")] public bool Success { get; init; }
        [JsonPropertyName("hostname")] public string? Hostname { get; init; }
        [JsonPropertyName("action")] public string? Action { get; init; }
        [JsonPropertyName("error-codes")] public string[]? ErrorCodes { get; init; }
    }
}
