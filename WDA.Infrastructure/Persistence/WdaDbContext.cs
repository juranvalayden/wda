using Microsoft.EntityFrameworkCore;
using WDA.Domain.Common;
using WDA.Domain.Entities;
using WDA.Infrastructure.Persistence.Configurations;

namespace WDA.Infrastructure.Persistence;

public class WdaDbContext : DbContext, IUnitOfWork
{
    public DbSet<User> Users { get; set; } = null!;

    public WdaDbContext(DbContextOptions<WdaDbContext> options) : base(options)
    {
    }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);

        modelBuilder.HasDefaultSchema("Wda");
        modelBuilder.ApplyConfiguration(new UserEntityTypeConfiguration());
    }
}
