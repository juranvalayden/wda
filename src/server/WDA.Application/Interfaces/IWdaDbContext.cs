namespace WDA.Application.Interfaces;

public interface IWdaDbContext
{
    Task<int> SaveChangesAsync(CancellationToken cancellationToken = default);
}