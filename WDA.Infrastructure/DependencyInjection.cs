using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using WDA.Domain.Common;
using WDA.Domain.Entities;
using WDA.Infrastructure.Persistence;
using WDA.Infrastructure.Repositories;

namespace WDA.Infrastructure;

public static class DependencyInjection
{
    public static void AddInfrastructure(this IServiceCollection services, IConfiguration configuration)
    {
        var connectionString = configuration.GetConnectionString("DefaultConnection") 
                               ?? throw new InvalidOperationException("No connection string can be found.");

        services.AddDbContext<WdaDbContext>(options =>
        {
            options.UseNpgsql(connectionString, builder =>
            {
                builder.MigrationsAssembly(typeof(WdaDbContext).Assembly.FullName);
            });
        });

        services.AddTransient<IRepository<User>, UserRepository>();
        services.AddTransient<IUnitOfWork, UnitOfWork>();
    }
}