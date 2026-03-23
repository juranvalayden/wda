using System.Linq.Expressions;
using Microsoft.EntityFrameworkCore;
using WDA.Domain.Common;
using WDA.Infrastructure.Persistence;

namespace WDA.Infrastructure.Repositories;

public abstract class GenericRepository<T> : IRepository<T> where T : class
{
    private readonly WdaDbContext _wdaDbContext;
    private readonly DbSet<T> _dbSet;

    public IUnitOfWork UnitOfWork => _wdaDbContext;

    protected GenericRepository(WdaDbContext wdaDbContext)
    {
        _wdaDbContext = wdaDbContext ?? throw new ArgumentNullException(nameof(wdaDbContext));
        _dbSet = wdaDbContext.Set<T>();
    }

    public virtual async Task<T?> GetAsync(Expression<Func<T, bool>> predicate, CancellationToken cancellationToken = default)
    {
        return await _dbSet.AsNoTracking().FirstOrDefaultAsync(predicate, cancellationToken);
    }

    public virtual async Task<bool> ExistsAsync(Expression<Func<T, bool>> predicate, CancellationToken cancellationToken = default)
    {
        return await _dbSet.AsNoTracking().AnyAsync(predicate, cancellationToken);
    }

    public virtual T Add(T entity)
    {
        return _dbSet.Add(entity).Entity;
    }
}