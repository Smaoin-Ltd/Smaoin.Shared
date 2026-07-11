namespace Smaoin.Shared.Cdn;

/// <summary>
/// Generates fully-qualified, cache-busted CDN URLs for webroot-relative asset paths.
/// </summary>
public interface ICdnAssetService
{
    /// <summary>
    /// Returns the CDN URL for a webroot-relative path with a content-hash query string.
    /// Example: /assets/js/site.min.js → https://cdn.example.com/assets/js/site.min.js?v=abc123
    /// </summary>
    string Url(string path);
}
