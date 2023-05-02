using AuthSketch.Enums;
using System.Text.Json.Serialization;

namespace AuthSketch.Models.Registration;

public sealed record ExternalSignInRequest
{
    public string TotpCode { get; init; }

    [JsonIgnore]
    public string Email { get; set; }
    [JsonIgnore]
    public string IpAddress { get; set; }
    [JsonIgnore]
    public AuthProvider Provider { get; set; }
}
