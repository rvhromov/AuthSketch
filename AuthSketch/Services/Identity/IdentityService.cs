using AuthSketch.Exceptions;
using AuthSketch.Models.Emails;
using AuthSketch.Models.Identity;
using AuthSketch.Persistence.Contexts;
using AuthSketch.Providers.Emails;
using AuthSketch.Providers.Identity;
using AuthSketch.Services.Security;
using AutoMapper;
using AutoMapper.QueryableExtensions;
using Microsoft.EntityFrameworkCore;

namespace AuthSketch.Services.Identity;

public class IdentityService : IIdentityService
{
    private readonly AppDbContext _dbContext;
    private readonly IMapper _mapper;
    private readonly IIdentityAccessorProvider _identityAccessor;
    private readonly ISecurityService _securityService;
    private readonly IEmailProvider _emailService;

    public IdentityService(
        AppDbContext dbContext,
        IMapper mapper,
        IIdentityAccessorProvider identityAccessor,
        ISecurityService securityService,
        IEmailProvider emailProvider)
    {
        _dbContext = dbContext;
        _mapper = mapper;
        _identityAccessor = identityAccessor;
        _securityService = securityService;
        _emailService = emailProvider;
    }

    public async Task<MeResponse> GetMeAsync()
    {
        var userId = _identityAccessor.GetUserId();

        return await _dbContext.Users
            .ProjectTo<MeResponse>(_mapper.ConfigurationProvider)
            .FirstOrDefaultAsync(u => u.Id == userId)
            ?? throw new NotFoundException("User not found.");
    }

    public async Task ChangePasswordAsync(ChangePasswordRequest request)
    {
        var userId = _identityAccessor.GetUserId();
        var (currentPassword, newPassword, newPasswordConfirmation) = request;

        var user = await _dbContext.Users
            .Include(u => u.RefreshTokens)
            .FirstOrDefaultAsync(u => u.Id == userId) 
            ?? throw new NotFoundException("User not found.");

        if (!_securityService.VerifyPassword(currentPassword, user.PasswordHash, user.Salt))
        {
            throw new ValidationException("Invalid password.");
        }

        if (!_securityService.PasswordMatch(newPassword, newPasswordConfirmation))
        {
            throw new ValidationException("Passwords don't match.");
        }

        var passwordHash = _securityService.HashPassword(newPassword);

        foreach (var refreshToken in user.RefreshTokens.Where(rt => !rt.IsRevoked && !rt.IsExpired))
        {
            refreshToken.Revoke(request.IpAddress);
        }

        user.UpdatePassword(passwordHash.Password, passwordHash.Salt);

        _dbContext.Users.Update(user);
        await _dbContext.SaveChangesAsync();

        await _emailService.SendPasswordChangedEmailAsync(new PasswordChangedEmail(user.Email, user.Name));
    }
}