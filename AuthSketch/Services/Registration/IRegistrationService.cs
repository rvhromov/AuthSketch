using AuthSketch.Models.Registration;

namespace AuthSketch.Services.Registration;

public interface IRegistrationService
{
    Task SignUpAsync(SignUpRequest request);
    Task VerifyIdentityAsync(VerificationRequest request);
    Task<SignInResponse> SignInAsync(SignInRequest request);
    Task SignUpWithExternalProviderAsync(ExternalSignUpRequest request);
    Task<SignInResponse> SignInWithExternalProviderAsync(ExternalSignInRequest request);
}
