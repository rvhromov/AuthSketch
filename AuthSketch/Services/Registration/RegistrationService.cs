using AuthSketch.Entities;
using AuthSketch.Enums;
using AuthSketch.Exceptions;
using AuthSketch.Models.Emails;
using AuthSketch.Models.Registration;
using AuthSketch.Persistence.Contexts;
using AuthSketch.Providers.Emails;
using AuthSketch.Providers.GitHub;
using AuthSketch.Providers.Google;
using AuthSketch.Providers.Otp;
using AuthSketch.Services.Security;
using AuthSketch.Services.TokenService;
using Microsoft.EntityFrameworkCore;

namespace AuthSketch.Services.Registration;

public sealed class RegistrationService : IRegistrationService
{
    private readonly AppDbContext _dbContext;
    private readonly ISecurityService _securityService;
    private readonly IEmailProvider _emailProvider;
    private readonly ITokenService _tokenService;
    private readonly ITotpProvider _totpProvider;
    private readonly IGitHubProvider _gitHubProvider;
    private readonly IGoogleProvider _googleProvider;

    public RegistrationService(
        AppDbContext dbContext,
        ISecurityService securityService,
        IEmailProvider emailProvider,
        ITokenService tokenService,
        ITotpProvider totpProvider,
        IGitHubProvider gitHubProvider,
        IGoogleProvider googleProvider)
    {
        _dbContext = dbContext;
        _securityService = securityService;
        _emailProvider = emailProvider;
        _tokenService = tokenService;
        _totpProvider = totpProvider;
        _gitHubProvider = gitHubProvider;
        _googleProvider = googleProvider;
    }

    public async Task SignUpAsync(SignUpRequest request)
    {
        var (name, email, password, passwordConfirmation) = request;

        var user = await _dbContext.Users.FirstOrDefaultAsync(u => u.Email == email);
        var usesExternalProvider = user.PasswordHash is null;

        // User exists and sign up with email
        if (user is not null && !usesExternalProvider)
        {
            throw new ValidationException("User with the same email already exists.");
        }

        if (!_securityService.PasswordMatch(password, passwordConfirmation))
        {
            throw new ValidationException("Passwords don't match.");
        }

        var passwordHash = _securityService.HashPassword(password);
        var verificationToken = _securityService.GenerateRandomKey();

        // User exists and signed up with external provider
        if (user is not null && usesExternalProvider)
        {
            user.ResetPassword(passwordHash.Password, passwordHash.Salt);

            _dbContext.Users.Update(user);
            await _dbContext.SaveChangesAsync();

            return;
        }

        // User doesn't exist and it's first sign up
        var newUser = User.Create(name, email, passwordHash.Password, passwordHash.Salt, verificationToken);

        _dbContext.Users.Add(newUser);
        await _dbContext.SaveChangesAsync();

        var verifyEmailModel = new VerifyEmail(newUser.Email, newUser.Name, verificationToken);
        await _emailProvider.SendVerificationEmailAsync(verifyEmailModel);
    }

    public async Task<SignInResponse> SignInAsync(SignInRequest request)
    {
        var user = await _dbContext.Users.FirstOrDefaultAsync(u => u.Email == request.Email)
            ?? throw new NotFoundException("User not found.");

        if (!user.IsVerified)
        {
            throw new UnauthorizedException("Verify your account.");
        }

        if (user.IsTfaEnabled && !_totpProvider.VerifyTotp(user.TfaKey, request.TotpCode))
        {
            throw new UnauthorizedException("Invalid TOTP code. Provide valid TFA code to sign in.");
        }

        if (!_securityService.VerifyPassword(request.Password, user.PasswordHash, user.Salt))
        {
            throw new UnauthorizedException("Invalid password.");
        }

        return await CreateTokensAsync(user, request.IpAddress);
    }

    public async Task VerifyIdentityAsync(VerificationRequest request)
    {
        var user = await _dbContext.Users.FirstOrDefaultAsync(u => u.VerificationToken == request.Token)
            ?? throw new ValidationException("Identity verification failed.");

        user.Verify();

        _dbContext.Update(user);
        await _dbContext.SaveChangesAsync();
    }

    public async Task SignUpWithExternalProviderAsync(ExternalSignUpRequest request)
    {
        var user = await _dbContext.Users
            .Include(u => u.ExternalAuthProviders)
            .FirstOrDefaultAsync(u => u.Email == request.Email);

        // Completely new user
        if (user is null)
        {
            var newUser = User.Create(request.Name, request.Email, request.Provider, request.AccessToken);
            newUser.Verify();

            _dbContext.Users.Add(newUser);
            await _dbContext.SaveChangesAsync();

            return;
        }

        // User exists, but signs in with a new provider
        if (!user.ExternalAuthProviders.Any(p => p.Provider == request.Provider))
        {
            user.AddAuthProvider(request.Provider, request.AccessToken);

            _dbContext.Users.Update(user);
            await _dbContext.SaveChangesAsync();

            return;
        }

        // User exists, signs in with existing provider
        user.ResetAuthProviderAccessToken(request.Provider, request.AccessToken);

        _dbContext.Users.Update(user);
        await _dbContext.SaveChangesAsync();
    }

    public async Task<SignInResponse> SignInWithExternalProviderAsync(ExternalSignInRequest request)
    {
        if (string.IsNullOrWhiteSpace(request.Email))
        {
            throw new UnauthorizedException($"{request.Provider} cookies expired. Authenticate with {request.Provider} again.");
        }

        var user = await _dbContext.Users
            .Include(u => u.ExternalAuthProviders)
            .FirstOrDefaultAsync(u => u.Email == request.Email)
            ?? throw new NotFoundException("User not found.");

        var provider = user.ExternalAuthProviders.FirstOrDefault(p => p.Provider == request.Provider) 
            ?? throw new NotFoundException("Provider not found.");

        // Consider other way of token validation if using in high load env due to many requests might be throttled by providers
        var isTokenValid = request.Provider switch
        {
            AuthProvider.GitHub => await _gitHubProvider.VerifyAccessTokenAsync(provider.AccessToken),
            AuthProvider.Google => await _googleProvider.VerifyAccessTokenAsync(provider.AccessToken),
            _ => false
        };

        if (!isTokenValid)
        {
            user.ResetAuthProviderAccessToken(request.Provider, null);

            _dbContext.Users.Add(user);
            await _dbContext.SaveChangesAsync();

            throw new UnauthorizedException($"Invalid {request.Provider} token. Consider to re-login into your {request.Provider} account.");
        }

        if (user.IsTfaEnabled && !_totpProvider.VerifyTotp(user.TfaKey, request.TotpCode))
        {
            throw new UnauthorizedException("Invalid TOTP code. Provide valid TFA code to sign in.");
        }

        return await CreateTokensAsync(user, request.IpAddress);
    }

    private async Task<SignInResponse> CreateTokensAsync(User user, string ipAddress)
    {
        var accessToken = _tokenService.CreateAccessToken(user.Id, user.Role, user.Email);
        var refreshToken = _tokenService.CreateRefreshToken(ipAddress);

        user.AddRefreshToken(refreshToken);

        _dbContext.Users.Update(user);
        await _dbContext.SaveChangesAsync();

        return new SignInResponse
        {
            AccessToken = accessToken,
            RefreshToken = refreshToken.Token
        };
    }
}