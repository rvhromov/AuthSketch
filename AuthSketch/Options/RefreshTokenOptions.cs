namespace AuthSketch.Options;

public record RefreshTokenOptions
{
    public int Size { get; init; }
    public int TailSize { get; init; }
    public int ExpiresInHours { get; init; }
}
