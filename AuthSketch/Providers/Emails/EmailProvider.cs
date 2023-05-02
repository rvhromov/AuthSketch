using AuthSketch.Extensions;
using AuthSketch.Models.Emails;
using AuthSketch.Options;
using FluentEmail.Core;

namespace AuthSketch.Providers.Emails;

public class EmailProvider : IEmailProvider
{
    private readonly IFluentEmail _fluentEmail;
    public readonly AppOptions _appOptions;

    public EmailProvider(IFluentEmail fluentEmail, IConfiguration configuration)
    {
        _fluentEmail = fluentEmail;
        _appOptions = configuration.GetOptions<AppOptions>(nameof(AppOptions));
    }

    public async Task SendVerificationEmailAsync(VerifyEmail verifyEmail)
    {
        var (email, name, token) = verifyEmail;

        // The link should point to the frontend page which then will take care of 
        // preparing the POST request to the verification endpoint
        var verifyLink = $"{_appOptions.Origin}/verification?token={token}";
        var template = string.Format(Constants.VerificationEmailTemplate, name, verifyLink, token);

        await _fluentEmail
            .To(email)
            .Subject("Verify your email")
            .Body(template, isHtml: true)
            .SendAsync();
    }

    public async Task SendResetPasswordEmailAsync(ResetPasswordEmail resetPasswordEmail)
    {
        var (email, name, token) = resetPasswordEmail;

        // The link should point to the frontend page which then will take care of 
        // preparing the POST request to the password-reset endpoint
        var resetLink = $"{_appOptions.Origin}/password-reset?token={token}";
        var template = string.Format(Constants.ResetPasswordEmailTemplate, name, resetLink, token);

        await _fluentEmail
            .To(email)
            .Subject("Reset password")
            .Body(template, isHtml: true)
            .SendAsync();
    }

    public async Task SendPasswordChangedEmailAsync(PasswordChangedEmail passwordChangedEmail)
    {
        var (email, name) = passwordChangedEmail;

        var template = string.Format(Constants.PasswordChangedEmailTemplate, name);

        await _fluentEmail
            .To(email)
            .Subject("Password changed")
            .Body(template, isHtml: true)
            .SendAsync();
    }

    public async Task SendTotpCodeEmailAsync(TotpEmail totpEmail)
    {
        var (email, name, totpCode) = totpEmail;

        var template = string.Format(Constants.TotpEmailTemplate, name, totpCode);

        await _fluentEmail
            .To(email)
            .Subject("Password changed")
            .Body(template, isHtml: true)
            .SendAsync();
    }
}
