namespace AuthSketch.Models.RefreshTokens;

public sealed record ActiveRefreshTokenResponse
{
    public string TokenTail { get; set; }
    public DateTime CreatedAt { get; set; }
    public string CreatedByIp { get; set; }
}
