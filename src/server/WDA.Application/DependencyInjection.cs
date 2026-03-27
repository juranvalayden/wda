using FluentValidation;
using Microsoft.Extensions.DependencyInjection;
using WDA.Application.Abstractions.Common;
using WDA.Application.Users.Commands.LoginUserCommand;
using WDA.Application.Users.Commands.RegisterUserCommand;
using WDA.Application.Users.Queries.GetUserByEmail;

namespace WDA.Application;

public static class DependencyInjection
{
    public static void AddApplication(this IServiceCollection services)
    {
        services.AddValidatorsFromAssembly(typeof(DependencyInjection).Assembly, includeInternalTypes: true);

        services.AddScoped<IHandler<LoginUserCommand>, LoginUserCommandHandler>();
        services.AddScoped<IHandler<RegisterUserCommand>, RegisterUserCommandHandler>();
        services.AddScoped<IHandler<GetUserByEmailQuery>, GetUserByEmailQueryHandler>();
    }
}