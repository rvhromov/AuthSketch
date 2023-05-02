namespace AuthSketch.Models.Emails;

public sealed record TotpEmail(string Email, string Name, string totpCode);