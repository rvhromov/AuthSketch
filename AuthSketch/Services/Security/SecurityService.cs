using AuthSketch.Extensions;
using AuthSketch.Models.Security;
using AuthSketch.Options;
using System.Security.Cryptography;
using System.Text;

namespace AuthSketch.Services.Security;

internal sealed class SecurityService : ISecurityService
{
    private readonly SecurityOptions _securityOptions;

    public SecurityService(IConfiguration configuration)
    {
        _securityOptions = configuration.GetOptions<SecurityOptions>(nameof(SecurityOptions));
    }

    public bool PasswordMatch(string plainPassword, string plainConfirmPassword) =>
        plainPassword == plainConfirmPassword;

    public string GenerateRandomKey()
    {
        var randomBytes = RandomNumberGenerator.GetBytes(_securityOptions.KeySize);

        return Convert.ToHexString(randomBytes);
    }

    public HashedPasswordModel HashPassword(string plainPassword)
    {
        var salt = RandomNumberGenerator.GetBytes(_securityOptions.KeySize);

        var hash = Rfc2898DeriveBytes.Pbkdf2(
            Encoding.UTF8.GetBytes(plainPassword),
            salt,
            _securityOptions.Interactions,
            HashAlgorithmName.SHA512,
            _securityOptions.KeySize);

        return new HashedPasswordModel
        {
            Password = Convert.ToHexString(hash),
            Salt = Convert.ToHexString(salt)
        };
    }

    public bool VerifyPassword(string plainPassword, string hash, string salt)
    {
        byte[] saltBytes = Convert.FromHexString(salt);

        var hashToCompare = Rfc2898DeriveBytes.Pbkdf2(
            plainPassword,
            saltBytes,
            _securityOptions.Interactions,
            HashAlgorithmName.SHA512,
            _securityOptions.KeySize);

        return hashToCompare.SequenceEqual(Convert.FromHexString(hash));
    }
}
