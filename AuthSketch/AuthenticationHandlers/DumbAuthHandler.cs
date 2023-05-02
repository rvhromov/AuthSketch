using AuthSketch.AuthenticationHandlers.Options;
using AuthSketch.Models.Dumb;
using Microsoft.AspNetCore.Authentication;
using Microsoft.Extensions.Options;
using Microsoft.Net.Http.Headers;
using System.Security.Claims;
using System.Text;
using System.Text.Encodings.Web;
using System.Text.Json;
using System.Text.RegularExpressions;

namespace AuthSketch.AuthenticationHandlers;

public sealed class DumbAuthHandler : AuthenticationHandler<DumbAuthSchemeOptions>
{
    public DumbAuthHandler(
        IOptionsMonitor<DumbAuthSchemeOptions> options,
        ILoggerFactory logger,
        UrlEncoder encoder,
        ISystemClock clock) : base(options, logger, encoder, clock)
    {
    }

    protected override async Task<AuthenticateResult> HandleAuthenticateAsync()
    {
        if (!Request.Headers.ContainsKey(HeaderNames.Authorization))
        {
            return AuthenticateResult.Fail($"{HeaderNames.Authorization} header not found.");
        }

        var authHeader = Request.Headers[HeaderNames.Authorization].ToString();
        var tokenMatch = Regex.Match(authHeader, Constants.DumbTokenRegex);

        if (!tokenMatch.Success)
        {
            return AuthenticateResult.Fail("Invalid token format.");
        }

        var token = authHeader.Split(' ').LastOrDefault();

        if (!VerifyToken(token, out var dumbClaims))
        {
            return AuthenticateResult.Fail("Invalid token.");
        }

        var claimsPrincipal = CreateClaimsPrincipal(dumbClaims);
        var ticket = new AuthenticationTicket(claimsPrincipal, Scheme.Name);

        return AuthenticateResult.Success(ticket);
    }

    private bool VerifyToken(string token, out DumbClaims dumbClaims)
    {
        dumbClaims = null;

        try
        {
            var dumbClaimsJson = Encoding.UTF8.GetString(Convert.FromBase64String(token));
            dumbClaims = JsonSerializer.Deserialize<DumbClaims>(dumbClaimsJson);

            return dumbClaims != null;
        }
        catch(Exception)
        {
            return false;
        }
    }

    private static ClaimsPrincipal CreateClaimsPrincipal(DumbClaims dumbClaims)
    {
        var claims = new List<Claim>
        {
            new(ClaimTypes.NameIdentifier, dumbClaims.UserId.ToString()),
            new(ClaimTypes.Role, dumbClaims.UserRole.ToString()),
            new(ClaimTypes.Email, dumbClaims.Email)
        };

        var claimsIdentity = new ClaimsIdentity(claims, nameof(DumbAuthHandler));

        return new ClaimsPrincipal(claimsIdentity);
    }
}
