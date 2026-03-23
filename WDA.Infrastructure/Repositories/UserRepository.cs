using Microsoft.EntityFrameworkCore;
using WDA.Domain.Entities;
using WDA.Domain.Repositories;
using WDA.Infrastructure.Persistence;

namespace WDA.Infrastructure.Repositories;

public class UserRepository : GenericRepository<User>, IUserRepository
{
    private readonly WdaDbContext _wdaDbContext;

    public UserRepository(WdaDbContext wdaDbContext) : base(wdaDbContext)
    {
        _wdaDbContext = wdaDbContext ?? throw new ArgumentNullException(nameof(wdaDbContext));
    }

    public async Task<User?> GetUserByEmailAsync(string email, bool includeTracking = false, CancellationToken cancellationToken = default)
    {
        User? user;

        if (includeTracking)
        {
            user = await _wdaDbContext
                .Users
                .FirstOrDefaultAsync(u => u.Email == email, cancellationToken);

            return user;
        }

        user = await _wdaDbContext
            .Users
            .AsNoTracking()
            .FirstOrDefaultAsync(u => u.Email == email, cancellationToken);

        return user;
    }

    public async Task<bool> UserExistsAsync(string email, CancellationToken cancellationToken = default)
    {
        var exists = await _wdaDbContext
            .Users
            .AsNoTracking()
            .AnyAsync(u => u.Email == email, cancellationToken);

        return exists;
    }
}