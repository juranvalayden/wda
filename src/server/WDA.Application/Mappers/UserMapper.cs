using WDA.Application.Dtos;
using WDA.Application.Services;

namespace WDA.Application.Mappers;

public static class UserMapper
{
    public static UserDto MapToDto(ApplicationUser applicationUser) => 
        new(applicationUser.Id,
            applicationUser.FirstName, 
            applicationUser.LastName, 
            applicationUser.Email!);

    public static ApplicationUser MapToEntity(RegisterUserDto registerUserDto) =>
        new(registerUserDto.FirstName, 
            registerUserDto.LastName, 
            registerUserDto.Email,
            registerUserDto.Password,
            registerUserDto.ConfirmedPassword);
}
