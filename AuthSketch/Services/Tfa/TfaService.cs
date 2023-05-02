using AuthSketch.Entities;
using AuthSketch.Exceptions;
using AuthSketch.Extensions;
using AuthSketch.Models.Emails;
using AuthSketch.Models.Tfa;
using AuthSketch.Options;
using AuthSketch.Persistence.Contexts;
using AuthSketch.Providers.Emails;
using AuthSketch.Providers.Identity;
using AuthSketch.Providers.Otp;
using Microsoft.EntityFrameworkCore;
using OtpNet;
using System.Security.Cryptography;

namespace AuthSketch.Services.Tfa;

public sealed class TfaService : ITfaService
{
    private readonly AppDbContext _dbContext;
    private readonly IIdentityAccessorProvider _identity;
    private readonly TfaOptions _tfaOptions;
    private readonly ITotpProvider _totpProvider;
    private readonly IEmailProvider _emailProvider;

    public TfaService(
        AppDbContext dbContext,
        IIdentityAccessorProvider identity,
        IConfiguration configuration,
        ITotpProvider totpProvider,
        IEmailProvider emailProvider)
    {
        _dbContext = dbContext;
        _identity = identity;
        _tfaOptions = configuration.GetOptions<TfaOptions>(nameof(TfaOptions));
        _totpProvider = totpProvider;
        _emailProvider = emailProvider;
    }

    public async Task SendCodeOnEmailAsync()
    {
        var user = await GetCurrentUserAsync();

        if (!user.IsTfaEnabled)
        {
            return;
        }

        var totpCode = _totpProvider.GetTotpCode(user.TfaKey);

        await _emailProvider.SendTotpCodeEmailAsync(new TotpEmail(user.Email, user.Name, totpCode));
    }

    public async Task<TfaResponse> EnableTfaAsync()
    {
        var user = await GetCurrentUserAsync();

        if (user.IsTfaEnabled)
        {
            throw new ValidationException("Two factor authentication already enabled.");
        }

        var keyBytes = RandomNumberGenerator.GetBytes(_tfaOptions.AuthenticatorKeySize);
        var authenticatorKey = Base32Encoding.ToString(keyBytes);
        var formattedKey = new OtpUri(OtpType.Totp, authenticatorKey, user.Email, _tfaOptions.Issuer).ToString();

        user.EnableTfa(authenticatorKey);

        _dbContext.Update(user);
        await _dbContext.SaveChangesAsync();

        return new TfaResponse
        {
            AuthenticatorKey = authenticatorKey,
            FormattedKey = formattedKey
        };
    }

    public async Task DisableTfaAsync(DisableTfaRequest request)
    {
        var user = await GetCurrentUserAsync();

        if (!user.IsTfaEnabled)
        {
            return;
        }

        if (!_totpProvider.VerifyTotp(user.TfaKey, request.TotpCode))
        {
            throw new ValidationException("Invalid TOTP code.");
        }

        user.DisableTfa();

        _dbContext.Update(user);
        await _dbContext.SaveChangesAsync();
    }

    private async Task<User> GetCurrentUserAsync()
    {
        var currentUserId = _identity.GetUserId();

        return await _dbContext.Users
            .FirstOrDefaultAsync(u => u.Id == currentUserId)
            ?? throw new NotFoundException("User not found.");
    }
}
