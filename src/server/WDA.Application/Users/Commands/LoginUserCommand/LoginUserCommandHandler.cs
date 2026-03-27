using WDA.Application.Abstractions.Common;
using WDA.Application.Interfaces;
using WDA.Application.Services;
using WDA.Shared.Errors;

namespace WDA.Application.Users.Commands.LoginUserCommand;

public class LoginUserCommandHandler : IHandler<LoginUserCommand>
{
    private readonly IIdentityService _identityService;

    public LoginUserCommandHandler(IIdentityService identityService)
    {
        _identityService = identityService ?? throw new ArgumentNullException(nameof(identityService));
    }

    public async Task<Response> Handle(LoginUserCommand request, CancellationToken cancellationToken)
    {
        var response = await _identityService.GetUserByEmailAsync(request.LoginUserDto.Email);

        if (response is not Response<ApplicationUser> successApplicationUserResponse) return response;

        var token = _identityService.GenerateToken(successApplicationUserResponse.Data!);

        if (string.IsNullOrWhiteSpace(token))
        {
            return UserErrors.Null;
        }

        return Response<string>.Success(token);
    }
}
