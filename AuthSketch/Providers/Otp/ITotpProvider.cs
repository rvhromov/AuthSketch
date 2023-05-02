namespace AuthSketch.Providers.Otp;

public interface ITotpProvider
{
    bool VerifyTotp(string userTfaKey, string totpCode);
    string GetTotpCode(string userTfaKey);
}
