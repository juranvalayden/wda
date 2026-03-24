using Microsoft.Extensions.Logging;
using WDA.Domain.Common;
using WDA.Domain.Entities;
using WDA.Infrastructure.Persistence;

namespace WDA.Infrastructure.Repositories;

public class UnitOfWork : IUnitOfWork
{
    private readonly ILogger<UnitOfWork> _logger;
    private readonly WdaDbContext _wdaDbContext;

    public IRepository<User> UserRepository => field ??= new UserRepository(_wdaDbContext);

    public UnitOfWork(ILogger<UnitOfWork> logger, WdaDbContext wdaDbContext)
    {
        _logger = logger ?? throw new ArgumentNullException(nameof(logger));
        _wdaDbContext = wdaDbContext ?? throw new ArgumentNullException(nameof(wdaDbContext));
    }

    public async Task<bool> SaveChangesAsync(CancellationToken cancellationToken = default)
    {
        try
        {
            return await _wdaDbContext.SaveChangesAsync(cancellationToken) > 0;
        }
        catch (Exception exception)
        {
            _logger.LogError(exception, "{ClassFailure} failed to save changes.", nameof(UnitOfWork));
            throw;
        }
    }

    public void Dispose()
    {
        try
        {
            _wdaDbContext.Dispose();
        }
        catch (Exception exception)
        {
            _logger.LogError(exception, "{ClassFailure} failed to dispose.", nameof(WdaDbContext));
            throw;
        }
    }
}
