namespace AuthSketch.Models.Security;

public class HashedPasswordModel
{
    public string Password { get; set; }
    public string Salt { get; set; }
}
