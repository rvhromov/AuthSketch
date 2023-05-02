using AuthSketch.Enums;

namespace AuthSketch.Entities;

public class ExternalAuth
{
    public ExternalAuth()
    {
    }

    public int Id { get; }
    public AuthProvider Provider { get; private set; }
    public string AccessToken { get; private set; }
    public DateTime CreatedAt { get; private set; }
    public DateTime? ModifiedAt { get; private set; }
    public int UserId { get; }

    public static ExternalAuth Create(AuthProvider provider, string accessToken)
    {
        return new()
        {
            Provider = provider,
            AccessToken = accessToken,
            CreatedAt = DateTime.UtcNow
        };
    }

    public void ResetToken(string accessToken)
    {
        AccessToken = accessToken;
        ModifiedAt = DateTime.UtcNow;
    }
}
