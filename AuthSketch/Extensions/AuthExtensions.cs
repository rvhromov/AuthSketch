using AuthSketch.AuthenticationHandlers;
using AuthSketch.AuthenticationHandlers.Options;
using AuthSketch.AuthOptions;
using AuthSketch.AuthorizationHandlers;
using AuthSketch.Enums;
using AuthSketch.Options;
using Microsoft.AspNetCore.Authentication.Google;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Authorization;
using Microsoft.IdentityModel.Tokens;
using System.Security.Claims;
using System.Text;
using GoogleOptions = AuthSketch.Options.GoogleOptions;

namespace AuthSketch.Extensions;

public static class AuthExtensions
{
    public static IServiceCollection AddAuthentication(this IServiceCollection services, IConfiguration configuration)
    {
        var jwtBearerOptions = GetBearerOptions(configuration);
        services.AddSingleton(jwtBearerOptions);

        var googleOptions = configuration.GetOptions<GoogleOptions>(nameof(GoogleOptions));

        services
            .AddAuthentication(opt =>
            {
                opt.DefaultScheme = JwtBearerDefaults.AuthenticationScheme;
                opt.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
                opt.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
            })
            .AddScheme<CustomJwtSchemeOptions, CustomJwtHandler>(Constants.CustomJwtScheme, _ => { })
            .AddScheme<DumbAuthSchemeOptions, DumbAuthHandler>(Constants.DumbAuthScheme, _ => { })
            .AddJwtBearer(opt =>
            {
                opt.SaveToken = jwtBearerOptions.SaveToken;
                opt.TokenValidationParameters = jwtBearerOptions.TokenValidationParameters;
            })
            .AddCookie(Constants.GitHubCookieAuthScheme)
            // More "raw" way to add 3rd party sign in. We could use a library instead
            .AddOAuth(Constants.GithubAuthScheme, opt =>
            {
                GitHubOAuthOptions.InitOAuthOptions(opt, configuration);
                GitHubOAuthOptions.InitOAuthEventActions(opt);
            })
            .AddCookie(Constants.GoogleCookieAuthScheme, _ => { })
            // Using library dedicated to oauth google sign in
            .AddGoogle(GoogleDefaults.AuthenticationScheme, opt =>
            {
                GoogleOAuthOptions.InitOAuthOptions(opt, configuration);
                GoogleOAuthOptions.InitOAuthEventActions(opt);
            });

        return services;
    }

    public static IServiceCollection AddAuthorization(this IServiceCollection services, IConfiguration configuration)
    {
        var jwtOptions = configuration.GetOptions<JwtOptions>(nameof(JwtOptions));

        services.AddSingleton<IAuthorizationHandler, ShouldHaveDomainEmailHandler>();

        services.AddAuthorization(opt =>
        {
            opt.AddPolicy(Constants.DomainEmailPolicy, policy =>
            {
                policy.AuthenticationSchemes.Add(JwtBearerDefaults.AuthenticationScheme);
                policy.AuthenticationSchemes.Add(Constants.CustomJwtScheme);
                policy.AuthenticationSchemes.Add(Constants.DumbAuthScheme);

                policy.RequireAuthenticatedUser();
                policy.Requirements.Add(new ShouldHaveDomainEmailRequirement());
            });

            opt.AddPolicy(Constants.AdminPolicy, policy =>
            {
                policy.RequireAuthenticatedUser();
                policy.RequireAssertion(context => context.User.HasClaim(c =>
                    c.Type == ClaimTypes.Role && 
                    c.Value == RoleType.Admin.ToString() &&
                    c.Issuer == jwtOptions.Issuer));
            });
        });

        return services;
    }

    private static JwtBearerOptions GetBearerOptions(IConfiguration configuration)
    {
        var jwtOptions = configuration.GetOptions<JwtOptions>(nameof(JwtOptions));
        var signingKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(jwtOptions.Secret));

        return new JwtBearerOptions
        {
            SaveToken = true,
            TokenValidationParameters = new()
            {
                ClockSkew = TimeSpan.Zero,
                IssuerSigningKey = signingKey,
                ValidAudience = jwtOptions.Audience,
                ValidIssuer = jwtOptions.Issuer,
                ValidateIssuerSigningKey = jwtOptions.ValidateIssuer,
                ValidateAudience = jwtOptions.ValidateAudience,
                ValidateIssuer = jwtOptions.ValidateIssuer,
                ValidateLifetime = jwtOptions.ValidateLifetime
            }
        };
    }
}