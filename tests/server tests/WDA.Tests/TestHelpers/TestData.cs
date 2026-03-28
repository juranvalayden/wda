using WDA.Application.Dtos;
using WDA.Application.Services;
using WDA.Shared;

namespace WDA.Tests.TestHelpers;

public static class TestData
{
    public static ApplicationUser ValidApplicationUser()
    {
        return new ApplicationUser(Constants.TestFirstName, Constants.TestLastName, Constants.TestUsername,
            Constants.TestPassword, Constants.TestPassword);
    }

    public static UserDto ValidUserDto(string id)
    {
        return new UserDto(id, Constants.TestFirstName, Constants.TestLastName, Constants.TestUsername);
    }

    public static RegisterUserDto ValidRegisterUserDto()
    {
        return new RegisterUserDto(Constants.TestFirstName, Constants.TestLastName, Constants.TestUsername,
            Constants.TestPassword, Constants.TestPassword);
    }

    public static RegisterUserDto InvalidRegisterUserDto()
    {
        return new RegisterUserDto("Bad", Constants.TestLastName, Constants.TestUsername,
            Constants.TestPassword, "Bad");
    }

    public static LoginUserDto ValidLoginUserDto()
    {
        return new LoginUserDto(Constants.TestUsername, Constants.TestPassword);
    }

    public static LoginUserDto InvalidLoginUserDto()
    {
        return new LoginUserDto("Bad", Constants.TestPassword);
    }
}