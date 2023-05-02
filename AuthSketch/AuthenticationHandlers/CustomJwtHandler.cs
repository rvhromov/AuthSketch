using AuthSketch.AuthenticationHandlers.Options;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.Extensions.Options;
using Microsoft.Net.Http.Headers;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text.Encodings.Web;
using System.Text.RegularExpressions;

namespace AuthSketch.AuthenticationHandlers;

public sealed class CustomJwtHandler : AuthenticationHandler<CustomJwtSchemeOptions>
{
    private readonly JwtBearerOptions _jwtBearerOptions;

    public CustomJwtHandler(
        IOptionsMonitor<CustomJwtSchemeOptions> options,
        ILoggerFactory logger,
        UrlEncoder encoder,
        ISystemClock clock,
        JwtBearerOptions jwtBearerOptions) : base(options, logger, encoder, clock)
    {
        _jwtBearerOptions = jwtBearerOptions;
    }

    protected override async Task<AuthenticateResult> HandleAuthenticateAsync()
    {
        if (!Request.Headers.ContainsKey(HeaderNames.Authorization))
        {
            return AuthenticateResult.Fail($"{HeaderNames.Authorization} header not found.");
        }

        var authHeader = Request.Headers[HeaderNames.Authorization].ToString();
        var tokenMatch = Regex.Match(authHeader, Constants.BearerTokenRegex);

        if (!tokenMatch.Success)
        {
            return AuthenticateResult.Fail("Invalid token format.");
        }

        var token = authHeader.Split(' ').LastOrDefault();

        if (!VerifyToken(token, out var claimsPrincipal))
        {
            return AuthenticateResult.Fail("Invalid token.");
        }

        var ticket = new AuthenticationTicket(claimsPrincipal, Scheme.Name);

        return AuthenticateResult.Success(ticket);
    }

    private bool VerifyToken(string token, out ClaimsPrincipal claimsPrincipal)
    {
        claimsPrincipal = null;

        var tokenHandler = new JwtSecurityTokenHandler();
        var tokenParameters = _jwtBearerOptions.TokenValidationParameters;

        try
        {
            claimsPrincipal = tokenHandler.ValidateToken(token, tokenParameters, out var validatedToken);

            return validatedToken != null;
        }
        catch (Exception)
        {
            return false;
        }
    }
}
