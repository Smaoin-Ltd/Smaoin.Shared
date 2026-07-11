namespace Smaoin.Shared.Captcha;

public interface ICaptchaVerificationService
{
    Task<bool> ValidateAsync(string token, CancellationToken cancellationToken = default);
}
