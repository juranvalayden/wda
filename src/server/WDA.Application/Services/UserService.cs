using WDA.Application.Dtos;
using WDA.Application.Interfaces;
using WDA.Application.Mappers;
using WDA.Domain.Common;
using WDA.Shared.Errors;

namespace WDA.Application.Services;

public class UserService : IUserService
{
    private readonly IUnitOfWork _unitOfWork;

    public UserService(IUnitOfWork unitOfWork)
    {
        _unitOfWork = unitOfWork ?? throw new ArgumentNullException(nameof(unitOfWork));
    }

    public async Task<Result> GetUserByEmailAsync(string email, CancellationToken cancellationToken = default)
    {
        var user = await _unitOfWork.UserRepository.GetAsync(
            predicate: u => u.Email == email,
            shouldIncludeTracking: false,
            cancellationToken: cancellationToken);

        if (user == null)
        {
            return UserErrors.NotFound(email);
        }

        var userDto = UserMapper.MapToDto(user);
        return Result<UserDto>.Success(userDto);
    }

    public async Task<Result> UserExistsAsync(string email, CancellationToken cancellationToken = default)
    {
        var exists = await _unitOfWork.UserRepository.ExistsAsync(
            predicate: u => u.Email == email,
            cancellationToken: cancellationToken);

        if (exists)
        {
            return UserErrors.AlreadyExists(email);
        }

        return Result<bool>.Success(false);
    }

    public async Task<Result> CreateUserAsync(CreateUserDto createUserDto, CancellationToken cancellationToken = default)
    {
        var createdUser = UserMapper.MapToEntity(createUserDto);

        var user = _unitOfWork.UserRepository.Add(createdUser);

        var hasSaved = await _unitOfWork.SaveChangesAsync(cancellationToken);

        var userDto = UserMapper.MapToDto(user);
        return Result<UserDto>.Success(userDto);
    }
}
