using AuthSketch.Entities;
using AuthSketch.Exceptions;
using AuthSketch.Extensions;
using AuthSketch.Models.Emails;
using AuthSketch.Models.Passwords;
using AuthSketch.Options;
using AuthSketch.Persistence.Contexts;
using AuthSketch.Providers.Emails;
using AuthSketch.Services.Security;
using Microsoft.EntityFrameworkCore;

namespace AuthSketch.Services.Passwords;

public class PasswordService : IPasswordService
{
    private readonly AppDbContext _dbContext;
    private readonly ISecurityService _securityService;
    private readonly IEmailProvider _emailProvider;
    private readonly ResetTokenOptions _resetOptions;

    public PasswordService(
        AppDbContext dbContext,
        ISecurityService securityService,
        IEmailProvider emailProvider,
        IConfiguration configuration)
    {
        _dbContext = dbContext;
        _securityService = securityService;
        _emailProvider = emailProvider;
        _resetOptions = configuration.GetOptions<ResetTokenOptions>(nameof(ResetTokenOptions));
    }

    public async Task ForgotPasswordAsync(ForgotPasswordRequest request)
    {
        var user = await _dbContext.Users.FirstOrDefaultAsync(u => u.Email == request.Email)
            ?? throw new NotFoundException("User not found.");

        var resetToken = _securityService.GenerateRandomKey();
        var resetExpiresAt = DateTime.UtcNow.AddHours(_resetOptions.ExpiresInHours);

        user.SetResetToken(resetToken, resetExpiresAt);

        _dbContext.Update(user);
        await _dbContext.SaveChangesAsync();

        var resetPasswordEmail = new ResetPasswordEmail(user.Email, user.Name, resetToken);
        await _emailProvider.SendResetPasswordEmailAsync(resetPasswordEmail);
    }

    public async Task ValidateResetTokenAsync(ResetTokenValidationRequest request)
    {
        await GetUserByResetTokenAsync(request.Token);
    }

    public async Task ResetPasswordAsync(ResetPasswordRequest request)
    {
        var (resetToken, password, passwordConfirmation) = request;

        var user = await GetUserByResetTokenAsync(resetToken);

        if (!_securityService.PasswordMatch(password, passwordConfirmation))
        {
            throw new ValidationException("Passwords don't match.");
        }

        var passwordHash = _securityService.HashPassword(password);

        user.ResetPassword(passwordHash.Password, passwordHash.Salt);

        foreach (var refreshToken in user.RefreshTokens.Where(rt => !rt.IsRevoked && !rt.IsExpired))
        {
            refreshToken.Revoke(request.IpAddress);
        }

        _dbContext.Update(user);
        await _dbContext.SaveChangesAsync();
    }

    private async Task<User> GetUserByResetTokenAsync(string resetToken)
    {
        var user = await _dbContext.Users.FirstOrDefaultAsync(u => u.ResetToken == resetToken && u.ResetTokenExpiresAt > DateTime.UtcNow);

        if (user is null)
        {
            throw new ValidationException("Invalid reset token.");
        }

        return user;
    }  
}