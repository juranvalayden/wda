using WDA.Application.Abstractions.Common;
using WDA.Application.Interfaces;
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
        return await _identityService.GetUserByEmailAsync(request.LoginUserDto.Email);
    }
}
