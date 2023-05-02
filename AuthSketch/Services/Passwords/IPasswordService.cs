using AuthSketch.Models.Passwords;

namespace AuthSketch.Services.Passwords;

public interface IPasswordService
{
    Task ForgotPasswordAsync(ForgotPasswordRequest request);
    Task ValidateResetTokenAsync(ResetTokenValidationRequest request);
    Task ResetPasswordAsync(ResetPasswordRequest request);
}