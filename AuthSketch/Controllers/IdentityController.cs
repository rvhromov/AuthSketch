using AuthSketch.Extensions;
using AuthSketch.Models.Identity;
using AuthSketch.Services.Identity;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace AuthSketch.Controllers;

[ApiController]
[Route("identity")]
public sealed class IdentityController : ControllerBase
{
    private readonly IIdentityService _identityService;

    public IdentityController(IIdentityService identityService) => 
        _identityService = identityService;

    // Gets basic info about current user using custom authentication scheme
    [HttpGet("me")]
    [Authorize(AuthenticationSchemes = Constants.CustomJwtScheme)]
    public async Task<ActionResult<MeResponse>> MeAsync() => Ok(await _identityService.GetMeAsync());

    // Changes user's current password
    [HttpPut("password-change")]
    [Authorize]
    public async Task<IActionResult> ChangePasswordAsync(ChangePasswordRequest request)
    {
        request.IpAddress = HttpContext.GetIpAddress();

        await _identityService.ChangePasswordAsync(request);
        return Ok(new { Message = "Password has been changed successfully. " });
    }
}
