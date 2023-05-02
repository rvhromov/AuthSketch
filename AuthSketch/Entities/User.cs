using AuthSketch.Enums;

namespace AuthSketch.Entities;

public class User
{
    private User()
    {
    }

    public int Id { get; }
    public string Name { get; private set; }
    public string Email { get; private set; }
    public string PasswordHash { get; private set; }
    public string Salt { get; private set; }
    public DateTime CreatedAt { get; private set; }
    public DateTime? ModifiedAt { get; private set; }
    public RoleType Role { get; private set; }
    public string VerificationToken { get; private set; }
    public DateTime? VerifiedAt { get; private set; }
    public string ResetToken { get; private set; }
    public DateTime? ResetTokenExpiresAt { get; private set; }
    public DateTime? ResetTokenUsedAt { get; private set; }
    public bool IsTfaEnabled { get; private set; }
    public string TfaKey { get; private set; }
    public DateTime? TfaEnabledAt { get; private set; }

    public ICollection<RefreshToken> RefreshTokens { get; private set; } = new List<RefreshToken>();
    public ICollection<ExternalAuth> ExternalAuthProviders { get; private set; } = new List<ExternalAuth>();

    public bool IsVerified => VerifiedAt.HasValue;

    public static User Create(string name, string email, string passwordHash, string salt, string verificationToken)
    {
        return new()
        {
            Email = email,
            Name = name,
            PasswordHash = passwordHash,
            Salt = salt,
            VerificationToken = verificationToken,
            Role = RoleType.User,
            CreatedAt = DateTime.UtcNow
        };
    }

    public static User Create(string name, string email, AuthProvider provider, string accessToken)
    {
        var dateTimeNow = DateTime.UtcNow;
        var externalAuthProvider = ExternalAuth.Create(provider, accessToken);

        return new()
        {
            Email = email,
            Name = name,
            Role = RoleType.User,
            VerifiedAt = dateTimeNow,
            CreatedAt = dateTimeNow,
            ExternalAuthProviders = new List<ExternalAuth> { externalAuthProvider }
        };
    }

    public void Verify()
    {
        if (IsVerified)
        {
            return;
        }

        var dateTimeNow = DateTime.UtcNow;

        VerificationToken = null;
        VerifiedAt = dateTimeNow;
        ModifiedAt = dateTimeNow;
    }

    public void AddRefreshToken(RefreshToken refreshToken)
    {
        RefreshTokens.Add(refreshToken);
        ModifiedAt = DateTime.UtcNow;
    }

    public void SetResetToken(string token, DateTime expiresAt)
    {
        ResetToken = token;
        ResetTokenExpiresAt = expiresAt;
        ModifiedAt = DateTime.UtcNow;
    }

    public void ResetPassword(string password, string salt)
    {
        PasswordHash = password;
        Salt = salt;
        ResetToken = null;
        ResetTokenExpiresAt = null;
        ResetTokenUsedAt = DateTime.UtcNow;
        ModifiedAt = DateTime.UtcNow;
    }

    public void UpdatePassword(string password, string salt)
    {
        PasswordHash = password;
        Salt = salt;
        ModifiedAt = DateTime.UtcNow;
    }

    public void EnableTfa(string tfaKey)
    {
        var dateTimeNow = DateTime.UtcNow;

        TfaKey = tfaKey;
        IsTfaEnabled = true;
        TfaEnabledAt = dateTimeNow;
        ModifiedAt = dateTimeNow;
    }

    public void DisableTfa()
    {
        TfaKey = null;
        IsTfaEnabled = false;
        TfaEnabledAt = null;
        ModifiedAt = DateTime.UtcNow;
    }

    public void AddAuthProvider(AuthProvider provider, string accessToken)
    {
        var requestedProvider = ExternalAuthProviders.FirstOrDefault(x => x.Provider == provider);

        if (requestedProvider is not null)
        {
            return;
        }

        var externalAuthProvider = ExternalAuth.Create(provider, accessToken);
        ExternalAuthProviders.Add(externalAuthProvider);
        ModifiedAt = DateTime.UtcNow;
    }

    public void ResetAuthProviderAccessToken(AuthProvider provider, string accessToken)
    {
        var requestedProvider = ExternalAuthProviders.FirstOrDefault(x => x.Provider == provider);

        if (requestedProvider is null)
        {
            return;
        }

        requestedProvider.ResetToken(accessToken);
        ModifiedAt = DateTime.UtcNow;
    }
}