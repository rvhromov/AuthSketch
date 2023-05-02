namespace AuthSketch.Options;

public record GoogleOptions
{
    public string ClientId { get; init; }
    public string ClientSecret { get; init; }
    public bool SaveTokens { get; init; }
    public string TokenValidationEndpoint { get; init; }
}
