namespace Smaoin.Shared.Security;

/// <summary>
/// Per-request nonce for use in Content-Security-Policy headers and Razor views.
/// Registered as Scoped — one nonce per HTTP request.
/// </summary>
public interface INonceService
{
    /// <summary>The base64-encoded nonce for this request.</summary>
    string Nonce { get; }
}
