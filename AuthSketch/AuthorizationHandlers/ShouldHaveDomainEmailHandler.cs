using Microsoft.AspNetCore.Authorization;
using System.Security.Claims;

namespace AuthSketch.AuthorizationHandlers;

public sealed class ShouldHaveDomainEmailHandler : AuthorizationHandler<IAuthorizationRequirement>
{
    protected override Task HandleRequirementAsync(AuthorizationHandlerContext context, IAuthorizationRequirement requirement)
    {
        var emailClaim = context.User.Claims.FirstOrDefault(x => x.Type == ClaimTypes.Email);

        if (emailClaim is null)
        {
            return Task.CompletedTask;
        }

        if (emailClaim.Value.EndsWith("@sketch.com"))
        {
            context.Succeed(requirement);
        }

        return Task.CompletedTask;
    }
}