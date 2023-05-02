namespace AuthSketch.Options;

public sealed record ResetTokenOptions
{
    public int ExpiresInHours { get; init; }
}
