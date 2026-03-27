using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using WDA.Application.Interfaces;
using WDA.Application.Services;
using WDA.Shared.Errors;

namespace WDA.Infrastructure.Identity;

public class IdentityService : IIdentityService
{
    private readonly UserManager<ApplicationUser> _userManager;
    private readonly IUserClaimsPrincipalFactory<ApplicationUser> _userClaimsPrincipalFactory;
    private readonly IAuthorizationService _authorizationService;

    public IdentityService(UserManager<ApplicationUser> userManager,
        IUserClaimsPrincipalFactory<ApplicationUser> userClaimsPrincipalFactory,
        IAuthorizationService authorizationService)
    {
        _userManager = userManager ?? throw new ArgumentNullException(nameof(userManager));
        _userClaimsPrincipalFactory = userClaimsPrincipalFactory ?? throw new ArgumentNullException(nameof(userClaimsPrincipalFactory));
        _authorizationService = authorizationService ?? throw new ArgumentNullException(nameof(authorizationService));
    }

    public async Task<Response> RegisterUserAsync(ApplicationUser applicationUser)
    {
        var existingUser = await _userManager.FindByEmailAsync(applicationUser.Email!);

        if (existingUser != null) return UserErrors.AlreadyExists(applicationUser.Email!);

        var result = await _userManager.CreateAsync(applicationUser, applicationUser.Password);

        if (!result.Succeeded)
        {
            return UserErrors.ErrorCreatingUser();
        }

        return Response<string>.Success(applicationUser.Id);
    }

    public async Task<Response> GetUserByEmailAsync(string email)
    {
        var applicationUser = await _userManager.FindByEmailAsync(email);

        if (applicationUser == null)
        {
            return UserErrors.NotFound(email);
        }

        return Response<ApplicationUser>.Success(applicationUser);
    }

    public async Task<string?> GetUserNameAsync(string userId)
    {
        var user = await _userManager.FindByIdAsync(userId);
        return user?.UserName;
    }

    public async Task<bool> IsInRoleAsync(string userId, string role)
    {
        var user = await _userManager.FindByIdAsync(userId);

        return user != null && await _userManager.IsInRoleAsync(user, role);
    }

    public async Task<bool> AuthorizeAsync(string userId, string policyName)
    {
        var user = await _userManager.FindByIdAsync(userId);

        if (user == null) return false;

        var principal = await _userClaimsPrincipalFactory.CreateAsync(user);

        var result = await _authorizationService.AuthorizeAsync(principal, policyName);

        return result.Succeeded;
    }
}
