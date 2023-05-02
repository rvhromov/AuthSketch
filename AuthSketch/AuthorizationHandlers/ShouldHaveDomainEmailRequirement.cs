using Microsoft.AspNetCore.Authorization;

namespace AuthSketch.AuthorizationHandlers;

public class ShouldHaveDomainEmailRequirement : IAuthorizationRequirement
{
}
