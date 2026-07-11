using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc.ViewFeatures;
using Microsoft.Extensions.Options;

namespace Smaoin.Shared.Cdn;

public sealed class CdnAssetService(
    IOptions<CdnOptions> options,
    IFileVersionProvider fileVersionProvider,
    IHttpContextAccessor httpContextAccessor) : ICdnAssetService
{
    private readonly string _base = options.Value.BaseUrl.TrimEnd('/');

    public string Url(string path)
    {
        var pathBase = httpContextAccessor.HttpContext?.Request.PathBase ?? PathString.Empty;
        return _base + fileVersionProvider.AddFileVersionToPath(pathBase, path);
    }
}
