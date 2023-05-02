namespace AuthSketch.Models.Tfa;

public sealed class TfaResponse
{
    public string AuthenticatorKey { get; set; }
    public string FormattedKey { get; set; }
}