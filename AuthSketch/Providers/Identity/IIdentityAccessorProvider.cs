using AuthSketch.Enums;

namespace AuthSketch.Providers.Identity;

public interface IIdentityAccessorProvider
{
    int GetUserId();
    RoleType GetUserRole();
    (int id, RoleType role) GetUserIdAndRole();
    string GetUserEmail();
}
