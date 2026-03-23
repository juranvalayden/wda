using WDA.Application.Dtos;
using WDA.Shared.Errors;

namespace WDA.Application.Users.Queries.GetUserByEmail;

public record GetUserByEmailResult
{
    public Result<UserDto> Result { get; set; } = null!;
}