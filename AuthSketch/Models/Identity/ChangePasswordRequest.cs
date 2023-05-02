using System.Text.Json.Serialization;

namespace AuthSketch.Models.Identity;

public sealed record ChangePasswordRequest(string CurrentPassword, string NewPassword, string NewPasswordConfirmation)
{
    [JsonIgnore]
    public string IpAddress { get; set; }
}
