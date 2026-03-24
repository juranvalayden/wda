using WDA.Application.Abstractions.Common;
using WDA.Application.Dtos;

namespace WDA.Application.Users.Commands.CreateUserCommand;

public record CreateUserCommand(CreateUserDto CreateUserDto) : ICommand<CreateUserCommandResult>;