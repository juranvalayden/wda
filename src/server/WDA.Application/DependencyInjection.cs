using FluentValidation;
using Microsoft.Extensions.DependencyInjection;
using WDA.Application.Abstractions.Common;
using WDA.Application.Interfaces;
using WDA.Application.Services;
using WDA.Application.Users.Commands.CreateUserCommand;
using WDA.Application.Users.Queries.GetUserByEmail;

namespace WDA.Application;

public static class DependencyInjection
{
    public static void AddApplication(this IServiceCollection services)
    {
        services.AddValidatorsFromAssembly(typeof(DependencyInjection).Assembly, includeInternalTypes: true);

        services.AddScoped<IUserService, UserService>();

        services.AddScoped<ICommandHandler<CreateUserCommand>, CreateUserCommandHandler>();
        services.AddScoped<IQueryHandler<GetUserByEmailQuery>, GetUserByEmailQueryHandler>();
    }
}