namespace AuthSketch.Options;

public record SecurityOptions
{
    public int KeySize { get; init; }
    public int Interactions { get; init; }
}
