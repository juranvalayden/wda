using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Logging;
using WDA.Application.Services;
using WDA.Shared;

namespace WDA.Infrastructure.Persistence;

public class ApplicationDbContextInitialiser
{
    private readonly ILogger<ApplicationDbContextInitialiser> _logger;
    private readonly WdaDbContext _context;
    private readonly UserManager<ApplicationUser> _userManager;
    private readonly RoleManager<IdentityRole> _roleManager;

    public ApplicationDbContextInitialiser(ILogger<ApplicationDbContextInitialiser> logger, WdaDbContext context, UserManager<ApplicationUser> userManager, RoleManager<IdentityRole> roleManager)
    {
        _logger = logger ?? throw new ArgumentNullException(nameof(logger));
        _context = context ?? throw new ArgumentNullException(nameof(context));
        _userManager = userManager ?? throw new ArgumentNullException(nameof(userManager));
        _roleManager = roleManager ?? throw new ArgumentNullException(nameof(roleManager));
    }

    public async Task InitialiseAsync()
    {
        try
        {
            await _context.Database.EnsureDeletedAsync();
            await _context.Database.EnsureCreatedAsync();
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "An error occurred while initialising the database.");
            throw;
        }
    }

    public async Task SeedAsync()
    {
        try
        {
            await TrySeedAsync();
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "An error occurred while seeding the database.");
            throw;
        }
    }

    private async Task TrySeedAsync()
    {
        // Default roles
        var administratorRole = new IdentityRole(Constants.AdministratorRole);

        if (_roleManager.Roles.All(r => r.Name != administratorRole.Name))
        {
            await _roleManager.CreateAsync(administratorRole);
        }

        // Default users
        var administrator = new ApplicationUser(Constants.TestFirstName, 
            Constants.TestLastName,
            Constants.TestUsername, 
            Constants.TestUsername, 
            Constants.TestPassword);

        if (_userManager.Users.All(u => u.UserName != administrator.UserName))
        {
            await _userManager.CreateAsync(administrator, Constants.TestPassword);
            if (!string.IsNullOrWhiteSpace(administratorRole.Name))
            {
                await _userManager.AddToRolesAsync(administrator, [administratorRole.Name]);
            }
        }

        if (!_context.Users.Any())
        {
            _context.Users.Add(administrator);
            
            var hasSaved = await _context.SaveChangesAsync() > 0;

            if (hasSaved)
            {
                _logger.LogInformation("Saved Test User.");
            }
            else
            {
                _logger.LogError("Test user not saved.");
            }
        }
    }
}
