using WDA.Application.Services;
using WDA.Shared.Errors;

namespace WDA.Application.Interfaces;

public interface IIdentityService
{
    Task<Response> RegisterUserAsync(ApplicationUser applicationUser);

    Task<Response> GetUserByEmailAsync(string email);

    Task<Response> GetUserByIdAsync(string userId);

    string? GenerateToken(ApplicationUser applicationUser);
}