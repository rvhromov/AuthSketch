using AuthSketch.Extensions;
using System.Net.Http.Headers;
using GoogleOptions = AuthSketch.Options.GoogleOptions;

namespace AuthSketch.Providers.Google;

public class GoogleProvider : IGoogleProvider
{
    private readonly GoogleOptions _googleOptions;
    private readonly HttpClient _httpClient;

    public GoogleProvider(IConfiguration configuration, IHttpClientFactory httpClientFactory)
    {
        _googleOptions = configuration.GetOptions<GoogleOptions>(nameof(GoogleOptions));
        _httpClient = httpClientFactory.CreateClient();
    }

    public async Task<bool> VerifyAccessTokenAsync(string accessToken)
    {
        var requestUrl = _googleOptions.TokenValidationEndpoint;
        using var request = new HttpRequestMessage(HttpMethod.Post, requestUrl);

        request.Headers.Authorization = new AuthenticationHeaderValue(Constants.BearerAuthHeaderKey, accessToken);

        var response = await _httpClient.SendAsync(request);

        // Invalid tokens will cause the response different from 200
        return response.IsSuccessStatusCode;
    }
}