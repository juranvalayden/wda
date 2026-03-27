using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using System.Reflection;
using WDA.Application.Interfaces;
using WDA.Application.Services;

namespace WDA.Infrastructure.Persistence;

public class WdaDbContext : IdentityDbContext<ApplicationUser>, IWdaDbContext
{
    public WdaDbContext(DbContextOptions<WdaDbContext> options) : base(options)
    {
    }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);
        
        modelBuilder.HasDefaultSchema("Wda");
        modelBuilder.ApplyConfigurationsFromAssembly(Assembly.GetExecutingAssembly());
    }
}
