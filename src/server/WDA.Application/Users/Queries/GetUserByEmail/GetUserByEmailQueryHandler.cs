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

    public async Task<Response> Handle(GetUserByEmailQuery request, CancellationToken cancellationToken)
    {
        var userDto = await _userService.GetUserByEmailAsync(request.Email, cancellationToken);

        if (userDto == null)
        {
            return UserErrors.NotFound(request.Email);
        }

        return Response<UserDto>.Success(userDto);
    }

}
