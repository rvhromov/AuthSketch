using AuthSketch.Enums;

namespace AuthSketch.Models.Identity;

public class MeResponse
{
    public int Id { get; set; }
    public string Name { get; set; }
    public string Email { get; set; }
    public RoleType Role { get; set; }
    public bool IsVerified { get; set; }
    public DateTime CreatedAt { get; set; }
}
