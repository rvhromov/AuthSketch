using AuthSketch.Middlewares;
using AuthSketch.Options;
using AuthSketch.Persistence.Contexts;
using AuthSketch.Providers.Emails;
using AuthSketch.Providers.GitHub;
using AuthSketch.Providers.Google;
using AuthSketch.Providers.Identity;
using AuthSketch.Providers.Otp;
using AuthSketch.Services.Identity;
using AuthSketch.Services.Passwords;
using AuthSketch.Services.Registration;
using AuthSketch.Services.Security;
using AuthSketch.Services.Tfa;
using AuthSketch.Services.TokenService;
using AuthSketch.Services.Users;
using Microsoft.EntityFrameworkCore;

namespace AuthSketch.Extensions;

public static class ServiceCollectionExtensions
{
    public static IServiceCollection AddPersistence(this IServiceCollection services, IConfiguration configuration)
    {
        var connectionString = configuration.GetConnectionString("postgres");

        services.AddDbContext<AppDbContext>(x => x.UseNpgsql(connectionString).LogTo(Console.WriteLine));

        return services;
    }

    public static IServiceCollection AddServices(this IServiceCollection services)
    {
        services.AddScoped<ISecurityService, SecurityService>();
        services.AddScoped<ITokenService, TokenService>();
        services.AddScoped<IIdentityService, IdentityService>();
        services.AddScoped<IRegistrationService, RegistrationService>();
        services.AddScoped<IPasswordService, PasswordService>();
        services.AddScoped<IUserService, UserService>();
        services.AddScoped<ITfaService, TfaService>();

        return services;
    }

    public static IServiceCollection AddProviders(this IServiceCollection services)
    {
        services.AddScoped<IEmailProvider, EmailProvider>();
        services.AddScoped<IIdentityAccessorProvider, IdentityAccessorProvider>();
        services.AddScoped<ITotpProvider, TotpProvider>();
        services.AddScoped<IGitHubProvider, GitHubProvider>();
        services.AddScoped<IGoogleProvider, GoogleProvider>();

        return services;
    }

    public static IServiceCollection AddMiddlewares(this IServiceCollection services)
    {
        services.AddScoped<ExceptionMiddleware>();

        return services;
    }

    public static IServiceCollection AddEmails(this IServiceCollection services, IConfiguration configuration)
    {
        var appOptions = configuration.GetOptions<AppOptions>(nameof(AppOptions));

        services
            .AddFluentEmail(appOptions.Email)
            .AddSmtpSender(appOptions.SmtpHost, appOptions.SmtpPort, appOptions.SmtpUsername, appOptions.SmtpPassword);

        return services;
    }
}
