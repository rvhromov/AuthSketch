using AuthSketch.Entities;
using AuthSketch.Enums;
using AuthSketch.Exceptions;
using AuthSketch.Extensions;
using AuthSketch.Models.RefreshTokens;
using AuthSketch.Models.Registration;
using AuthSketch.Options;
using AuthSketch.Persistence.Contexts;
using AuthSketch.Providers.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Security.Cryptography;
using System.Text;

namespace AuthSketch.Services.TokenService;

public class TokenService : ITokenService
{
    private readonly JwtOptions _jwtOptions;
    private readonly RefreshTokenOptions _refreshOptions;
    private readonly AppDbContext _dbContext;
    private readonly IIdentityAccessorProvider _identityAccessor;

    public TokenService(
        IConfiguration configuration, 
        AppDbContext dbContext,
        IIdentityAccessorProvider identityAccessor)
    {
        _jwtOptions = configuration.GetOptions<JwtOptions>(nameof(JwtOptions));
        _refreshOptions = configuration.GetOptions<RefreshTokenOptions>(nameof(RefreshTokenOptions));
        _dbContext = dbContext;
        _identityAccessor = identityAccessor;
    }

    public string CreateAccessToken(int userId, RoleType userRole, string userEmail)
    {
        var secretKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_jwtOptions.Secret));
        var signInCredentials = new SigningCredentials(secretKey, SecurityAlgorithms.HmacSha256);

        var claims = new List<Claim>
        {
            new(ClaimTypes.NameIdentifier, userId.ToString()),
            new(ClaimTypes.Role, userRole.ToString()),
            new(ClaimTypes.Email, userEmail)
        };

        var expires = DateTime.UtcNow.AddMinutes(_jwtOptions.ExpiresInMinutes);

        var tokenOptions = new JwtSecurityToken(_jwtOptions.Issuer, _jwtOptions.Audience, claims, default, expires, signInCredentials);
        var accessToken = new JwtSecurityTokenHandler().WriteToken(tokenOptions);

        return accessToken;
    }

    public RefreshToken CreateRefreshToken(string ipAddress)
    {
        var token = Convert.ToHexString(RandomNumberGenerator.GetBytes(_refreshOptions.Size));
        var expiresAt = DateTime.UtcNow.AddHours(_refreshOptions.ExpiresInHours);

        return RefreshToken.Create(token, ipAddress, expiresAt);
    }

    public async Task<List<ActiveRefreshTokenResponse>> GetActiveRefreshTokensAsync()
    {
        var userId = _identityAccessor.GetUserId();

        return await _dbContext.RefreshTokens
            .AsNoTracking()
            .Where(rt => rt.UserId == userId && rt.ExpiresAt > DateTime.UtcNow && !rt.RevokedAt.HasValue)
            .Select(rt => new ActiveRefreshTokenResponse
            {
                // Gets only the n characters from the end of the token to prevent exposing the whole body
                // The tail then is also may be used to revoke a token 
                TokenTail = rt.Token.Substring(rt.Token.Length - _refreshOptions.TailSize),
                CreatedAt = rt.CreatedAt,
                CreatedByIp = rt.CreatedByIp
            })
            .ToListAsync();
    }

    public async Task<SignInResponse> RefreshAsync(RefreshRequest request)
    {
        var user = await _dbContext.Users
            .Include(u => u.RefreshTokens)
            .FirstOrDefaultAsync(u => u.RefreshTokens.Any(rt => rt.Token == request.Token))
            ?? throw new ValidationException("Invalid refresh token.");

        var currentRefreshToken = user.RefreshTokens.First(rt => rt.Token == request.Token);

        if (currentRefreshToken.IsRevoked || currentRefreshToken.IsExpired)
        {
            // Revoke whole chain of tokens in case if current token has leaked
            if (currentRefreshToken.IsRevoked)
                await RevokeTokensChainAsync(currentRefreshToken, user, request.IpAddress);

            throw new ValidationException("Invalid refresh token.");
        }

        var newAccessToken = CreateAccessToken(user.Id, user.Role, user.Email);
        var newRefreshToken = CreateRefreshToken(request.IpAddress);

        user.AddRefreshToken(newRefreshToken);
        currentRefreshToken.Revoke(request.IpAddress, newRefreshToken.Token);

        _dbContext.Users.Update(user);
        await _dbContext.SaveChangesAsync();

        return new SignInResponse
        {
            AccessToken = newAccessToken,
            RefreshToken = newRefreshToken.Token
        };
    }

    private async Task RevokeTokensChainAsync(RefreshToken currentRefreshToken, User user, string ipAddress)
    {
        RevokeNextToken(currentRefreshToken, user.RefreshTokens, ipAddress);

        _dbContext.Users.Update(user);
        await _dbContext.SaveChangesAsync();
    }

    private void RevokeNextToken(RefreshToken token, ICollection<RefreshToken> tokensChain, string ipAddress)
    {
        if (token.ReplacedByToken is not null)
        {
            var nextToken = tokensChain.FirstOrDefault(rt => rt.Token == token.ReplacedByToken);

            if (nextToken is not null && !nextToken.IsRevoked && !nextToken.IsExpired)
            {
                nextToken.Revoke(ipAddress);
            }

            RevokeNextToken(nextToken, tokensChain, ipAddress);
        }
    }

    public async Task RevokeRefreshTokenAsync(RevokeRequest request)
    {
        var userId = _identityAccessor.GetUserId();

        var user = await _dbContext.Users
            .Include(u => u.RefreshTokens)
            .FirstOrDefaultAsync(u => u.Id == userId)
            ?? throw new ValidationException("Invalid refresh token.");

        // We use EndsWith in case if client passed only the tail of token
        var token = user.RefreshTokens.FirstOrDefault(rt => rt.Token.EndsWith(request.Token));

        if (token?.Token is null || token.IsExpired || token.IsRevoked)
        {
            throw new ValidationException("Invalid refresh token.");
        }

        token.Revoke(request.IpAddress);

        _dbContext.Update(token);
        await _dbContext.SaveChangesAsync();
    }
}
