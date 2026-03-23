using FluentValidation;
using Microsoft.Extensions.DependencyInjection;
using WDA.Application.Interfaces;
using WDA.Application.Services;

namespace WDA.Application;

public static class DependencyInjection
{
    public static void AddApplication(this IServiceCollection services)
    {
        services.AddValidatorsFromAssembly(typeof(DependencyInjection).Assembly, includeInternalTypes: true);
        services.AddScoped<IUserService, UserService>();
    }
}