using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.IdentityModel.Tokens;
using System.Text;
using WDA.Application.Interfaces;
using WDA.Application.Services;
using WDA.Infrastructure.Identity;
using WDA.Infrastructure.Persistence;

namespace WDA.Infrastructure;

public static class DependencyInjection
{
    public static void AddInfrastructure(this IHostApplicationBuilder builder)
    {
        var configuration = builder.Configuration;

        var connectionString = configuration.GetConnectionString("DefaultConnection")
                               ?? throw new InvalidOperationException("No connection string can be found.");

        builder.Services.AddDbContext<WdaDbContext>(options =>
        {
            options.UseNpgsql(connectionString, optionsBuilder =>
            {
                optionsBuilder.MigrationsAssembly(typeof(WdaDbContext).Assembly.FullName);
            });
        });

        builder.Services.AddScoped<IWdaDbContext>(provider => provider.GetRequiredService<WdaDbContext>());

        builder.Services.AddScoped<ApplicationDbContextInitialiser>();

        builder.Services
            .AddIdentityCore<ApplicationUser>()
            .AddRoles<IdentityRole>()
            .AddEntityFrameworkStores<WdaDbContext>()
            .AddSignInManager()
            .AddDefaultTokenProviders()
            .AddApiEndpoints();

        builder.Services.AddTransient<IIdentityService, IdentityService>();

        var secretForKey = configuration["Authentication:SecretForKey"]
                           ?? throw new InvalidOperationException("No Authentication:SecretForKey for key found.");

        var issuer = configuration["Authentication:Issuer"]
                     ?? throw new InvalidOperationException("No Authentication:Issuer found.");

        var audience = configuration["Authentication:Audience"]
                       ?? throw new InvalidOperationException("No Authentication:Audience found.");

        var encodedSecretForKey = Encoding.UTF8.GetBytes(secretForKey);
        var symmetricSecurityKey = new SymmetricSecurityKey(encodedSecretForKey);

        builder.Services.AddAuthentication(options =>
            {
                options.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
                options.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
            })
            .AddJwtBearer(JwtBearerDefaults.AuthenticationScheme, options =>
            {
                options.TokenValidationParameters = new TokenValidationParameters
                {
                    ValidateIssuer = true,
                    ValidateAudience = true,
                    ValidateLifetime = true,
                    ValidateIssuerSigningKey = true,
                    ValidIssuer = issuer,
                    ValidAudience = audience,
                    IssuerSigningKey = symmetricSecurityKey
                };
            })
            .AddIdentityCookies();

        builder.Services.AddAuthorization();
    }
}