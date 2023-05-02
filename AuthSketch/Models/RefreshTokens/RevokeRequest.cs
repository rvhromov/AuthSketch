using System.Text.Json.Serialization;

namespace AuthSketch.Models.RefreshTokens;

public sealed record RevokeRequest
{
    public string Token { get; init; }

    [JsonIgnore]
    public string IpAddress { get; set; }
}