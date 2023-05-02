using AuthSketch.Exceptions;
using OtpNet;

namespace AuthSketch.Providers.Otp;

public class TotpProvider : ITotpProvider
{
    public bool VerifyTotp(string userTfaKey, string totpCode)
    {
        if (string.IsNullOrWhiteSpace(userTfaKey) || string.IsNullOrWhiteSpace(totpCode))
        {
            return false;
        }

        var keyBytes = Base32Encoding.ToBytes(userTfaKey);
        var totp = new Totp(keyBytes);

        return totp.VerifyTotp(totpCode, out _, VerificationWindow.RfcSpecifiedNetworkDelay);
    }

    public string GetTotpCode(string userTfaKey)
    {
        if (string.IsNullOrEmpty(userTfaKey))
        {
            throw new ValidationException("Unable to compute TOTP code.");
        }

        var keyBytes = Base32Encoding.ToBytes(userTfaKey);
        var totp = new Totp(keyBytes);

        return totp.ComputeTotp(DateTime.UtcNow);
    }
}
