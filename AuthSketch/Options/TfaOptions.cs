namespace AuthSketch.Options;

public record TfaOptions
{
    public string Issuer { get; init; }
    public int AuthenticatorKeySize { get; init; }
}
