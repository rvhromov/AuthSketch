using AuthSketch.Entities;
using AuthSketch.Enums;
using AuthSketch.Models.RefreshTokens;
using AuthSketch.Models.Registration;

namespace AuthSketch.Services.TokenService;

public interface ITokenService
{
    public string CreateAccessToken(int userId, RoleType userRole, string userEmail);
    public RefreshToken CreateRefreshToken(string ipAddress);
    Task<List<ActiveRefreshTokenResponse>> GetActiveRefreshTokensAsync();
    Task<SignInResponse> RefreshAsync(RefreshRequest request);
    Task RevokeRefreshTokenAsync(RevokeRequest request);
}