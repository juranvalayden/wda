using System.Linq.Expressions;

namespace WDA.Domain.Common;

public interface IRepository<T> where T : class
{
    IUnitOfWork UnitOfWork { get; }

    Task<T?> GetAsync(Expression<Func<T, bool>> predicate, CancellationToken cancellationToken = default);
    Task<bool> ExistsAsync(Expression<Func<T, bool>> predicate, CancellationToken cancellationToken = default);
    T Add(T entity);
}