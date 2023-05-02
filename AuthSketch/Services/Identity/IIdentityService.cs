using AuthSketch.Models.Identity;

namespace AuthSketch.Services.Identity;

public interface IIdentityService
{
    Task<MeResponse> GetMeAsync();
    Task ChangePasswordAsync(ChangePasswordRequest request);
}