using AuthSketch.Extensions;
using AuthSketch.Models.RefreshTokens;
using AuthSketch.Models.Registration;
using AuthSketch.Services.TokenService;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace AuthSketch.Controllers;

[ApiController]
[Route("refresh-tokens")]
[Authorize]
public sealed class RefreshTokenController : ControllerBase
{
    private readonly ITokenService _tokenService;
    public RefreshTokenController(ITokenService tokenService) => 
        _tokenService = tokenService;

    // Gets list of user's active refresh tokens
    [HttpGet]
    public async Task<ActionResult<List<ActiveRefreshTokenResponse>>> GetRefreshTokensAsync() => 
        Ok(await _tokenService.GetActiveRefreshTokensAsync());

    // Revokes requested user's refresh token
    [HttpPost("revocation")]
    public async Task<IActionResult> RevokeTokenAsync(RevokeRequest request)
    {
        var revokeRequest = new RevokeRequest
        {
            Token = request.Token ?? HttpContext.GetCookie(Constants.RefreshTokenKey),
            IpAddress = HttpContext.GetIpAddress()
        };

        await _tokenService.RevokeRefreshTokenAsync(revokeRequest);
        return Ok(new { Message = "Token has been revoked." });
    }

    // Exchanges refresh token for the new pair of access and refresh tokens
    [HttpPost]
    [AllowAnonymous]
    public async Task<ActionResult<SignInResponse>> RefreshAsync(RefreshRequest request)
    {
        var refreshRequest = new RefreshRequest
        {
            Token = request.Token ?? HttpContext.GetCookie(Constants.RefreshTokenKey),
            IpAddress = HttpContext.GetIpAddress()
        };

        var response = await _tokenService.RefreshAsync(refreshRequest);

        HttpContext.SetCookie(Constants.RefreshTokenKey, response.RefreshToken);
        return Ok(response);
    }
}
