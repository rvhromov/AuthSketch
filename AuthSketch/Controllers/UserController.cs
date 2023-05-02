using AuthSketch.Models.Users;
using AuthSketch.Services.Users;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace AuthSketch.Controllers;

[ApiController]
[Route("users")]
public sealed class UserController : ControllerBase
{
    private readonly IUserService _userService;

    public UserController(IUserService userService) => 
        _userService = userService;

    // Gets the list of users from the system. Only users with @sketch.com email allowed to access this endpoint
    [HttpGet]
    [Authorize(Constants.DomainEmailPolicy)]
    public async Task<ActionResult<List<GetUsersResponse>>> GetUsersAsync([FromQuery] GetUsersRequest request)
    {
        var (totalCount, users) = await _userService.GetUsersAsync(request);

        HttpContext.Response.Headers.Add("X-Total-Count", totalCount.ToString());
        return Ok(users);
    }

    // Deletes a user. Only users with "Admin" role are allowed to access this endpoint
    [HttpDelete("{id}")]
    [Authorize(Constants.AdminPolicy)]
    public async Task<IActionResult> DeleteUserAsync([FromRoute] int id)
    {
        await _userService.DeleteUserAsync(id);
        return new StatusCodeResult(StatusCodes.Status204NoContent);
    }
}
