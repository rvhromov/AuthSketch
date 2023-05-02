namespace AuthSketch.Models.Registration;

public record SignUpRequest(string Name, string Email, string Password, string PasswordConfirmation);
