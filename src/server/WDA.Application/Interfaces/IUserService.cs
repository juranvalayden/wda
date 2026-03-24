using WDA.Application.Dtos;

namespace WDA.Application.Interfaces;

public interface IUserService
{
    Task<UserDto?> GetUserByEmailAsync(string email, CancellationToken cancellationToken = default);
    Task<bool> UserExistsAsync(string email, CancellationToken cancellationToken = default);

    Task<UserDto?> CreateUserAsync(CreateUserDto createUserDto, CancellationToken cancellationToken = default);
}