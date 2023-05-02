using AuthSketch.Enums;
using AuthSketch.Extensions;
using AuthSketch.Models.Registration;
using AuthSketch.Options;
using AuthSketch.Services.Registration;
using Microsoft.AspNetCore.Authentication.OAuth;
using System.Security.Claims;

namespace AuthSketch.AuthOptions;

public static class GoogleOAuthOptions
{
    public static void InitOAuthOptions(OAuthOptions opt, IConfiguration configuration)
    {
        var googleOptions = configuration.GetOptions<GoogleOptions>(nameof(GoogleOptions));

        opt.SignInScheme = Constants.GoogleCookieAuthScheme;
        opt.ClientId = googleOptions.ClientId;
        opt.ClientSecret = googleOptions.ClientSecret;
        opt.SaveTokens = true;
    }

    public static void InitOAuthEventActions(OAuthOptions opt)
    {
        opt.Events.OnCreatingTicket = async ctx =>
        {
            var registrationService = ctx.HttpContext.RequestServices.GetRequiredService<IRegistrationService>();

            var firstName = ctx.Identity.FindFirst(ClaimTypes.GivenName).Value;
            var lastName = ctx.Identity.FindFirst(ClaimTypes.Surname)?.Value;
            var email = ctx.Identity.FindFirst(ClaimTypes.Email).Value;

            var signUpRequest = new ExternalSignUpRequest
            {
                Email = email,
                Name = $"{firstName} {lastName}",
                Provider = AuthProvider.Google,
                AccessToken = ctx.AccessToken
            };

            await registrationService.SignUpWithExternalProviderAsync(signUpRequest);
        };
    }
}
