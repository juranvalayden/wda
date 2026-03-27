using WDA.Application.Services;
using WDA.Shared.Errors;

namespace WDA.Application.Interfaces;

public interface IIdentityService
{
    Task<Response> RegisterUserAsync(ApplicationUser applicationUser);

    Task<Response> GetUserByEmailAsync(string email);

    Task<string?> GetUserNameAsync(string userId);

    Task<bool> IsInRoleAsync(string userId, string role);

    Task<bool> AuthorizeAsync(string userId, string policyName);
}