using AuthSketch.Models.Users;

namespace AuthSketch.Services.Users;

public interface IUserService
{
    Task<(int totalCount, List<GetUsersResponse>)> GetUsersAsync(GetUsersRequest request);
    Task DeleteUserAsync(int id);
}
