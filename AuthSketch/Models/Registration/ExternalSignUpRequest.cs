using AuthSketch.Enums;

namespace AuthSketch.Models.Registration;

public sealed class ExternalSignUpRequest
{
    public string Name { get; set; }
    public string Email { get; set; }
    public AuthProvider Provider { get; set; }
    public string AccessToken { get; set; }
}
