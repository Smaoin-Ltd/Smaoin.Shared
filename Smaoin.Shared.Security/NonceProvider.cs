using System.Security.Cryptography;
using Microsoft.AspNetCore.Http;

namespace Smaoin.Shared.Security;

public sealed class NonceProvider : INonceProvider
{
    private const string NonceKey = "csp-nonce";
    private readonly IHttpContextAccessor _contextAccessor;

    public NonceProvider(IHttpContextAccessor contextAccessor)
    {
        _contextAccessor = contextAccessor;
    }

    public string GetNonce()
    {
        var context = _contextAccessor.HttpContext
            ?? throw new InvalidOperationException("No active HttpContext.");

        if (context.Items.TryGetValue(NonceKey, out var existing))
            return (string)existing!;

        var nonce = Convert.ToBase64String(RandomNumberGenerator.GetBytes(16));
        context.Items[NonceKey] = nonce;
        return nonce;
    }
}
