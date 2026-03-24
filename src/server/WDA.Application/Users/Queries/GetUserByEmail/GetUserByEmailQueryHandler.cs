using WDA.Application.Abstractions.Common;
using WDA.Application.Interfaces;

namespace WDA.Application.Users.Queries.GetUserByEmail;

public class GetUserByEmailQueryHandler : IQueryHandler<GetUserByEmailQuery, GetUserByEmailResult>
{
    private readonly IUserService _userService;

    public GetUserByEmailQueryHandler(IUserService userService)
    {
        _userService = userService ?? throw new ArgumentNullException(nameof(userService));
    }

    public async Task<GetUserByEmailResult> Handle(GetUserByEmailQuery request, CancellationToken cancellationToken)
    {
        var result = await _userService.GetUserByEmailAsync(request.Email, cancellationToken);
        return new GetUserByEmailResult(result);
    }
}
