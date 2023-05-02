using AuthSketch.Enums;

namespace AuthSketch.Models.Dumb;

public sealed class DumbClaims
{
    public int UserId { get; set; }
    public RoleType UserRole { get; set; }
    public string Email { get; set; }
}
