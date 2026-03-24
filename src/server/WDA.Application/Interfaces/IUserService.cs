using WDA.Application.Dtos;
using WDA.Shared.Errors;

namespace WDA.Application.Interfaces;

public interface IUserService
{
    Task<Result> GetUserByEmailAsync(string email, CancellationToken cancellationToken = default);
    Task<Result> UserExistsAsync(string email, CancellationToken cancellationToken = default);

    Task<Result> CreateUserAsync(CreateUserDto createUserDto, CancellationToken cancellationToken = default);
}