using WDA.Application.Abstractions.Common;
using WDA.Application.Interfaces;
using WDA.Shared.Errors;

namespace WDA.Application.Users.Commands.CreateUserCommand;

public class CreateUserCommandHandler : ICommandHandler<CreateUserCommand, CreateUserCommandResult>
{
    private readonly IUserService _userService;

    public CreateUserCommandHandler(IUserService userService)
    {
        _userService = userService ?? throw new ArgumentNullException(nameof(userService));
    }

    public async Task<CreateUserCommandResult> Handle(CreateUserCommand request, CancellationToken cancellationToken)
    {
        var userExistsResult = await _userService.UserExistsAsync(request.CreateUserDto.Email, cancellationToken);

        if (userExistsResult is not Success<bool> { Data: false }) return new CreateUserCommandResult(Result: userExistsResult);

        var result = await _userService.CreateUserAsync(request.CreateUserDto, cancellationToken);
        return new CreateUserCommandResult(result);
    }
}