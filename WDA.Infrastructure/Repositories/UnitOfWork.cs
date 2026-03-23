using Microsoft.EntityFrameworkCore;
using WDA.Domain.Common;
using WDA.Domain.Entities;
using WDA.Domain.Repositories;
using WDA.Infrastructure.Persistence;

namespace WDA.Infrastructure.Repositories;

public class UnitOfWork : IUnitOfWork
{
    private readonly WdaDbContext _wdaDbContext;

    private IRepository<User>? _userRepository;
    public IRepository<User> UserRepository => _userRepository ??= new UserRepository(_wdaDbContext);

    public UnitOfWork(WdaDbContext wdaDbContext)
    {
        _wdaDbContext = wdaDbContext ?? throw new ArgumentNullException(nameof(wdaDbContext));
    }

    public async Task<int> SaveChangesAsync(CancellationToken cancellationToken = default)
    {
        return await _wdaDbContext.SaveChangesAsync(cancellationToken);
    }

    public ValueTask DisposeAsync()
    {
        _wdaDbContext.Dispose();
        _userRepository = null;
        return ValueTask.CompletedTask;
    }
}
