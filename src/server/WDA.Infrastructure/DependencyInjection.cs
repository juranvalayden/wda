using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.IdentityModel.Tokens;
using WDA.Application.Interfaces;
using WDA.Application.Services;
using WDA.Infrastructure.Identity;
using WDA.Infrastructure.Persistence;

namespace WDA.Infrastructure;

public static class DependencyInjection
{
    public static void AddInfrastructure(this IHostApplicationBuilder builder)
    {
        var connectionString = builder.Configuration.GetConnectionString("DefaultConnection")
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

        builder.Services.AddAuthentication(options =>
            {
                options.DefaultScheme = IdentityConstants.ApplicationScheme;
                options.DefaultSignInScheme = IdentityConstants.ExternalScheme;
            }).AddIdentityCookies();


        builder.Services.AddAuthorizationBuilder();

        builder.Services
            .AddIdentityCore<ApplicationUser>()
            .AddRoles<IdentityRole>()
            .AddEntityFrameworkStores<WdaDbContext>()
            .AddSignInManager()
            .AddDefaultTokenProviders()
            .AddApiEndpoints();

        builder.Services.AddTransient<IIdentityService, IdentityService>();

        //var secretForKey = builder.Configuration["Authentication:SecretForKey"]
        //                   ?? throw new InvalidOperationException("No Authentication:SecretForKey for key found.");

        //var validIssuer = builder.Configuration["Authentication:Issuer"]
        //                  ?? throw new InvalidOperationException("No Authentication:Issuer found.");

        //var validAudience = builder.Configuration["Authentication:Audience"]
        //                    ?? throw new InvalidOperationException("No Authentication:Audience found.");

        //builder.Services.AddAuthentication("Bearer")
        //    .AddJwtBearer(options =>
        //        {
        //            var optionsTokenValidationParameters = CreateTokenValidationParameters(validIssuer, validAudience, secretForKey);
        //            options.TokenValidationParameters = optionsTokenValidationParameters;
        //        }
        //    );

        //builder.Services.AddAuthorization(options =>
        //{
        //    options.AddPolicy("Admin", policy =>
        //    {
        //        policy.RequireAuthenticatedUser();
        //        policy.RequireClaim("email", "test.test@test.test");
        //    });
        //});


        //builder.Services.AddScoped<ApplicationDbContextInitialiser>();

        //builder.Services.AddAuthorizationBuilder();

        //builder.Services
        //    .AddIdentityCore<User>()
        //    .AddRoles<IdentityRole>()
        //    .AddEntityFrameworkStores<WdaDbContext>()
        //    .AddSignInManager()
        //    .AddDefaultTokenProviders()
        //    .AddApiEndpoints();

        //builder.Services.AddScoped<IUnitOfWork, UnitOfWork>();
        //builder.Services.AddTransient<IIdentityService, IdentityService>();
    }

    private static TokenValidationParameters CreateTokenValidationParameters(string validIssuer, string validAudience, string secretForKey)
    {
        return new TokenValidationParameters()
        {
            ValidateIssuer = true,
            ValidateAudience = true,
            ValidateIssuerSigningKey = true,
            ValidIssuer = validIssuer,
            ValidAudience = validAudience,
            IssuerSigningKey = new SymmetricSecurityKey(Convert.FromBase64String(secretForKey))
        };
    }
}