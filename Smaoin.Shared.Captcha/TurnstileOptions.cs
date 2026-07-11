using System.ComponentModel.DataAnnotations;

namespace Smaoin.Shared.Captcha;

public sealed class TurnstileOptions
{
    public const string SectionName = "Turnstile";

    [Required] public string SiteKey { get; set; } = string.Empty;
    [Required] public string SecretKey { get; set; } = string.Empty;

    // SEC-26: bind a verified token to THIS site. Cloudflare returns the hostname the
    // challenge was solved on and the action it was issued for; validating them stops a
    // token farmed on an attacker's clone of the widget (a different hostname) or minted
    // for a low-value action being replayed here. Both default to "unset" → not enforced,
    // so existing consumers are unaffected until they configure them.
    public string[] ExpectedHostnames { get; set; } = System.Array.Empty<string>();
    public string? ExpectedAction { get; set; }

    // SEC-26: HTTP timeout for the siteverify call (seconds). The default HttpClient timeout
    // is 100s — a slow/hung Cloudflare response would pin the abuse-gating request that long
    // (a DoS amplifier). Kept short and configurable.
    [Range(1, 60)] public int TimeoutSeconds { get; set; } = 5;
}
