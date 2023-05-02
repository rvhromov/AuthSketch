namespace AuthSketch.Options;

public sealed record AppOptions
{
    public string Email { get; init; }
    public string Origin { get; init; }
    public string SmtpHost { get; init; }
    public int SmtpPort { get; init; }
    public string SmtpUsername { get; init; }
    public string SmtpPassword { get; init; }
    public string RedirectUrl { get; init; }
}
