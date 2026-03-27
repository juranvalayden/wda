using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Configuration;
using Microsoft.IdentityModel.Tokens;
using WDA.Application.Interfaces;
using WDA.Application.Services;
using WDA.Shared.Errors;

namespace WDA.Infrastructure.Identity;

public class IdentityService : IIdentityService
{
    private readonly UserManager<ApplicationUser> _userManager;
    private readonly IUserClaimsPrincipalFactory<ApplicationUser> _userClaimsPrincipalFactory;
    private readonly IAuthorizationService _authorizationService;
    private readonly IConfiguration _configuration;

    public IdentityService(UserManager<ApplicationUser> userManager,
        IUserClaimsPrincipalFactory<ApplicationUser> userClaimsPrincipalFactory,
        IAuthorizationService authorizationService,
        IConfiguration configuration)
    {
        _userManager = userManager ?? throw new ArgumentNullException(nameof(userManager));
        _userClaimsPrincipalFactory = userClaimsPrincipalFactory ?? throw new ArgumentNullException(nameof(userClaimsPrincipalFactory));
        _authorizationService = authorizationService ?? throw new ArgumentNullException(nameof(authorizationService));
        _configuration = configuration ?? throw new ArgumentNullException(nameof(configuration));
    }

    public async Task<Response> RegisterUserAsync(ApplicationUser applicationUser)
    {
        var existingUser = await _userManager.FindByEmailAsync(applicationUser.Email!);

        if (existingUser != null) return UserErrors.AlreadyExists(applicationUser.Email!);

        var result = await _userManager.CreateAsync(applicationUser, applicationUser.Password);

        if (!result.Succeeded)
        {
            return UserErrors.ErrorRegisteringUser();
        }

        return Response<string>.Success(applicationUser.Id);
    }

    public async Task<Response> GetUserByIdAsync(string userId)
    {
        var applicationUser = await _userManager.FindByIdAsync(userId);

        if (applicationUser == null)
        {
            return UserErrors.NotFound(userId);
        }

        return Response<ApplicationUser>.Success(applicationUser);
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

    public string? GenerateToken(ApplicationUser applicationUser)
    {
        var claims = new List<Claim>
        {
            new("sub", applicationUser.Id),
            new("given_name", applicationUser.FirstName),
            new("family_name", applicationUser.LastName),
            new("email", applicationUser.Email!)
        };

        var secretForKey = _configuration["Authentication:SecretForKey"]
                           ?? throw new InvalidOperationException("No Authentication:SecretForKey for key found.");

        var issuer = _configuration["Authentication:Issuer"]
                          ?? throw new InvalidOperationException("No Authentication:Issuer found.");

        var audience = _configuration["Authentication:Audience"]
                            ?? throw new InvalidOperationException("No Authentication:Audience found.");

        var symmetricSecurityKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(secretForKey));
        var signingCredentials = new SigningCredentials(symmetricSecurityKey, SecurityAlgorithms.HmacSha256);

        var token = new JwtSecurityToken(issuer: issuer,
            audience: audience,
            claims: claims,
            expires: DateTime.Now.AddHours(1),
            signingCredentials: signingCredentials);

        return new JwtSecurityTokenHandler().WriteToken(token) ?? string.Empty;
    }
}
