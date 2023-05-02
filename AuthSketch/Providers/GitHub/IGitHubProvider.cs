using System.Text.Json;

namespace AuthSketch.Providers.GitHub;

public interface IGitHubProvider
{
    Task<JsonElement> GetUserAsync(string accessToken);
    Task<string> GetUserEmailAsync(string accessToken);
    Task<bool> VerifyAccessTokenAsync(string accessToken);
}