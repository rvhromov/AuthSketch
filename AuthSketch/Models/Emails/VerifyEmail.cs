namespace AuthSketch.Models.Emails;

public sealed record VerifyEmail(string Email, string Name, string Token);