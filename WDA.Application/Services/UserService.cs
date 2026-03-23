using WDA.Application.Dtos;
using WDA.Application.Interfaces;
using WDA.Application.Mappers;
using WDA.Domain.Common;
using WDA.Domain.Repositories;

namespace WDA.Application.Services;

public class UserService : IUserService
{
    private readonly IUserRepository _userRepository;
    private readonly IUnitOfWork _unitOfWork;

    public UserService(IUserRepository userRepository, IUnitOfWork unitOfWork)
    {
        _userRepository = userRepository ?? throw new ArgumentNullException(nameof(userRepository));
        _unitOfWork = unitOfWork ?? throw new ArgumentNullException(nameof(unitOfWork));
    }

    public async Task<UserDto?> GetUserByEmailAsync(string email, CancellationToken cancellationToken = default)
    {
        var user = await _userRepository.GetUserByEmailAsync(
            email: email,
            includeTracking: false,
            cancellationToken: cancellationToken);

        return user is not null 
            ? UserMapper.MapToDto(user) 
            : null;
    }

    public async Task<bool> UserExistsAsync(string email, CancellationToken cancellationToken = default)
    {
        var exists = await _userRepository.UserExistsAsync(email, cancellationToken);
        return exists;
    }

    public async Task<UserDto?> CreateUserAsync(CreateUserDto createUserDto, CancellationToken cancellationToken = default)
    {
        var createdUser = UserMapper.MapToEntity(createUserDto);
        
        var user =  _userRepository.Add(createdUser);

        var hasSaved = await _userRepository.UnitOfWork.SaveChangesAsync(cancellationToken) > 0;

        return hasSaved 
            ? UserMapper.MapToDto(user) 
            : null;
    }
}
