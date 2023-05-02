using AuthSketch.Enums;
using AuthSketch.Extensions;
using AuthSketch.Models.Registration;
using AuthSketch.Options;
using AuthSketch.Providers.Identity;
using AuthSketch.Services.Registration;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Google;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace AuthSketch.Controllers;

[ApiController]
[Route("google")]
public class GoogleAuthController : ControllerBase
{
    private readonly IIdentityAccessorProvider _identityAccessorProvider;
    private readonly IRegistrationService _registrationService;
    private readonly string _redirectUrl;

    public GoogleAuthController(
        IIdentityAccessorProvider identityAccessorProvider,
        IRegistrationService registrationService,
        IConfiguration configuration)
    {
        _identityAccessorProvider = identityAccessorProvider;
        _registrationService = registrationService;
        _redirectUrl = configuration.GetOptions<AppOptions>(nameof(AppOptions)).RedirectUrl;
    }

    [HttpPost("signin")]
    [Authorize(AuthenticationSchemes = Constants.GoogleCookieAuthScheme)]
    public async Task<IActionResult> SignInAsync(ExternalSignInRequest request)
    {
        request.Email = _identityAccessorProvider.GetUserEmail();
        request.IpAddress = HttpContext.GetIpAddress();
        request.Provider = AuthProvider.Google;

        var response = await _registrationService.SignInWithExternalProviderAsync(request);

        return Ok(response);
    }

    [HttpGet("authenticate")]
    public async Task<IActionResult> AuthenticateAsync()
    {
        var authProp = new AuthenticationProperties { RedirectUri = _redirectUrl };

        return Challenge(authProp, GoogleDefaults.AuthenticationScheme);
    }
}