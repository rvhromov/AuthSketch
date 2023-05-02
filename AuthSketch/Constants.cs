namespace AuthSketch;

public static class Constants
{
    public const string RefreshTokenKey = "refresh-token";
    public const string CustomJwtScheme = "CustomJwt";
    public const string DumbAuthScheme = "DumbAuth";
    public const string GitHubCookieAuthScheme = "GitHubCookie";
    public const string GithubAuthScheme = "GitHub";
    public const string GoogleCookieAuthScheme = "GoogleCookie";

    public const string DomainEmailPolicy = "ShouldHaveDomainEmail";
    public const string AdminPolicy = "ShouldBeAnAdmin";

    public const string BearerTokenRegex = "(?i)Bearer(?-i) (?<token>.*)";
    public const string DumbTokenRegex = "(?i)Dumb(?-i) (?<token>.*)";

    public const string GitHubTokenKey = "access_token";
    public const string GitHubAcceptHeaderValue = "application/vnd.github+json";

    public const string BasicAuthHeaderKey = "basic";
    public const string BearerAuthHeaderKey = "bearer";
    public const string UserAgentHeaderValue = "request";

    public const string JsonContent = "application/json";

    public const string GitHubIdClaimKey = "id";
    public const string GitHubNameClaimKey = "name";
    public const string GitHubEmailClaimKey = "email";
    public const string GitHubEmailScopeKey = "user:email";

    public const string VerificationEmailTemplate = @"
        <p>Hello <strong>{0}</strong>, welcome to our community!</p>
        <p>Please, verify your email by clicking the link below:</p>
        <p><a href='{1}'>Verify email</a></p>
        <p>or if it doesn't work, submit the below token to <strong><code>/verification</code></strong> endpoint:
        <p><code>{2}</code></p>";

    public const string ResetPasswordEmailTemplate = @"
        <p>Hello <strong>{0}</strong>.</p>
        <p>To reset your password use the link below, otherwise ignore this email.</p>
        <p><a href='{1}'>Reset password</a></p>
        <p>or if it doesn't work, submit the below token to <strong><code>/password-reset</code></strong> endpoint:
        <p><code>{2}</code></p>";

    public const string PasswordChangedEmailTemplate = @"
        <p>Hello <strong>{0}</strong>.</p>
        <p>Your password has been changed.</p>";

    public const string TotpEmailTemplate = @"
        <p>Hello <strong>{0}</strong>.</p>
        <p>Your TOTP code: <strong>{1}</strong> .</p>";
}