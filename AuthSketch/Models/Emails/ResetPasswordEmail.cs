namespace AuthSketch.Models.Emails;

public sealed record ResetPasswordEmail(string Email, string Name, string Token);
