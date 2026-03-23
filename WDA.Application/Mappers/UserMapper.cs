using WDA.Application.Dtos;
using WDA.Domain.Entities;

namespace WDA.Application.Mappers;

public static class UserMapper
{
    public static UserDto MapToDto(User user) => 
        new(user.Id,
            user.FirstName, 
            user.LastName, 
            user.Email);

    public static User MapToEntity(CreateUserDto createUserDto) => 
        new(createUserDto.FirstName, 
            createUserDto.LastName,
            createUserDto.Email, 
            createUserDto.Password);
}
