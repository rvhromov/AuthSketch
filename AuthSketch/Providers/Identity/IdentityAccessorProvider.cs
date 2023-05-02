using AuthSketch.Enums;
using System.Globalization;
using System.Security.Claims;

namespace AuthSketch.Providers.Identity;

public class IdentityAccessorProvider :  IIdentityAccessorProvider
{
    private readonly ClaimsPrincipal _user;

    public IdentityAccessorProvider(IHttpContextAccessor contextAccessor)
    {
        _user = contextAccessor.HttpContext.User;
    }

    public int GetUserId()
    {
        var id = _user.FindFirstValue(ClaimTypes.NameIdentifier);

        if (id is null)
        {
            return default;
        }

        var idParsed = int.TryParse(id, CultureInfo.InvariantCulture, out var userId);

        return idParsed ? userId : default;
    }

    public RoleType GetUserRole()
    {
        var role = _user.FindFirstValue(ClaimTypes.Role);

        if (string.IsNullOrWhiteSpace(role))
        {
            return default;
        }

        var roleParsed = Enum.TryParse<RoleType>(role, true, out var userRole);

        return roleParsed ? userRole : default;
    }

    public (int id, RoleType role) GetUserIdAndRole() => (GetUserId(), GetUserRole());

    public string GetUserEmail() => _user.FindFirstValue(ClaimTypes.Email);
}
