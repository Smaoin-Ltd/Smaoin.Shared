using Xunit;

namespace Smaoin.Shared.Security.Tests;

public class NonceServiceTests
{
    [Fact]
    public void Nonce_is_stable_for_the_lifetime_of_one_instance()
    {
        var service = new NonceService();

        Assert.Equal(service.Nonce, service.Nonce);
    }

    [Fact]
    public void Nonce_is_non_empty_valid_base64()
    {
        var service = new NonceService();

        Assert.False(string.IsNullOrEmpty(service.Nonce));
        _ = Convert.FromBase64String(service.Nonce); // throws if not valid base64
    }

    [Fact]
    public void Separate_instances_produce_different_nonces()
    {
        Assert.NotEqual(new NonceService().Nonce, new NonceService().Nonce);
    }
}
