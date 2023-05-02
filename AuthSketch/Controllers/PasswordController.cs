using AuthSketch.Extensions;
using AuthSketch.Models.Passwords;
using AuthSketch.Services.Passwords;
using Microsoft.AspNetCore.Mvc;

namespace AuthSketch.Controllers;

[ApiController]
public sealed class PasswordController : ControllerBase
{
    private readonly IPasswordService _passwordService;

    public PasswordController(IPasswordService passwordService) => 
        _passwordService = passwordService;

    // Sends an email with a link to reset a password
    [HttpPost("password-forgot")]
    public async Task<IActionResult> ForgotPasswordAsync(ForgotPasswordRequest request)
    {
        await _passwordService.ForgotPasswordAsync(request);
        return Ok(new { Message = "Recovery message has been sent to the email." });
    }

    // Validates reset token and throws the exception if it's invalid
    [HttpPost("reset-token-validation")]
    public async Task<IActionResult> ValidateResetTokenAsync(ResetTokenValidationRequest request)
    {
        await _passwordService.ValidateResetTokenAsync(request);
        return Ok(new { Message = "Token is valid." });
    }

    // Resets an existing password to the new one from the request
    [HttpPost("password-reset")]
    public async Task<IActionResult> ResetPasswordAsync(ResetPasswordRequest request)
    {
        request.IpAddress = HttpContext.GetIpAddress();

        await _passwordService.ResetPasswordAsync(request);
        return Ok(new { Message = "Password has been reset." });
    }
}
