using AuthSketch.Models.Security;

namespace AuthSketch.Services.Security;

public interface ISecurityService
{
    bool PasswordMatch(string plainPassword, string plainConfirmPassword);
    string GenerateRandomKey();
    HashedPasswordModel HashPassword(string plainPassword);
    bool VerifyPassword(string plainPassword, string hash, string salt);
}
