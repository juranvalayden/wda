using WDA.Domain.Common;
using WDA.Domain.Entities;
using WDA.Infrastructure.Persistence;

namespace WDA.Infrastructure.Repositories;

public class UnitOfWork : IUnitOfWork
{
    private readonly WdaDbContext _wdaDbContext;

    public IRepository<User> UserRepository => field ??= new UserRepository(_wdaDbContext);

    public UnitOfWork(WdaDbContext wdaDbContext)
    {
        _wdaDbContext = wdaDbContext ?? throw new ArgumentNullException(nameof(wdaDbContext));
    }

    public async Task<bool> SaveChangesAsync(CancellationToken cancellationToken = default)
    {
        return await _wdaDbContext.SaveChangesAsync(cancellationToken) > 0;
    }

    public void Dispose()
    {
        _wdaDbContext.Dispose();
    }
}
