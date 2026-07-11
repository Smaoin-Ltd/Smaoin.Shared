namespace Smaoin.Shared.Security;

public sealed class SecurityHeadersOptions
{
    public const string SectionName = "SecurityHeaders";

    public string? ContentSecurityPolicy { get; set; }
    public bool UseNonce { get; set; } = true;
    public bool UseHsts { get; set; } = true;
    public bool UseXContentTypeOptions { get; set; } = true;
    public bool UseXFrameOptions { get; set; } = true;
    public bool UseReferrerPolicy { get; set; } = true;

    // Permissions-Policy — restrict browser feature access.
    // Default covers the most commonly exploited APIs; override in appsettings if a site
    // legitimately needs camera / microphone / geolocation / payment.
    public bool UsePermissionsPolicy { get; set; } = true;
    public string PermissionsPolicy { get; set; } =
        "camera=(), microphone=(), geolocation=(), payment=()";

    // Cross-Origin-Opener-Policy — isolates the browsing context to prevent cross-origin
    // opener attacks (e.g. Spectre-style side-channel via window.opener).
    public bool UseCrossOriginOpenerPolicy { get; set; } = true;
    public string CrossOriginOpenerPolicy { get; set; } = "same-origin";
}
