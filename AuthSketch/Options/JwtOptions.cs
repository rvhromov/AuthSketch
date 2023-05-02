namespace AuthSketch.Options;

public record JwtOptions
{
    public string Secret { get; init; }
    public string Audience { get; init; }
    public string Issuer { get; init; }
    public bool ValidateKey { get; init; }
    public bool ValidateAudience { get; init; }
    public bool ValidateIssuer { get; init; }
    public bool ValidateLifetime { get; init; }
    public int ExpiresInMinutes { get; init; }
}
