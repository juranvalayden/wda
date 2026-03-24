using WDA.Application.Abstractions.Common;
using WDA.Application.Dtos;
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

    public async Task<Response> Handle(CreateUserCommand request, CancellationToken cancellationToken)
    {
        var exists = await _userService.UserExistsAsync(request.CreateUserDto.Email, cancellationToken);

        if (exists)
        {
            return UserErrors.AlreadyExists(request.CreateUserDto.Email);
        }

        var createdUserDto = await _userService.CreateUserAsync(request.CreateUserDto, cancellationToken);

        if (createdUserDto == null)
        {
            return UserErrors.ErrorCreatingUser();
        }

        return Response<UserDto>.Success(createdUserDto);
    }
}