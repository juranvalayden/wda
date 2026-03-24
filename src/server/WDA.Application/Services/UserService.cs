using WDA.Application.Dtos;
using WDA.Application.Interfaces;
using WDA.Application.Mappers;
using WDA.Domain.Common;

namespace WDA.Application.Services;

public class UserService : IUserService
{
    private readonly IUnitOfWork _unitOfWork;

    public UserService(IUnitOfWork unitOfWork)
    {
        _unitOfWork = unitOfWork ?? throw new ArgumentNullException(nameof(unitOfWork));
    }

    public async Task<UserDto?> GetUserByEmailAsync(string email, CancellationToken cancellationToken = default)
    {
        var user = await _unitOfWork.UserRepository.GetAsync(
            predicate: u => u.Email == email,
            shouldIncludeTracking: false,
            cancellationToken: cancellationToken);

        return user == null 
            ? null 
            : UserMapper.MapToDto(user);
    }

    public async Task<bool> UserExistsAsync(string email, CancellationToken cancellationToken = default)
    {
        return await _unitOfWork.UserRepository.ExistsAsync(
            predicate: u => u.Email == email,
            cancellationToken: cancellationToken);
    }

    public async Task<UserDto?> CreateUserAsync(CreateUserDto createUserDto, CancellationToken cancellationToken = default)
    {
        var createdUser = UserMapper.MapToEntity(createUserDto);

        var user = _unitOfWork.UserRepository.Add(createdUser);

        var hasSaved = await _unitOfWork.SaveChangesAsync(cancellationToken);

        return !hasSaved 
            ? null 
            : UserMapper.MapToDto(user);
    }
}
