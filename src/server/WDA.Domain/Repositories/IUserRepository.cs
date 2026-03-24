using WDA.Domain.Common;
using WDA.Domain.Entities;

namespace WDA.Domain.Repositories;

public interface IUserRepository : IRepository<User>
{
    Task<User?> GetUserByEmailAsync(string email, bool includeTracking = false,
        CancellationToken cancellationToken = default);

    Task<bool> UserExistsAsync(string email, CancellationToken cancellationToken = default);
}