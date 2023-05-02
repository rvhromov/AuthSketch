using System.Text.Json.Serialization;

namespace AuthSketch.Models.Passwords;

public sealed record ResetPasswordRequest(string ResetToken, string Password, string PasswordConfirmation)
{
    [JsonIgnore]
    public string IpAddress { get; set; }
}
