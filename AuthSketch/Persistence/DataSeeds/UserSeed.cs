using AuthSketch.Enums;
using System.Security.Cryptography;

namespace AuthSketch.Persistence.DataSeeds;

public static class UserSeed
{
    public static IEnumerable<object> GetData()
    {
        // qwerty
        var passwordHash = "8AA0FCD19E374352DA62AEA1F176E8ACBBE3C4B9ECF7B5520EF514E3C0CDAE5C9195D6DC39580919EA569FD4B14C5A5F2DB0A882BE221E4A597BC6F8CE5AEA5D";
        var salt = "6A5EF455C2FF6F3C554831666058A1943C0252ED0FC47BF4C1B3E39462F575A45CE8D3688C65E566FCBDABE3309DA91511051668E4B0EC6278E7D10BFF3A2B95";
        var verificationToken = Convert.ToHexString(RandomNumberGenerator.GetBytes(64));

        // Create and return an anonymous object because .HasData() method requires an Id to be set
        // which is challenging because it has a private setter
        var adminUser = new
        {
            Id = 1,
            Name = "Admin",
            Email = "admin@auth.com",
            PasswordHash = passwordHash,
            Salt = salt,
            VerificationToken = verificationToken,
            CreatedAt = DateTime.UtcNow,
            Role = RoleType.Admin,
            VerifiedAt = DateTime.UtcNow,
            IsTfaEnabled = false
        };

        var basicUser = new 
        { 
            Id = 2,
            Name = "User", 
            Email = "user@auth.com",
            PasswordHash = passwordHash, 
            Salt = salt, 
            VerificationToken = verificationToken,
            CreatedAt = DateTime.UtcNow,
            Role = RoleType.User,
            VerifiedAt = DateTime.UtcNow,
            IsTfaEnabled = false
        };

        return new[] { adminUser, basicUser };
    }
}