namespace AuthSketch.Options;

public record GitHubOptions
{
    public string ClientId { get; init; }
    public string ClientSecret { get; init; }
    public string AuthorizationEndpoint { get; init; }
    public string TokenEndpoint { get; init; }
    public string UserInformationEndpoint { get; init; }
    public string CallbackPath { get; init; }
    public string TokenValidationEndpoint { get; init; }
    public string UserEmailEndpoint { get; init; }
    public bool SaveTokens { get; init; }
}