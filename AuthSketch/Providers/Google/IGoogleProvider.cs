namespace AuthSketch.Providers.Google
{
    public interface IGoogleProvider
    {
        Task<bool> VerifyAccessTokenAsync(string accessToken);
    }
}
