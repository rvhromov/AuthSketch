using AuthSketch.Models.Emails;

namespace AuthSketch.Providers.Emails;

public interface IEmailProvider
{
    public Task SendVerificationEmailAsync(VerifyEmail verifyEmail);
    public Task SendResetPasswordEmailAsync(ResetPasswordEmail resetPasswordEmail);
    public Task SendPasswordChangedEmailAsync(PasswordChangedEmail passwordChangedEmail);
    public Task SendTotpCodeEmailAsync(TotpEmail totpEmail);
}