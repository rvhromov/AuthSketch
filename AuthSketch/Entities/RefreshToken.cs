namespace AuthSketch.Entities;

public class RefreshToken
{
    private RefreshToken()
    {
    }

    public int Id { get; }
    public string Token { get; private set; }
    public DateTime CreatedAt { get; private set; }
    public DateTime? ModifiedAt { get; private set; }
    public string CreatedByIp { get; private set; }
    public DateTime ExpiresAt { get; private set; }
    public DateTime? RevokedAt { get; private set; }
    public string RevokedByIp { get; private set; }
    public string ReplacedByToken { get; private set; }
    public int UserId { get; }

    public bool IsExpired => DateTime.UtcNow > ExpiresAt;
    public bool IsRevoked => RevokedAt.HasValue;

    public static RefreshToken Create(string token, string ipAddress, DateTime expiresAt)
    {
        return new()
        {
            Token = token,
            CreatedByIp = ipAddress,
            ExpiresAt = expiresAt,
            CreatedAt = DateTime.UtcNow
        };
    }

    public void Revoke(string ipAddress, string replacedByToken = null)
    {
        var dateTimeNow = DateTime.UtcNow;

        RevokedAt = dateTimeNow;
        ExpiresAt = dateTimeNow;
        RevokedByIp = ipAddress;
        ReplacedByToken = replacedByToken;
        ModifiedAt = dateTimeNow;
    }
}