using AuthSketch.Enums;
using AuthSketch.Extensions;
using AuthSketch.Models.Registration;
using AuthSketch.Options;
using AuthSketch.Providers.GitHub;
using AuthSketch.Services.Registration;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.OAuth;
using System.Security.Claims;

namespace AuthSketch.AuthOptions;

public static class GitHubOAuthOptions
{
    /**
     *  By using AspNet.Security.OAuth.GitHub package we could omit some steps and configs,
     *  as it preconfigures basic routes, api calls and mappings under the hood
     *
     *  1. Go to the Authorization endpoint, it will show you the github auth screen, put in your github creds
     *  2. The Authorization endpoint will produce an auth code and send us back to a Callback path
     *  3. The Callback path is just a random path (can be anything). We don't need an actual endpoint for this.
     *     Under the hood, it is needed just to trigger handling of auth code and exchanging it for an access token.
     *  4. OAuth authentication handler gets triggered to exchange the auth code for an access token.
     *     The access token is returned back, however it doesn't automatically puts in a cookie or a claims.
     *     We need to create an additional authentication schema which will take the token and claims and store them.
     *     We can obtain claims from a UserInformationEndpoint with an access token.
    **/

    public static void InitOAuthOptions(OAuthOptions opt, IConfiguration configuration)
    {
        var gitHubOptions = configuration.GetOptions<GitHubOptions>(nameof(GitHubOptions));

        // Github uses cookies
        opt.SignInScheme = Constants.GitHubCookieAuthScheme;
        opt.ClientId = gitHubOptions.ClientId;
        opt.ClientSecret = gitHubOptions.ClientSecret;

        // An endpoint where we obtain an authorization code
        opt.AuthorizationEndpoint = gitHubOptions.AuthorizationEndpoint;
        // An endpoint where we exchange the auth code for an access token
        opt.TokenEndpoint = gitHubOptions.TokenEndpoint;
        // An endpoint where we obtain the github user info with the access token
        opt.UserInformationEndpoint = gitHubOptions.UserInformationEndpoint;
        // Just a random path to trigger auth handler under the hood. 
        opt.CallbackPath = gitHubOptions.CallbackPath;

        // Saves github access token in cookies and makes it available in AuthenticationProperties class
        opt.SaveTokens = gitHubOptions.SaveTokens;
        opt.Scope.Add(Constants.GitHubEmailScopeKey);
    }

    public static void InitOAuthEventActions(OAuthOptions opt)
    {
        // Mapping rules (specific only to "cookie" auth scheme)
        opt.ClaimActions.MapJsonKey(ClaimTypes.NameIdentifier, Constants.GitHubIdClaimKey);
        opt.ClaimActions.MapJsonKey(ClaimTypes.Name, Constants.GitHubNameClaimKey);
        opt.ClaimActions.MapJsonKey(ClaimTypes.Email, Constants.GitHubEmailClaimKey);

        opt.Events.OnCreatingTicket = async ctx =>
        {
            var gitHubProvider = ctx.HttpContext.RequestServices.GetRequiredService<IGitHubProvider>();
            var registrationService = ctx.HttpContext.RequestServices.GetRequiredService<IRegistrationService>();

            var user = await gitHubProvider.GetUserAsync(ctx.AccessToken);
            var userEmail = await gitHubProvider.GetUserEmailAsync(ctx.AccessToken);
            var emailElement = JsonExtensions.JsonElementFromObject(new { email = userEmail });

            // Initialize http context claims
            ctx.RunClaimActions(user);
            ctx.RunClaimActions(emailElement);

            var signUpRequest = new ExternalSignUpRequest
            {
                Email = userEmail,
                Name = user.GetString(Constants.GitHubNameClaimKey),
                Provider = AuthProvider.GitHub,
                AccessToken = ctx.AccessToken
            };

            // Create a new user
            await registrationService.SignUpWithExternalProviderAsync(signUpRequest);
        };
    }
}
