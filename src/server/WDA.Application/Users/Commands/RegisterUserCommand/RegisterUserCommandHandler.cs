using WDA.Application.Abstractions.Common;
using WDA.Application.Interfaces;
using WDA.Application.Mappers;
using WDA.Shared.Errors;

namespace WDA.Application.Users.Commands.RegisterUserCommand;

public class RegisterUserCommandHandler : IHandler<RegisterUserCommand>
{
    private readonly IIdentityService _identityService;

    public RegisterUserCommandHandler(IIdentityService identityService)
    {
        _identityService = identityService ?? throw new ArgumentNullException(nameof(identityService));
    }

    public async Task<Response> Handle(RegisterUserCommand request, CancellationToken cancellationToken)
    {
        var applicationUser = UserMapper.MapToEntity(request.RegisterUserDto);
        return await _identityService.RegisterUserAsync(applicationUser);
    }
}