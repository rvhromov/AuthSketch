using System.Text.Json.Serialization;

namespace AuthSketch.Models.Registration;

public record SignInRequest
{
    public string Email { get; init; }
    public string Password { get; init; }
    public string TotpCode { get; init; }

    [JsonIgnore]
    public string IpAddress { get; set; }
};