using AuthSketch.Exceptions;
using AuthSketch.Models.Dumb;
using AuthSketch.Persistence.Contexts;
using AuthSketch.Services.Security;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Text.Json;
using System.Text;
using Microsoft.AspNetCore.Authorization;
using AuthSketch.Services.Identity;
using AuthSketch.Models.Identity;

namespace AuthSketch.Controllers;

[ApiController]
[Route("dumb")]
public sealed class DumbController : ControllerBase
{
    private readonly AppDbContext _dbContext;
    private readonly ISecurityService _securityService;
    private readonly IIdentityService _identityService;

    public DumbController(
        AppDbContext dbContext,
        ISecurityService securityService,
        IIdentityService identityService)
    {
        _dbContext = dbContext;
        _securityService = securityService;
        _identityService = identityService;
    }

    // Imitates real sing in by generating simple base64 string access token with user info required for claims
    [HttpPost("signin")]
    public async Task<IActionResult> SignInAsync(DumbSignInRequest request)
    {
        var user = await _dbContext.Users
            .AsNoTracking()
            .FirstOrDefaultAsync(u => u.Email == request.Email)
            ?? throw new NotFoundException("User not found.");

        if (!_securityService.VerifyPassword(request.Password, user.PasswordHash, user.Salt))
            throw new ValidationException("Invalid password.");

        var claims = JsonSerializer.Serialize(new DumbClaims { UserId = user.Id, UserRole = user.Role, Email = user.Email });
        var accessToken = Convert.ToBase64String(Encoding.UTF8.GetBytes(claims));

        return Ok(new DumbSignInResponse { AccessToken = accessToken });
    }

    // Gets basic info about current user using dumb authentication scheme
    [HttpGet("me")]
    [Authorize(AuthenticationSchemes = Constants.DumbAuthScheme)]
    public async Task<ActionResult<MeResponse>> MeAsync() => Ok(await _identityService.GetMeAsync());
}