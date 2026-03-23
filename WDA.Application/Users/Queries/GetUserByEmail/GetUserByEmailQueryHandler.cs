using WDA.Application.Abstractions.Common;
using WDA.Application.Dtos;
using WDA.Application.Interfaces;
using WDA.Shared.Errors;

namespace WDA.Application.Users.Queries.GetUserByEmail;

public class GetUserByEmailQueryHandler : IQueryHandler<GetUserByEmailQuery, GetUserByEmailResult>
{
    private readonly IUserService _userService;

    public GetUserByEmailQueryHandler(IUserService userService)
    {
        _userService = userService ?? throw new ArgumentNullException(nameof(userService));
    }

    public async Task<GetUserByEmailResult> Handle(GetUserByEmailQuery query, CancellationToken cancellationToken = default)
    {
        var userDto = await _userService.GetUserByEmailAsync(query.Email, cancellationToken);

        if (userDto == null)
        {
            return new GetUserByEmailResult
            {
                Result = Result<UserDto>.Failure(UserErrors.NotFound(query.Email))
            };
        }

        return Result<GetUserByEmailQueryResult>.Success(userDto);
    }
}