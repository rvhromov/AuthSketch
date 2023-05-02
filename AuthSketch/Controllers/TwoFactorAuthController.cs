using AuthSketch.Models.Tfa;
using AuthSketch.Services.Tfa;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace AuthSketch.Controllers;

[ApiController]
[Route("tfa")]
[Authorize]
public sealed class TwoFactorAuthController : ControllerBase
{
    private readonly ITfaService _tfaService;

    public TwoFactorAuthController(ITfaService tfaService) =>
        _tfaService = tfaService;

    // Sends TOTP code on user's email
    [HttpPost("email-notification")]
    public async Task<IActionResult> SendCodeOnEmailAsync()
    {
        await _tfaService.SendCodeOnEmailAsync();
        return new StatusCodeResult(StatusCodes.Status202Accepted);
    }

    // Enables tfa authentication, returns authenticator key and QR code url to the client
    [HttpPost]
    public async Task<ActionResult<TfaResponse>> EnableTfaAsync() => await _tfaService.EnableTfaAsync();

    // Disables two factor authentication and erases the authenticator key
    [HttpDelete]
    public async Task<IActionResult> DisableTfaAsync(DisableTfaRequest request)
    {
        await _tfaService.DisableTfaAsync(request);
        return Ok(new { Message = "Two factor authentication is disabled." });
    }
}
