using AuthSketch.Extensions;
using AuthSketch.Options;
using System.Net.Http.Headers;
using System.Text;
using System.Text.Json;

namespace AuthSketch.Providers.GitHub;

public sealed class GitHubProvider : IGitHubProvider
{
    private readonly GitHubOptions _gitHubOptions;
    private readonly HttpClient _httpClient;

    public GitHubProvider(IConfiguration configuration, IHttpClientFactory httpClientFactory)
    {
        _gitHubOptions = configuration.GetOptions<GitHubOptions>(nameof(GitHubOptions));
        _httpClient = httpClientFactory.CreateClient();
    }

    public async Task<JsonElement> GetUserAsync(string accessToken)
    {
        using var request = new HttpRequestMessage(HttpMethod.Get, _gitHubOptions.UserInformationEndpoint);

        request.Headers.Authorization = new AuthenticationHeaderValue(Constants.BearerAuthHeaderKey, accessToken);
        request.Headers.UserAgent.TryParseAdd(Constants.UserAgentHeaderValue);

        using var response = await _httpClient.SendAsync(request);

        return await response.Content.ReadFromJsonAsync<JsonElement>();
    }

    public async Task<string> GetUserEmailAsync(string accessToken)
    {
        using var request = new HttpRequestMessage(HttpMethod.Get, _gitHubOptions.UserEmailEndpoint);

        request.Headers.Authorization = new AuthenticationHeaderValue(Constants.BearerAuthHeaderKey, accessToken);
        request.Headers.UserAgent.TryParseAdd(Constants.UserAgentHeaderValue);

        using var response = await _httpClient.SendAsync(request);
        var result = await response.Content.ReadFromJsonAsync<List<EmailResponse>>();

        return result.FirstOrDefault()?.Email;
    }

    private class EmailResponse
    {
        public string Email { get; set; }
    }

    public async Task<bool> VerifyAccessTokenAsync(string accessToken)
    {
        var requestUrl = string.Format(_gitHubOptions.TokenValidationEndpoint, _gitHubOptions.ClientId);
        using var request = new HttpRequestMessage(HttpMethod.Post, requestUrl);

        request.Headers.Accept.TryParseAdd(Constants.GitHubAcceptHeaderValue);
        request.Headers.UserAgent.TryParseAdd(Constants.UserAgentHeaderValue);
        request.Headers.Authorization = CreateBasicAuthorizationHeader();
        request.Content = CreateRequestBody(accessToken);

        var response = await _httpClient.SendAsync(request);

        // Invalid tokens will return 404 NOT FOUND.
        return response.IsSuccessStatusCode;
    }

    private AuthenticationHeaderValue CreateBasicAuthorizationHeader()
    {
        var authorizationHeaderPayload = $"{_gitHubOptions.ClientId}:{_gitHubOptions.ClientSecret}";
        var base64AuthHeaderPayload = Convert.ToBase64String(Encoding.ASCII.GetBytes(authorizationHeaderPayload));

        return new AuthenticationHeaderValue(Constants.BasicAuthHeaderKey, base64AuthHeaderPayload);
    }

    private static StringContent CreateRequestBody(string accessToken)
    {
        var content = $"{{\"{Constants.GitHubTokenKey}\":\"{accessToken}\"}}";

        return new StringContent(content, Encoding.UTF8, Constants.JsonContent);
    }
}
