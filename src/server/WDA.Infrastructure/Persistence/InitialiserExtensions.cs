using Microsoft.AspNetCore.Builder;
using Microsoft.Extensions.DependencyInjection;

namespace WDA.Infrastructure.Persistence;

public static class InitialiserExtensions
{
    public static async Task InitialiseDatabaseAsync(this WebApplication app)
    {
        using var scope = app.Services.CreateScope();
        var provider = scope.ServiceProvider;
        var applicationInitializerService = provider.GetRequiredService<ApplicationDbContextInitialiser>();

        await applicationInitializerService.InitialiseAsync();
        await applicationInitializerService.SeedAsync();
    }
}