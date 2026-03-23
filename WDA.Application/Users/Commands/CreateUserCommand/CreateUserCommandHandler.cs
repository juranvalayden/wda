using WDA.Application.Abstractions.Common;
using WDA.Application.Interfaces;
using WDA.Shared.Errors;

namespace WDA.Application.Users.Commands.CreateUserCommand;

public class CreateUserCommandHandler : ICommandHandler<CreateUserCommand>
{
    private readonly IUserService _userService;

    public CreateUserCommandHandler(IUserService userService)
    {
        _userService = userService ?? throw new ArgumentNullException(nameof(userService));
    }

    public async Task<Result> Handle(CreateUserCommand command, CancellationToken cancellationToken = default)
    {
        var exists = await _userService.UserExistsAsync(command.CreateUserDto.Email, cancellationToken);

        if (exists)
        {
            return UserErrors.AlreadyExists(command.CreateUserDto.Email);
        }

        var createdUser = await _userService.CreateUserAsync(command.CreateUserDto, cancellationToken);

        if (createdUser != null)
        {
            return Result<string>.Success(createdUser.Email);
        }

        return UserErrors.ErrorSaving();
    }
}