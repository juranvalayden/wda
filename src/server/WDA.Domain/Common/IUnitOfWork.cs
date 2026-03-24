using WDA.Domain.Entities;

namespace WDA.Domain.Common;

public interface IUnitOfWork : IDisposable
{
    IRepository<User> UserRepository { get; }

    Task<bool> SaveChangesAsync(CancellationToken cancellationToken = default);
}