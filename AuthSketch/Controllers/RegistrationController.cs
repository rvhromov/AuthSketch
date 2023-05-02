using AuthSketch.Extensions;
using AuthSketch.Models.Registration;
using AuthSketch.Services.Registration;
using Microsoft.AspNetCore.Mvc;

namespace AuthSketch.Controllers;

[ApiController]
public sealed class RegistrationController : ControllerBase
{
    private readonly IRegistrationService _registrationService;

    public RegistrationController(IRegistrationService registrationService) =>
        _registrationService = registrationService;

    // Registers new user in the system
    [HttpPost("signup")]
    public async Task<IActionResult> SignUpAsync(SignUpRequest request)
    {
        await _registrationService.SignUpAsync(request);
        return Ok(new { Message = "Please, check your email to verify identity." });
    }

    // Verifies user identity and his email address
    [HttpPost("verification")]
    public async Task<IActionResult> VerifyIdentityAsync(VerificationRequest request)
    {
        await _registrationService.VerifyIdentityAsync(request);
        return Ok(new { Message = "Identity has been confirmed successfully." });
    }

    // Logs user in to the system by generating access and refresh tokens
    // If TFA is enabled it is required to pass the valid TOTP code to get tokens
    [HttpPost("signin")]
    public async Task<ActionResult<SignInResponse>> SignInAsync(SignInRequest request)
    {
        request.IpAddress = HttpContext.GetIpAddress();
        var response = await _registrationService.SignInAsync(request);

        HttpContext.SetCookie(Constants.RefreshTokenKey, response.RefreshToken);
        return Ok(response);
    }
}
