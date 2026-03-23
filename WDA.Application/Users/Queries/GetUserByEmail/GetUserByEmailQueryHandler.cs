using WDA.Application.Abstractions.Common;
using WDA.Application.Dtos;
using WDA.Application.Interfaces;
using WDA.Shared.Errors;

namespace WDA.Application.Users.Queries.GetUserByEmail;

public class GetUserByEmailQueryHandler : IQueryHandler<GetUserByEmailQuery>
{
    private readonly IUserService _userService;

    public GetUserByEmailQueryHandler(IUserService userService)
    {
        _userService = userService ?? throw new ArgumentNullException(nameof(userService));
    }

    public async Task<Result> Handle(GetUserByEmailQuery query, CancellationToken cancellationToken = default)
    {
        var userDto = await _userService.GetUserByEmailAsync(query.Email, cancellationToken);
        
        if (userDto == null)
        {
            return UserErrors.NotFound(query.Email);
        }

        return Result<UserDto>.Success(userDto);
    }
}