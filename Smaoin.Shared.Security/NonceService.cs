using System.Security.Cryptography;

namespace Smaoin.Shared.Security;

/// <summary>
/// Generates a cryptographically random nonce per HTTP request.
/// Registered as Scoped so the same value is returned for the lifetime of one request.
/// </summary>
public sealed class NonceService : INonceService
{
    private readonly string _nonce = Convert.ToBase64String(RandomNumberGenerator.GetBytes(16));
    public string Nonce => _nonce;
}
